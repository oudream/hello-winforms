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
using DevExpress.Xpo.DB;
using System.Reflection.Emit;
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

        private TcpListener _listener;
        private volatile bool _isRunning;
        private readonly List<Thread> _clientThreads = new List<Thread>();
        private ConcurrentDictionary<string, TcpClient> _connectedClients = new ConcurrentDictionary<string, TcpClient>();
        private Random _random = new Random();
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
                                if (plcMessages.Count > 0)
                                {
                                    if (plcFeedback.Pos == POS_QRCodeAcquisition)
                                    {
                                        var qrCodes = plcFeedback.Sn1 + ";" + plcFeedback.Sn2 + ";" + plcFeedback.Sn3;
                                        uint fromBatchNumber = plcFeedback.BatchNumber;
                                        uint sendBatchNumber = 0;
                                        foreach (var plcMessage in plcMessages)
                                        {
                                            if (plcMessage.BatchNumber == plcFeedback.BatchNumber)
                                            {
                                                plcMessage.ReceivedQRCodeTime = TimeHelper.GetNow();
                                                plcMessage.ReceivedQRCodes = qrCodes;
                                                sendBatchNumber = plcMessage.BatchNumber;
                                                break;
                                            }
                                        }
                                        LogHelper.Debug($"received 读码:{qrCodes} fromBatchNumber:{fromBatchNumber} sendBatchNumber:{sendBatchNumber}");
                                    }
                                    else if (plcFeedback.Pos == POS_ImageAcquisition)
                                    {
                                        uint fromBatchNumber = plcFeedback.BatchNumber;
                                        uint sendBatchNumber = 0;
                                        foreach (var plcMessage in plcMessages)
                                        {
                                            if (plcMessage.BatchNumber == plcFeedback.BatchNumber)
                                            {
                                                plcMessage.ReceivedImageTime = TimeHelper.GetNow();
                                                sendBatchNumber = plcMessage.BatchNumber;
                                                break;
                                            }
                                        }
                                        LogHelper.Debug($"received 采图 fromBatchNumber:{fromBatchNumber} sendBatchNumber:{sendBatchNumber}");
                                    }
                                    else if (plcFeedback.Pos == POS_Result)
                                    {
                                        var result = plcFeedback.Number1 + ";" + plcFeedback.Number2 + ";" + plcFeedback.Number3;
                                        uint fromBatchNumber = plcFeedback.BatchNumber;
                                        uint sendBatchNumber = 0;
                                        foreach (var plcMessage in plcMessages)
                                        {
                                            if (plcMessage.BatchNumber == plcFeedback.BatchNumber)
                                            {
                                                plcMessage.ReceivedResultTime = TimeHelper.GetNow();
                                                plcMessage.ReceivedResult = result;
                                                sendBatchNumber = plcMessage.BatchNumber;
                                                break;
                                            }
                                        }
                                        LogHelper.Debug($"received 结果:{result} fromBatchNumber:{fromBatchNumber} sendBatchNumber:{sendBatchNumber}");
                                    }
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

                    bool hSend = false;

                    // 请求读码
                    // 每 1.5 秒发送一个产品检测
                    // 7 个为一个大批
                    if (!hSend && plcMessages.Count < 7)
                    {
                        // 检查是否需要发送新的消息
                        if (nowMs - lastSendTime >= 1500) 
                        {
                            lastSendTime = nowMs;
                            PlcMessage plcMessage = new PlcMessage
                            {
                                BatchNumber = _batchNumber++,
                                Cmd = CMD_PC,
                                Dt = dtNow,
                                Sn1 = _position1HasProduct ? "1" : "0",
                                Sn2 = _position2HasProduct ? "1" : "0",
                                Sn3 = _position3HasProduct ? "1" : "0",
                            };
                            plcMessage.Pos = POS_QRCodeAcquisition;
                            plcMessage.SendImageAcquisitionTime = 0;
                            plcMessage.ReceivedQRCodeTime = 0;
                            plcMessage.ReceivedImageTime = 0;
                            plcMessage.SendResultTime = 0;
                            plcMessage.ReceivedResultTime = 0;

                            SendPlcMessage(stream, plcMessage);
                            hSend = true;

                            plcMessages.Add(plcMessage);
                        }
                    }

                    // 请求采图
                    if (!hSend && plcMessages.Count > 0)
                    {
                        foreach (var plcMessage in plcMessages)
                        {
                            if (plcMessage.SendImageAcquisitionTime <= 0)
                            {
                                plcMessage.Pos = POS_ImageAcquisition;
                                SendPlcMessage(stream, plcMessage);
                                plcMessage.SendImageAcquisitionTime = nowMs;
                                hSend = true;
                                break;
                            }
                        }
                    }

                    // 请求结果
                    // 如果没有收齐结果，就一直请求结果
                    if (!hSend && plcMessages.Count >= 7)
                    {
                        foreach (var plcMessage in plcMessages)
                        {
                            if (nowMs - plcMessage.SendImageAcquisitionTime > 3000 && plcMessage.ReceivedResultTime <= 0)
                            {
                                if (nowMs - plcMessage.SendResultTime < 1000)
                                {
                                    plcMessage.Pos = POS_Result;
                                    plcMessage.SendResultTime = nowMs;
                                    SendPlcMessage(stream, plcMessage);
                                    hSend = true;
                                    break;
                                }
                            }
                        }
                    }

                    // 清除 7 个为一个大批
                    if (!hSend && plcMessages.Count >= 7)
                    {
                        bool allResultValid = true;
                        foreach (var plcMessage in plcMessages)
                        {
                            if (plcMessage.ReceivedResultTime <= 0)
                            {
                                allResultValid = false;
                                break;
                            }
                        }
                        if (allResultValid)
                        {
                            LogHelper.Debug($"清除大批次 共 {plcMessages.Count} 个检测");
                            plcMessages.Clear();
                            lastSendTime = nowMs;
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

        private void sendButton_Click(object sender, EventArgs e)
        {
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
