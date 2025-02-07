using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using CxWorkStation.Utilities;
using OpenCvSharp.Flann;
using HelloWinForms.Utilities;
using HelloWinForms.Protocols;
using System.Runtime.InteropServices.ComTypes;
using System.Globalization;
using HelloWinForms.Channel;
using HalconDotNet;
using Newtonsoft.Json;
using System.Diagnostics;
using CxAAC.Utilities;
using System.Reflection;

namespace HelloWinForms.PLC
{
    // 仿真过程
    // 每隔 1.5 秒发送一次产品检测（ #pos:11=扫码请求；21=Xray采图请求，会相隔 100ms）
    // 7个产品检测后并到出料口，请求结果（ #pos:100=请求最终结果；会相隔 100ms）
    public partial class AACPLCServer : Form
    {
        private const ushort CMD_PC = 1; 
        private const ushort POS_QRCodeAcquisition = 31;
        private const ushort POS_ImageAcquisition = 41;
        private const ushort POS_Result = 100;
        // 结果常数：1=ok;2=busy;3=fault
        public const ushort PLCResultOk = 1;
        public const ushort PLCResultBusy = 2;
        public const ushort PLCResultFault = 3;
        public const ushort PLCResultWait = 10;

        private TcpListener _listener;
        private volatile bool _isRunning;
        private readonly List<Thread> _clientThreads = new List<Thread>();
        private ConcurrentDictionary<string, TcpClient> _connectedClients = new ConcurrentDictionary<string, TcpClient>();
        private Random _random = new Random();
        private volatile uint _moduleNumber = 0;
        private volatile uint _batchNumber = 1;

        private volatile bool _position1HasProduct = false;
        private volatile bool _position2HasProduct = false;
        private volatile bool _position3HasProduct = false;
        
        public AACPLCServer()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            _position1HasProduct = checkBox1.Checked;
            _position2HasProduct = checkBox2.Checked;
            _position3HasProduct = checkBox3.Checked;
            Start();
        }

        public void Start()
        {
            if (_isRunning) return;
            _isRunning = true;

            _listener = new TcpListener(IPAddress.Parse(ipTextBox.Text), (ushort)portNumericUpDown.Value);
            _listener.Start();

            Thread acceptThread = new Thread(() =>
            {
                try
                {
                    while (_isRunning)
                    {
                        TcpClient client = _listener.AcceptTcpClient();
                        Thread clientThread = new Thread(() => HandleClient(client));
                        clientThread.IsBackground = true;
                        clientThread.Start();
                        _clientThreads.Add(clientThread);
                    }
                }
                catch (SocketException e)
                {
                    Console.WriteLine($"SocketException: {e.Message}");
                }
                finally
                {
                    _listener?.Stop();
                    _isRunning = false;
                }
            });
            acceptThread.IsBackground = true;
            acceptThread.Start();
        }

        public void Stop()
        {
            _isRunning = false;

            if (_listener != null)
            {
                _listener.Stop();
                _listener = null;
            }

            //foreach (var thread in clientThreads)
            //{
            //    if (thread.IsAlive)
            //    {
            //        thread.Join();
            //    }
            //}
            _clientThreads.Clear();
            Console.WriteLine("Server stopped.");
        }


        private List<string> SplitMessageIntoRandomParts(string message)
        {
            List<string> parts = new List<string>();
            int totalLength = message.Length;
            while (totalLength > 0)
            {
                int partLength = _random.Next(1, Math.Min(50, totalLength + 1)); // 随机选择部分的长度，假设最大不超过50个字符
                parts.Add(message.Substring(0, partLength)); // 添加消息的一部分到列表
                message = message.Substring(partLength); // 更新消息，移除已经添加的部分
                totalLength -= partLength; // 更新剩余长度
            }
            return parts;
        }

