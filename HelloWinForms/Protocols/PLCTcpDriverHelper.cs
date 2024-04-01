using CxWorkStation.Utilities;
using HelloWinForms.Channel;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static HelloWinForms.Channel.TcpClientChannel;

namespace HelloWinForms.Protocols
{
    public static class PLCTcpDriverHelper
    {
        // TCP连接的远程端口
        public const ushort PLCTcpPort = 530;
        // TCP客户端
        private static TcpClientChannel tcpClientChannel = null;

        // 本模块状态变化事件，给其他模块订阅调用
        public static event EventHandler<TcpClientModuleState> ModuleStateChanged;
        public static event EventHandler<PlcMessage> ReceivedMessage;
        private static long receivedTime = 0;

        // 接收数据到独立线程中处理
        private static volatile bool isRunning = false;
        private static readonly object lockUpdateList = new object();
        private static List<byte[]> updateList = new List<byte[]>();
        private static AutoResetEvent updateSignal = new AutoResetEvent(false);

        public static void Start(string ip, ushort port)
        {
            if (tcpClientChannel != null)
            {
                tcpClientChannel.Stop();
            }

            DoRun();
            tcpClientChannel = new TcpClientChannel(ip, port);
            tcpClientChannel.StateChanged += TcClient_StateChanged;
            tcpClientChannel.DataReceived += TcClient_DataReceived;
            tcpClientChannel.Start();
        }

        public static void Stop()
        {
            if (tcpClientChannel != null)
            {
                tcpClientChannel.Stop();
                tcpClientChannel = null;
            }
            isRunning = false;
            // 通知线程
            updateSignal.Set();
            TcClient_StateChanged(TcpClientModuleState.Stopped);
        }

        private static void DoRun()
        {
            //_updateStack = new ConcurrentStack<VariableEntry>();
            isRunning = true;
            new Thread(() =>
            {
                try
                {
                    while (isRunning)
                    {
                        // 等待一小段时间或直到收到更新信号
                        //_updateSignal.WaitOne(); // 等待更新信号
                        updateSignal.WaitOne(TimeSpan.FromMilliseconds(100));

                        List<byte[]> changedEntries = null;
                        lock (lockUpdateList)
                        {
                            if (updateList.Count > 0)
                            {
                                changedEntries = new List<byte[]>(updateList);
                                updateList.Clear();
                            }
                        }
                        if (changedEntries != null && changedEntries.Count > 0)
                        {
                            ProcessUpdates(changedEntries);
                            changedEntries.Clear();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 记录异常
                    LogHelper.Error($"Exception in VariableManager thread, Message: {ex.Message}, StackTrace: {ex.StackTrace}");
                    // 再次抛出
                    throw;
                }

            })
            { IsBackground = true }.Start();
        }

        private static List<byte> messageBuffer = new List<byte>();

        private static void ProcessSingleUpdate(byte[] bytes)
        {
            // 将新接收的数据添加到缓冲区
            messageBuffer.AddRange(bytes);

            while (true)
            {
                // 将缓冲区数据转换为字符串以搜索消息开始和结束标记
                var bufferString = Encoding.UTF8.GetString(messageBuffer.ToArray());

                // 检查是否存在至少一个完整的消息
                var start = bufferString.IndexOf('*');
                var end = bufferString.IndexOf('&');

                // 确保开始和结束标志的位置有效
                if (start != -1 && end != -1 && end > start)
                {
                    // 提取消息（不包括开始的'*'和结束的'&'，这取决于你的需求）
                    var message = bufferString.Substring(start + 1, end - start - 1);
                    // 处理提取的消息
                    var plcMessage = ProcessCompleteMessage(message);
                    ProcessPlcMessage(plcMessage);
                    // 移除已处理的消息部分，包括结束符'&'
                    messageBuffer.RemoveRange(0, end + 1);
                }
                else
                {
                    // 如果没有完整的消息，退出循环等待更多数据
                    break;
                }
            }
        }


        private static void ProcessPlcMessage(PlcMessage plcMessage)
        {
            ReceivedMessage?.Invoke(tcpClientChannel, plcMessage);
        }

        private static PlcMessage ProcessCompleteMessage(string message)
        {
            if (string.IsNullOrEmpty(message)) return null;
            var plcMessage = new PlcMessage();
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
                            plcMessage.ModuleNumber = uint.Parse(keyValue[1]);
                            break;
                        case "batch_number":
                            plcMessage.BatchNumber = uint.Parse(keyValue[1]);
                            break;
                        case "cmd":
                            plcMessage.Cmd = ushort.Parse(keyValue[1]);
                            break;
                        case "pos":
                            plcMessage.Pos = ushort.Parse(keyValue[1]);
                            break;
                        case "sn1":
                            plcMessage.Sn1 = keyValue[1].Trim();
                            break;
                        case "sn2":
                            plcMessage.Sn2 = keyValue[1].Trim();
                            break;
                        case "sn3":
                            plcMessage.Sn3 = keyValue[1].Trim();
                            break;
                        case "sn4":
                            plcMessage.Sn4 = keyValue[1].Trim();
                            break;
                        case "dt":
                            // Assuming the date-time format is exactly as specified, e.g., "yyyy-MM-dd HH:mm:ss"
                            if (DateTime.TryParseExact(keyValue[1].Trim(), "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out DateTime dt))
                            {
                                plcMessage.Dt = dt;
                            }
                            break;
                        default:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    LogHelper.Error($"PLCTcp驱动 解析报文异常, Message: {ex.Message}, StackTrace: {ex.StackTrace}，报文：{message}");
                }
            }
            return plcMessage;
        }
        private static void ProcessUpdates(List<byte[]> changedEntries)
        {
            if (TimeHelper.GetNow() - receivedTime > 3000)
            {
                messageBuffer.Clear();
            }
            foreach (var bytes in changedEntries)
            {
                ProcessSingleUpdate(bytes);
            }
            receivedTime = TimeHelper.GetNow();
        }

        public static bool IsConnected()
        {
            return tcpClientChannel != null && tcpClientChannel.IsConnected();
        }

        public static TcpClientModuleState ModuleState
        {
            get
            {
                if (tcpClientChannel == null)
                {
                    return tcpClientChannel.ModuleState;
                }
                else
                {
                    return TcpClientModuleState.Stopped;
                }
            }
        }

        private static void TcClient_DataReceived(byte[] buffer, int bytesRead)
        {
            // 在这里处理数据
            // 注意: 由于data是从外部传入的，建议在这里进行深复制以避免潜在的并发问题
            var dataCopy = new byte[bytesRead];
            Array.Copy(buffer, dataCopy, bytesRead);
            lock (lockUpdateList)
            {
                updateList.Add(dataCopy);
            }
            updateSignal.Set(); // 触发信号通知更新
        }

        private static void TcClient_StateChanged(TcpClientChannel.TcpClientModuleState state)
        {
            ModuleStateChanged?.Invoke(tcpClientChannel, state);
        }

    }


}
