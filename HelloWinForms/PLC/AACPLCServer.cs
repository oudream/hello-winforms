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
using System.Runtime.InteropServices.ComTypes;
using System.Globalization;
using HelloWinForms.Channel;
using DevExpress.Internal.WinApi.Windows.UI.Notifications;

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

        private void timer1_Tick(object sender, EventArgs e)
        {
            Stop();
            Thread.Sleep(30);
            Start();
        }

        public static string Product_v54 = "models_v54";

        private void sendButton_Click(object sender, EventArgs e)
        {
            string s = "v54";
            if (Product_v54.Contains(s))
            {
                Console.WriteLine(s);
            }
            string input = "BO,XXX";
            if (ContainsOtherNGStrings(input))
            {
                Console.WriteLine("The string contains invalid substrings.");
            }
            else
            {
                Console.WriteLine("The string only contains 'BB' and 'BO'.");
            }
            input = "BO,BB";
            if (ContainsOtherNGStrings(input))
            {
                Console.WriteLine("The string contains invalid substrings.");
            }
            else
            {
                Console.WriteLine("The string only contains 'BB' and 'BO'.");
            }
            input = "BO";
            if (ContainsOtherNGStrings(input))
            {
                Console.WriteLine("The string contains invalid substrings.");
            }
            else
            {
                Console.WriteLine("The string only contains 'BB' and 'BO'.");
            }
            input = "Fail";
            if (ContainsOtherNGStrings(input))
            {
                Console.WriteLine("The string contains invalid substrings.");
            }
            else
            {
                Console.WriteLine("The string only contains 'BB' and 'BO'.");
            }
            input = "";
            if (ContainsOtherNGStrings(input))
            {
                Console.WriteLine("The string contains invalid substrings.");
            }
            else
            {
                Console.WriteLine("The string only contains 'BB' and 'BO'.");
            }
            //if (_stream == null) return;
            //var dtNow = DateTime.Now;
            //var nowMs = TimeHelper.GetMs(dtNow);

            //var plcMessage = new PlcMessage
            //{
            //    BatchNumber = _batchNumber++,
            //    Cmd = CMD_PC,
            //    Dt = dtNow,
            //    Sn1 = _position1HasProduct ? "1" : "0",
            //    Sn2 = _position2HasProduct ? "1" : "0",
            //    Sn3 = _position3HasProduct ? "1" : "0",
            //};

            //plcMessage.SendImageAcquisitionTime = 0;
            //plcMessage.ReceivedQRCodeTime = 0;
            //plcMessage.ReceivedImageTime = 0;
            //plcMessage.SendResultTime = 0;
            //plcMessage.ReceivedResultTime = 0;

            //// 请求读码
            //plcMessage.Pos = POS_QRCodeAcquisition;
            //SendPlcMessage(_stream, plcMessage);

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