        private void OutputMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(OutputMessage), message);
            }
            else
            {
                richTextBox1.AppendText(message + Environment.NewLine);
            }
        }

        private NetworkStream _stream =  null;

        private void HandleClient(TcpClient client)
        {
            var clientEndpoint = client.Client.RemoteEndPoint.ToString();
            _connectedClients.TryAdd(clientEndpoint, client);
            Console.WriteLine("Client connected.");
            try
            {
                NetworkStream stream = client.GetStream();
                _stream = stream;
                stream.WriteTimeout = 100;
                stream.ReadTimeout = 100;
                byte[] buffer = new byte[client.ReceiveBufferSize];
                List<byte> _plcMessageBuffer = new List<byte>();

                long lastSendTime = 0;
                long lastAcquisitionTime = 0;
                List<PlcMessage> plcMessages = new List<PlcMessage>();
                
                while (_isRunning)
                {
                    try
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            // 把 buffer bytesRead 中的数据放到 _plcMessageBuffer 中
                            var plcFeedback = ProcessUpdatePlcData(_plcMessageBuffer, buffer.Take(bytesRead).ToArray());
                            if (plcFeedback != null)
                            {
                                // 接收 QRCode，结果
                                if (plcFeedback.Pos == POS_QRCodeAcquisition)
                                {
                                    var qrCodes = plcFeedback.Sn1 + ";" + plcFeedback.Sn2 + ";" + plcFeedback.Sn3;
                                    uint fromBatchNumber = plcFeedback.BatchNumber;
                                    uint sendBatchNumber = 0;

                                    LogHelper.Debug($"received 读码:{qrCodes} fromBatchNumber:{fromBatchNumber} sendBatchNumber:{sendBatchNumber}");
                                }
                                else if (plcFeedback.Pos == POS_ImageAcquisition)
                                {
                                    if (plcFeedback.Result == PLCResultOk)
                                    {
                                        lastAcquisitionTime = TimeHelper.GetNow();
                                    }
                                    
                                    uint fromBatchNumber = plcFeedback.BatchNumber;
                                    uint sendBatchNumber = 0;

                                    LogHelper.Debug($"received 采图 result:{plcFeedback.Result} fromBatchNumber:{fromBatchNumber} sendBatchNumber:{sendBatchNumber}");
                                }
                                else if (plcFeedback.Pos == POS_Result)
                                {
                                    var result = plcFeedback.Number1 + ";" + plcFeedback.Number2 + ";" + plcFeedback.Number3;
                                    uint fromBatchNumber = plcFeedback.BatchNumber;
                                    uint sendBatchNumber = 0;

                                    LogHelper.Debug($"received 结果:{result} fromBatchNumber:{fromBatchNumber} sendBatchNumber:{sendBatchNumber}");
                                }
                            }
                        }
                        else if (bytesRead == 0)
                        {
                            // The client has disconnected
                            throw new Exception("Client disconnected.");
                        }
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx && (socketEx.SocketErrorCode == SocketError.TimedOut))
                    {
                        Console.WriteLine("Read timed out.");
                    }
                    catch (Exception)
                    {
                        // 除了超时异常，其他都认为是异常，则尝试重新连接
                        throw;
                    }
                    
                    var dtNow = DateTime.Now;
                    var nowMs = TimeHelper.GetMs(dtNow);

                    // 检查是否需要发送新的消息
                    //if (nowMs - lastSendTime >= 3000 * 1234567890123L)
                    if (lastSendTime == 0 || (lastAcquisitionTime > lastSendTime && nowMs - lastSendTime >= 5000))
                    {
                        lastSendTime = nowMs;
                        var plcMessage = new PlcMessage
                        {
                            ModuleNumber = _moduleNumber++,
                            BatchNumber = _batchNumber++,
                            Cmd = CMD_PC,
                            Dt = dtNow,
                            Sn1 = _position1HasProduct ? "1" : "0",
                            Sn2 = _position2HasProduct ? "1" : "0",
                            Sn3 = _position3HasProduct ? "1" : "0",
                        };

                        plcMessage.SendImageAcquisitionTime = 0;
                        plcMessage.ReceivedQRCodeTime = 0;
                        plcMessage.ReceivedImageTime = 0;
                        plcMessage.SendResultTime = 0;
                        plcMessage.ReceivedResultTime = 0;

                        // 请求读码
                        plcMessage.Pos = POS_QRCodeAcquisition;
                        SendPlcMessage(stream, plcMessage);

                        Thread.Sleep(100);

                        // 请求采图
                        plcMessage.Pos = POS_ImageAcquisition;
                        SendPlcMessage(stream, plcMessage);
                        LogHelper.Debug($"发送采图请求 BatchNumber:{plcMessage.BatchNumber} Pos:{plcMessage.Pos} Sn1:{plcMessage.Sn1} Sn2:{plcMessage.Sn2} Sn3:{plcMessage.Sn3} Sn4:{plcMessage.Sn4}");

                        plcMessages.Add(plcMessage);

                        if (doubleDetectorCheckBox.Checked)
                        {
                            Thread.Sleep(100);
                            // 请求读码
                            var plcMessageTwo = plcMessage.Clone();
                            plcMessageTwo.ModuleNumber = 2;
                            plcMessageTwo.BatchNumber = _batchNumber++;
                            plcMessageTwo.Pos = POS_QRCodeAcquisition;
                            SendPlcMessage(stream, plcMessageTwo);

                            Thread.Sleep(100);

                            // 请求采图
                            plcMessageTwo.Pos = POS_ImageAcquisition;
                            SendPlcMessage(stream, plcMessageTwo);
                            LogHelper.Debug($"发送采图请求 BatchNumber:{plcMessageTwo.BatchNumber} Pos:{plcMessageTwo.Pos} Sn1:{plcMessageTwo.Sn1} Sn2:{plcMessageTwo.Sn2} Sn3:{plcMessageTwo.Sn3} Sn4:{plcMessageTwo.Sn4}");

                            plcMessages.Add(plcMessageTwo);
                        }
                    }

                    int CountPlcMessageSent = 3;
                    if (doubleDetectorCheckBox.Checked)
                    {
                        CountPlcMessageSent = 5;
                    }

                    if (plcMessages.Count > CountPlcMessageSent)
                    {
                        foreach (var plcMessage in plcMessages)
                        {
                            var timeDifference = dtNow - plcMessage.Dt;
                            if (timeDifference.TotalMilliseconds >= 10000 && plcMessage.SendResultTime <= 0)
                            {
                                // 请求结果
                                plcMessage.Pos = POS_Result;
                                Thread.Sleep(100);
                                SendPlcMessage(stream, plcMessage);
                                plcMessage.SendResultTime = nowMs;
                                LogHelper.Debug($"发送请求结果 BatchNumber:{plcMessage.BatchNumber} Pos:{plcMessage.Pos} Sn1:{plcMessage.Sn1} Sn2:{plcMessage.Sn2} Sn3:{plcMessage.Sn3} Sn4:{plcMessage.Sn4}");
                            }
                        }

                        for (var i = plcMessages.Count - 1; i >= 0; i--)
                        {
                            var plcMessage = plcMessages[i];
                            if (plcMessage.SendResultTime > 0)
                            {
                                // 删除
                                plcMessages.Remove(plcMessage);
                                LogHelper.Debug($"删除已经发送的结果请求的 BatchNumber:{plcMessage.BatchNumber} ");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                TcpClient removedClient;
                _connectedClients.TryRemove(clientEndpoint, out removedClient);
                client.Close();
            }
        }

        private bool SendPlcMessage(NetworkStream stream, PlcMessage plcMessage)
        {
            try
            {
                string message = plcMessage.ToMessage();
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                stream.Write(messageBytes, 0, messageBytes.Length);
                LogHelper.Debug($"{DateTime.Now:dd/HH:mm:ss.fff} Sent: Cmd:{plcMessage.Cmd} BatchNumber:{plcMessage.BatchNumber} Pos:{plcMessage.Pos} Sn1:{plcMessage.Sn1} Sn2:{plcMessage.Sn2} Sn3:{plcMessage.Sn3} Sn4:{plcMessage.Sn4}");
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Debug($"{DateTime.Now:dd/HH:mm:ss.fff} Sent: Cmd:{plcMessage.Cmd} BatchNumber:{plcMessage.BatchNumber} Pos:{plcMessage.Pos} Sn1:{plcMessage.Sn1} Sn2:{plcMessage.Sn2} Sn3:{plcMessage.Sn3} Sn4:{plcMessage.Sn4}");
            }
            return false;
        }

        private void HandleClient1(TcpClient client)
        {
            var clientEndpoint = client.Client.RemoteEndPoint.ToString();
            _connectedClients.TryAdd(clientEndpoint, client);
            Console.WriteLine("Client connected.");
            try
            {
                NetworkStream stream = client.GetStream();
                stream.WriteTimeout = 1000;
                stream.ReadTimeout = 100;
                byte[] buffer = new byte[client.ReceiveBufferSize];
                List<byte> _plcMessageBuffer = new List<byte>();

                // 定义10个SN码（20个字符）

                while (_isRunning)
                {
                    try
                    {
                        int bytesRead = stream.Read(buffer, 0, buffer.Length);
                        if (bytesRead > 0)
                        {
                            // 把 buffer bytesRead 中的数据放到 _plcMessageBuffer 中
                            var plcFeedback = ProcessUpdatePlcData(_plcMessageBuffer, buffer.Take(bytesRead).ToArray());
                            if (plcFeedback != null)
                            {
                                // 根据命令处理，如果当前位置已经采图完成
                                LogHelper.Debug(plcFeedback.ToString());
                            }
                        }
                        else if (bytesRead == 0)
                        {
                            // The client has disconnected
                            throw new Exception("Client disconnected.");
                        }
                    }
                    catch (IOException ex) when (ex.InnerException is SocketException socketEx && (socketEx.SocketErrorCode == SocketError.TimedOut))
                    {
                        Console.WriteLine("Read timed out.");
                    }
                    catch (Exception)
                    {
                        // 除了超时异常，其他都认为是异常，则尝试重新连接
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception: {e.Message}");
            }
            finally
            {
                TcpClient removedClient;
                _connectedClients.TryRemove(clientEndpoint, out removedClient);
                client.Close();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Stop();
        }

        static uint ExtractNumberFromPath(string path)
        {
            // 定义可能的关键字
            string[] keys = { "original.", "detector.", "prediction." };

            foreach (string key in keys)
            {
                int keyIndex = path.IndexOf(key); // 找到关键字的位置
                if (keyIndex != -1)
                {
                    keyIndex += key.Length; // 定位到数字的开始位置
                    int endIndex = keyIndex;

                    // 找到数字的结束位置
                    while (endIndex < path.Length && char.IsDigit(path[endIndex]))
                    {
                        endIndex++;
                    }

                    // 提取数字字符串并尝试转换为 uint
                    string numberStr = path.Substring(keyIndex, endIndex - keyIndex);
                    if (uint.TryParse(numberStr, out uint number))
                    {
                        return number; // 返回解析出的 uint 值
                    }
                }
            }

            return 0; // 如果没有找到任何关键字或数字，返回 0
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Stop();
            Thread.Sleep(30);
            Start();
        }

        public class UserDrawState
        {
            // 使用 JsonIgnore 来忽略此属性的序列化
            [JsonIgnore]
            public List<DrawObjectInfo> DrawObjectList { get; private set; }

            public List<DrawMeasureLineInfo> MeasureLines { get; private set; }
            public List<DrawRectangleInfo> UserRectangles { get; private set; }
            public List<DrawCircleInfo> UserCircles { get; private set; }

            // 同样使用 JsonIgnore 来忽略此属性的序列化
            [JsonIgnore]
            public List<DrawActionType> ActionTypes { get; private set; }

            public UserDrawState(
                List<DrawObjectInfo> drawObjectList,
                List<DrawMeasureLineInfo> measureLines,
                List<DrawRectangleInfo> userRectangles,
                List<DrawCircleInfo> userCircles,
                List<DrawActionType> actionTypes)
            {
                DrawObjectList = new List<DrawObjectInfo>(drawObjectList);
                MeasureLines = new List<DrawMeasureLineInfo>(measureLines);
                UserRectangles = new List<DrawRectangleInfo>(userRectangles);
                UserCircles = new List<DrawCircleInfo>(userCircles);
                ActionTypes = new List<DrawActionType>(actionTypes);
            }

            public UserDrawState()
            {
                DrawObjectList = new List<DrawObjectInfo>();
                MeasureLines = new List<DrawMeasureLineInfo>();
                UserRectangles = new List<DrawRectangleInfo>();
                UserCircles = new List<DrawCircleInfo>();
                ActionTypes = new List<DrawActionType>();
            }
        }
        public class DrawObjectInfo
        {
            public HObject HObject;
            public int LineWidth;
            public string Color;
            public string DrawMode;
            public int Font;
            public Action<HTuple> DrawAction;
            public DrawTextInfo TextInfo;

        }

        public class DrawTextInfo
        {
            public string Text { get; set; }
            public double Row { get; set; }
            public double Col { get; set; }
            public string Color { get; set; }
        }

        public enum DrawActionType
        {
            AddRectangle,
            AddCircle,
            AddMeasureLine
        }

        /// <summary>
        /// 测量线信息
        /// </summary>
        public class DrawMeasureLineInfo
        {
            /// <summary>
            /// 线段起始点
            /// </summary>
            public Point Start { get; set; }

            /// <summary>
            /// 线段结束点
            /// </summary>
            public Point End { get; set; }

            /// <summary>
            /// 线段长度
            /// </summary>
            public double Length { get; set; }
        }

        /// <summary>
        /// 矩形信息
        /// </summary>
        public class DrawRectangleInfo
        {
            /// <summary>
            /// 矩形区域
            /// </summary>
            public Rectangle Rectangle { get; set; }

            /// <summary>
            /// 平均灰度值
            /// </summary>
            public double GrayValue { get; set; }
        }

        /// <summary>
        /// 圆信息
        /// </summary>
        public class DrawCircleInfo
        {
            /// <summary>
            /// 圆心坐标
            /// </summary>
            public Point Center { get; set; }

            /// <summary>
            /// 半径
            /// </summary>
            public int Radius { get; set; }

            /// <summary>
            /// 平均灰度值
            /// </summary>
            public double GrayValue { get; set; }
        }

        /// <summary>
        /// 将 UserDrawState 对象序列化成字节数组（内部为 JSON）
        /// </summary>
        /// <param name="state">UserDrawState 对象</param>
        /// <returns>序列化后得到的字节数组</returns>
        public static byte[] SerializeToBytes(UserDrawState state)
        {
            // 序列化配置
            var settings = new JsonSerializerSettings
            {
                // Formatting.Indented 让 JSON 更容易阅读（带缩进）
                Formatting = Formatting.Indented,
                // ReferenceLoopHandling.Ignore 用于在出现循环引用时避免错误
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // 先序列化为 JSON 字符串
            string jsonString = JsonConvert.SerializeObject(state, settings);

            // 再转换为 UTF8 字节数组
            return Encoding.UTF8.GetBytes(jsonString);
        }

        /// <summary>
        /// 从字节数组反序列化回 UserDrawState 对象
        /// </summary>
        /// <param name="data">字节数组（内容是 JSON）</param>
        /// <returns>反序列化得到的对象</returns>
        public static UserDrawState DeserializeFromBytes(byte[] data)
        {
            if (data == null || data.Length == 0)
                return null; // 或者返回一个新的 UserDrawState()

            // 先从字节数组还原 UTF8 字符串
            string jsonString = Encoding.UTF8.GetString(data);

            // 反序列化配置
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            // 反序列化回 UserDrawState 对象
            return JsonConvert.DeserializeObject<UserDrawState>(jsonString, settings);
        }

        public static void testSerialize()
        {
            // 1. 准备一些测试数据
            var drawObjectList = new List<DrawObjectInfo>
            {
                new DrawObjectInfo { LineWidth = 1, Color = "LineObject" },
                new DrawObjectInfo { LineWidth = 2, Color = "CircleObject" }
            };

            var measureLines = new List<DrawMeasureLineInfo>
            {
                new DrawMeasureLineInfo
                {
                    Start = new Point(0, 0),
                    End = new Point(10, 0),
                    Length = 10
                },
                new DrawMeasureLineInfo
                {
                    Start = new Point(10, 10),
                    End = new Point(10, 20),
                    Length = 10
                }
            };

            var rectangles = new List<DrawRectangleInfo>
            {
                new DrawRectangleInfo
                {
                    Rectangle = new Rectangle(0, 0, 50, 50),
                    GrayValue = 80.5
                }
            };

            var circles = new List<DrawCircleInfo>
            {
                new DrawCircleInfo
                {
                    Center = new Point(100, 100),
                    Radius = 30,
                    GrayValue = 120.7
                }
            };

            var actionTypes = new List<DrawActionType>
            {
                DrawActionType.AddCircle,
                DrawActionType.AddRectangle,
                DrawActionType.AddMeasureLine
            };

            // 2. 创建 UserDrawState 对象
            var userDrawState = new UserDrawState(
                drawObjectList,
                measureLines,
                rectangles,
                circles,
                actionTypes
            );

            // 3. 序列化到字节数组
            byte[] serializedData = SerializeToBytes(userDrawState);

            Console.WriteLine("===== 序列化得到的 JSON（字符串查看）=====");
            Console.WriteLine(System.Text.Encoding.UTF8.GetString(serializedData));
            Console.WriteLine();

            // 4. 反序列化回对象
            UserDrawState deserializedState = DeserializeFromBytes(serializedData);

            // 5. 验证结果
            // 因为我们给 DrawObjectList 和 ActionTypes 加了 [JsonIgnore]
            // 它们不会被写入 JSON，也就不会被反序列化回来
            Console.WriteLine("===== 反序列化后的结果 =====");
            Console.WriteLine($"MeasureLines.Count = {deserializedState.MeasureLines.Count}");
            Console.WriteLine($"UserRectangles.Count = {deserializedState.UserRectangles.Count}");
            Console.WriteLine($"UserCircles.Count = {deserializedState.UserCircles.Count}");
            Console.WriteLine($"DrawObjectList.Count = {deserializedState.DrawObjectList.Count} (被忽略，反序列化为 0)");
            Console.WriteLine($"ActionTypes.Count = {deserializedState.ActionTypes.Count} (被忽略，反序列化为 0)");
        }

        public static void testProcessFullPath()
        {
            string[] processNames = { "infer_demo1", "infer_demo2" };

            foreach (string procName in processNames)
            {
                // 通过名称获取进程列表
                Process[] processes = Process.GetProcessesByName(procName);

                if (processes.Length == 0)
                {
                    Console.WriteLine($"进程 {procName} 不存在，请先运行程序。");
                }
                else
                {
                    // 这里仅取第一个同名进程
                    Process proc = processes[0];

                    try
                    {
                        // 获取进程可执行文件完整路径
                        string fullPath = proc.MainModule?.FileName;

                        if (!string.IsNullOrEmpty(fullPath))
                        {
                            // 获取所在目录
                            string directory = Path.GetDirectoryName(fullPath);

                            Console.WriteLine($"{procName} 全路径: {fullPath}");
                            Console.WriteLine($"{procName} 所在目录: {directory}");
                        }
                        else
                        {
                            Console.WriteLine($"无法获取进程 {procName} 的可执行文件全路径。");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"获取进程 {procName} 信息时出错: {ex.Message}");
                    }
                }

                Console.WriteLine(new string('-', 50));
            }

            Console.WriteLine("按任意键退出...");
            Console.ReadKey();
        }



        private void sendButton_Click(object sender, EventArgs e)
        {
            // 获取当前执行的程序集所在的目录
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;

            // 获取目录部分
            string appDirectory = Path.GetDirectoryName(assemblyLocation);

            string testRootDir = Path.Combine(appDirectory, "BrazingParamTest");

            // 生成测试数据
            GenerateTestDirectoriesAndFiles(testRootDir);
            return;
            if (checkBox4.Checked)
            {
                byte[] buffer = HexStringToByteArray(sendTextBox.Text);
                _stream.Write(buffer, 0, buffer.Length);
            }
            else
            {
                byte[] buffer = Encoding.UTF8.GetBytes(sendTextBox.Text);
                _stream.Write(buffer, 0, buffer.Length);
            }
        }

        public static void GenerateTestDirectoriesAndFiles(string rootDir)
        {
            try
            {
                // 清理测试根目录
                if (Directory.Exists(rootDir))
                {
                    Directory.Delete(rootDir, true);
                }

                // 创建日期目录
                string today = DateTime.Now.ToString("yyyyMMdd");
                string dateDir = Path.Combine(rootDir, today);
                Directory.CreateDirectory(dateDir);

                // 创建白班和夜班目录
                string dayShiftDir = Path.Combine(dateDir, "白班");
                string nightShiftDir = Path.Combine(dateDir, "夜班");
                Directory.CreateDirectory(dayShiftDir);
                Directory.CreateDirectory(nightShiftDir);

                // 生成测试文件
                GenerateTestFiles(dayShiftDir, lineNumber: 0, fileCount: 12);
                GenerateTestFiles(nightShiftDir, lineNumber: 0, fileCount: 15);
                GenerateTestFiles(dayShiftDir, lineNumber: 1, fileCount: 15);
                GenerateTestFiles(nightShiftDir, lineNumber: 1, fileCount: 15);

                Console.WriteLine("测试目录和文件生成完成！");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"生成测试目录和文件时发生错误：{ex.Message}");
            }
        }

        private static void GenerateTestFiles(string dir, int lineNumber, int fileCount)
        {
            for (int i = 0; i < fileCount; i++)
            {
                string trayPos = (i + 1).ToString();
                string timestamp = DateTime.Now.AddSeconds(i).ToString("yyyyMMddHHmmss");
                string fileName = $"src_image_{trayPos}_{lineNumber}_{timestamp}.tif";
                string filePath = Path.Combine(dir, fileName);

                // 创建空文件
                File.Create(filePath).Dispose();
            }
        }

        public static void SplitFileName(string fileName, out uint lineNumber, out uint batchNumber, out DateTime time, out string sn1, out string sn2)
        {
            lineNumber = 0;
            batchNumber = 0;
            time = DateTime.MinValue;
            sn1 = string.Empty;
            sn2 = string.Empty;

            fileName = Path.GetFileNameWithoutExtension(fileName);   //去掉后缀
            string[] sp = fileName.Split('_');
            try
            {
                if (sp.Length == 5)
                {
                    sn1 = sp[0];
                    lineNumber = uint.Parse(sp[2]);
                    batchNumber = uint.Parse(sp[3]);
                    TimeHelper.TryParseDateTime(sp[4], out time);
                }
                else if (sp.Length == 6)
                {
                    sn1 = sp[0];
                    sn2 = sp[1];
                    lineNumber = uint.Parse(sp[3]);
                    batchNumber = uint.Parse(sp[4]);
                    TimeHelper.TryParseDateTime(sp[5], out time);
                }
            }
            catch { }
        }

        public static bool ContainsOtherNGStrings(string input)
        {
            // Check if the input string is null or empty
            if (string.IsNullOrEmpty(input)) return false;

            // Trim the input string
            var input2 = input.Trim();

            // Check if the trimmed input string is null or empty
            if (string.IsNullOrEmpty(input2)) return false;

            // Split the input string by commas
            string[] parts = input2.Split(',');

            // Check each part
            foreach (string part in parts)
            {
                // Trim each part to remove any extra whitespace
                var trimmedPart = part.Trim();
                if (trimmedPart != "BB" && trimmedPart != "BO")
                {
                    return true;
                }
            }

            return false;
        }

        public static byte[] HexStringToByteArray(string hexString)
        {
            // 去除字符串中的连字符
            hexString = hexString.Replace("-", string.Empty);

            if (hexString.Length % 2 != 0)
                throw new ArgumentException("Hex string must have an even length");

            byte[] byteArray = new byte[hexString.Length / 2];

            for (int i = 0; i < hexString.Length; i += 2)
            {
                byteArray[i / 2] = byte.Parse(hexString.Substring(i, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return byteArray;
        }


        private static PlcFeedback ProcessUpdatePlcData(List<byte> _plcMessageBuffer, byte[] bytes)
        {
            // 将新接收的数据添加到缓冲区
            _plcMessageBuffer.AddRange(bytes);

            while (true)
            {
                // 将缓冲区数据转换为字符串以搜索消息开始和结束标记
                var bufferString = Encoding.UTF8.GetString(_plcMessageBuffer.ToArray());

                // 检查是否存在至少一个完整的消息
                var start = bufferString.IndexOf('*');
                var end = bufferString.IndexOf('&');

                // 确保开始和结束标志的位置有效
                if (start != -1 && end != -1 && end > start)
                {
                    // 提取消息（不包括开始的'*'和结束的'&'，这取决于你的需求）
                    var message = bufferString.Substring(start + 1, end - start - 1);
                    // 移除已处理的消息部分，包括结束符'&'
                    _plcMessageBuffer.RemoveRange(0, end + 1);
                    // 处理提取的消息
                    return ProcessPCCompleteMessage(message);
                }
                else
                {
                    if (_plcMessageBuffer.Count > 1024 * 4)
                    {
                        _plcMessageBuffer.Clear();
                    }
                    // 如果没有完整的消息，退出循环等待更多数据
                    break;
                }
            }
            return null;
        }

        // 处理PLC完整消息，解析成PlcMessage对象
        private static PlcFeedback ProcessPCCompleteMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return null;
            var plcFeedback = new PlcFeedback();
            var fields = message.Split('#').Where(f => !string.IsNullOrEmpty(f));

            foreach (var field in fields)
            {
                try
                {
                    var keyValue = field.Split(':');
                    if (keyValue.Length != 2) continue; // 跳过不符合键值对格式的部分

                    switch (keyValue[0].Trim())
                    {
                        case "module_number":
                            plcFeedback.ModuleNumber = uint.Parse(keyValue[1].Trim());
                            break;
                        case "result":
                            plcFeedback.Result = ushort.Parse(keyValue[1].Trim());
                            break;
                        case "pos":
                            plcFeedback.Pos = ushort.Parse(keyValue[1].Trim());
                            break;
                        case "batch_number":
                            plcFeedback.BatchNumber = uint.Parse(keyValue[1].Trim());
                            break;
                        case "number1":
                            {
                                var value = keyValue[1].Trim();
                                if (string.IsNullOrEmpty(value))
                                {
                                    plcFeedback.Number1 = null;
                                }
                                else
                                {
                                    plcFeedback.Number1 = ushort.Parse(keyValue[1].Trim());
                                }
                            }
                            break;
                        case "number2":
                            {
                                var value = keyValue[1].Trim();
                                if (string.IsNullOrEmpty(value))
                                {
                                    plcFeedback.Number2 = null;
                                }
                                else
                                {
                                    plcFeedback.Number2 = ushort.Parse(keyValue[1].Trim());
                                }
                            }
                            break;
                        case "number3":
                            {
                                var value = keyValue[1].Trim();
                                if (string.IsNullOrEmpty(value))
                                {
                                    plcFeedback.Number3 = null;
                                }
                                else
                                {
                                    plcFeedback.Number3 = ushort.Parse(keyValue[1].Trim());
                                }
                            }
                            break;
                        case "number4":
                            {
                                var value = keyValue[1].Trim();
                                if (string.IsNullOrEmpty(value))
                                {
                                    plcFeedback.Number4 = null;
                                }
                                else
                                {
                                    plcFeedback.Number4 = ushort.Parse(keyValue[1].Trim());
                                }
                            }
                            break;
                        case "sn1":
                            plcFeedback.Sn1 = keyValue[1].Trim();
                            break;
                        case "sn2":
                            plcFeedback.Sn2 = keyValue[1].Trim();
                            break;
                        case "sn3":
                            plcFeedback.Sn3 = keyValue[1].Trim();
                            break;
                        case "sn4":
                            plcFeedback.Sn4 = keyValue[1].Trim();
                            break;
                        case "dt":
                            // Assuming the date-time format is exactly as specified, e.g., "yyyy-MM-dd HH:mm:ss"
                            if (DateTime.TryParseExact(keyValue[1].Trim(), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                            {
                                plcFeedback.Dt = dt;
                            }
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error($"PLCTcp驱动 解析报文异常, Message: {ex.Message}, StackTrace: {ex.StackTrace}，报文：{message}");
                    return null;
                }
            }
            return plcFeedback;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            _position1HasProduct = checkBox1.Checked;
            _position2HasProduct = checkBox2.Checked;
            _position3HasProduct = checkBox3.Checked;
        }
    }

}
