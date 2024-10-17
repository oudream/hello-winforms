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
using System.Web.UI.WebControls.WebParts;
using CxWorkStation.Utilities;
using OpenCvSharp.Flann;
using HelloWinForms.Utilities;
using HelloWinForms.Protocols;
using CommonInterfaces;

namespace HelloWinForms.PLC
{
    public partial class SolderBallPLCServer : Form
    {
        private static readonly string[] SerialNumbers = new string[]
        {
            "SN123456789012345678",
            "SN345678901234567890",
            "SN567890123456789012",
            "SN789012345678901234",
            "SN789012345678901234",
            "SN567890123456789012",
            "SN345678901234567890",
            "SN123456789012345678",
        };

        private static readonly ushort[] Poses = new ushort[]
        {
            11,
            12,
            13,
            14,
            24,
            23,
            22,
            21,
        };

        private TcpListener _listener;
        private volatile bool _isRunning;
        private readonly List<Thread> _clientThreads = new List<Thread>();
        private ConcurrentDictionary<string, TcpClient> _connectedClients = new ConcurrentDictionary<string, TcpClient>();
        private Random _random = new Random();

        private string _batchNumberFilePath;
        static uint _batchNumber = 1;

        public SolderBallPLCServer()
        {
            InitializeComponent();

            _batchNumberFilePath = AppHelper.JoinFullPath("BatchNumber.ini");
            _batchNumber = ReadIntegerFromFile(_batchNumberFilePath)+1;

            this.Text = $"SolderBallPLCServer - {_batchNumber}";
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
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

        private int _sampleBatchCounter;
        private uint _currentBatchNumber;
        private uint CurrentBatchNumber 
        { 
            get
            {
                return _currentBatchNumber;
            }
            set
            {
                //LogHelper.Warning($"_currentBatchNumber:{_currentBatchNumber}] value:{value}");
                //OutputMessage($"_currentBatchNumber:{_currentBatchNumber}] value:{value}");

                if (_currentBatchNumber != value)
                {
                    if (_sampleBatchCounter != 8)
                    {
                        LogHelper.Warning($"Sample batch[{_currentBatchNumber}] counter is {_sampleBatchCounter}");
                        OutputMessage($"Sample batch[{_currentBatchNumber}] counter is {_sampleBatchCounter}");
                    }
                    _sampleBatchCounter = 0;
                    _currentBatchNumber = value;
                    if (value % 10 == 0)
                    {
                        LogHelper.Warning($"Running Outinfo CurrentBatch[{_currentBatchNumber}] counter is {_sampleBatchCounter}");
                        OutputMessage($"Running Outinfo CurrentBatch[{_currentBatchNumber}] counter is {_sampleBatchCounter}");
                    }
                }
                else
                {
                    _sampleBatchCounter++;
                }
            }
        }



        private void HandleClient(TcpClient client)
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

                long lastSendTime = 0;
                int sendIndex = 0;
                int snIndex = 0;

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
                                CurrentBatchNumber = plcFeedback.BatchNumber;
                                //OutputMessage($"{DateTime.Now:dd/HH:mm:ss.fff} received: {plcFeedback}");
                                LogHelper.Debug($"{DateTime.Now:dd/HH:mm:ss.fff} received: {plcFeedback}");
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

                    // 检查是否需要发送新的消息
                    var dtNow = TimeHelper.GetNow();
                    if (dtNow - lastSendTime >= 1500) // 每秒发送一次状态更新
                    {
                        lastSendTime = dtNow;
                        SendPlcMessage(stream, ref sendIndex, ref snIndex, ref _batchNumber);
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

        private void SendPlcMessage(NetworkStream stream, ref int sendIndex, ref int snIndex, ref uint batchNumber)
        {
            PlcMessage plcMessage = new PlcMessage
            {
                ModuleNumber = 1,
                BatchNumber = batchNumber,
                Dt = DateTime.Now,
            };
            if (sendIndex < 8) // 模拟发送拍照请求
            {
                plcMessage.Cmd = 11; // 11=1号光源请求取图；
                //
                ushort position = Poses[sendIndex];
                string sn = SerialNumbers[sendIndex];
                plcMessage.Pos = position;
                //
                switch (position)
                {
                    case 11:
                    case 21:
                        plcMessage.Sn1 = sn;
                        break;
                    case 12:
                    case 22:
                        plcMessage.Sn2 = sn;
                        break;
                    case 13:
                    case 23:
                        plcMessage.Sn3 = sn;
                        break;
                    case 14:
                    case 24:
                        plcMessage.Sn4 = sn;
                        break;
                    default:
                        break;
                }
                sendIndex++;
            }
            else // 模拟请求最终结果
            {
                plcMessage.Cmd = 100; // 100 = 请求最终结果；
                sendIndex++;
                if (sendIndex == 10)
                {
                    sendIndex = 0;
                    batchNumber++;
                }
            }
            string message = plcMessage.ToMessage();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            stream.Write(messageBytes, 0, messageBytes.Length);
            LogHelper.Debug($"{DateTime.Now:dd/HH:mm:ss.fff} Sent: Cmd:{plcMessage.Cmd} BatchNumber:{plcMessage.BatchNumber} Pos:{plcMessage.Pos} Sn1:{plcMessage.Sn1} Sn2:{plcMessage.Sn2} Sn3:{plcMessage.Sn3} Sn4:{plcMessage.Sn4}");
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            Stop();
            Thread.Sleep(30);
            Start();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            for (int i = 10; i < 16; i++)
            {
                var (pos, orientation) = ConvertToImageDetection((ushort)i);
                Console.WriteLine($"Send: {pos} {orientation}");
            }
            for (int i = 20; i <= 26; i++)
            {
                var (pos, orientation) = ConvertToImageDetection((ushort)i);
                Console.WriteLine($"Send: {pos} {orientation}");
            }
        }

        // 位置数据转换
        // 11=1号产品正面取图反馈；12=2号产品正面取图反馈；13=3号产品正面取图反馈；14=4号产品正面取图反馈；
        // 21=1号产品反面取图反馈；22=2号产品反面取图反馈；23=3号产品反面取图反馈；24=4号产品反面取图反馈；
        // 位置数据转换为图像检测任务：11=1号产品正面，12=2号产品正面，13=3号产品正面，14=4号产品正面，
        //                             21=1号产品反面，22=2号产品反面，23=3号产品反面，24=4号产品反面
        public static (int pos, Orientation orientation) ConvertToImageDetection(ushort position)
        {
            if (position < 11 || position > 24) Console.WriteLine($"Invalid Positon: {position}");
            if (position > 14 && position < 21) Console.WriteLine($"Invalid Positon: {position}");

            int productNumber = position % 10;

            Orientation orientation = (position / 10 == 1) ? Orientation.Front : Orientation.Side;

            return (productNumber, orientation);
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        // 写入整数到文件的方法
        static void WriteIntegerToFile(string filePath, uint number)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(number);
            }
        }

        // 从文件加载整数的方法
        static uint ReadIntegerFromFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return 1; // 文件不存在时返回默认值 1
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line = reader.ReadLine();
                if (uint.TryParse(line, out uint number))
                {
                    return number;
                }
                else
                {
                    return 1;
                }
            }
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

        private void SolderBallPLCServer_FormClosing(object sender, FormClosingEventArgs e)
        {
            WriteIntegerToFile(_batchNumberFilePath, _batchNumber);
        }
    }

   

}
