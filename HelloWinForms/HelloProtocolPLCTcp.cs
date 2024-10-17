using CxWorkStation.Utilities;
using HelloWinForms.Protocols;
using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloProtocolPLCTcp : Form
    {

        public HelloProtocolPLCTcp()
        {
            InitializeComponent();
        }


        public const int PLCMaxPacketSize = 4 * 1024;
        public const int PLCMaxPacketCacheSize = PLCMaxPacketSize * 4;

        private static byte[] _plcMessageBuffer = new byte[PLCMaxPacketCacheSize * 2];
        private static int _plcMessageBufferLength = 0;
        private static long _plcMessageBufferReceiveTime = 0;

        private static void ProcessUpdatePlcData(byte[] bytes)
        {
            // 1. 防止接收报文时间过长，影响后面通信
            if (bytes.Length > _plcMessageBuffer.Length)
            {
                _plcMessageBufferLength = 0;
                Array.Clear(_plcMessageBuffer, 0, _plcMessageBuffer.Length);
                LogHelper.Error($"PLCTcp驱动 接收数据1[{bytes.Length}]超过缓冲区大小，清空缓冲区，并不处理数据");
                return;
            }

            // 2. 防止剩下未有解析的报文与新接收的报文过长，会抛弃旧报文
            if (_plcMessageBufferLength + bytes.Length > _plcMessageBuffer.Length)
            {
                // 如果超出，清空缓冲区或采取其他适当措施
                _plcMessageBufferLength = 0;
                Array.Clear(_plcMessageBuffer, 0, _plcMessageBuffer.Length);
                LogHelper.Error($"PLCTcp驱动 接收数据2[{_plcMessageBufferLength + bytes.Length}]超过剩下缓冲区大小，清空旧缓冲区");
            }

            //var recvBufString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            //LogHelper.Debug($"PLCTcp驱动 +++ 接收数据[{bytes.Length}]，内容为:{recvBufString}");

            // 将新接收的数据添加到缓冲区
            Array.Copy(bytes, 0, _plcMessageBuffer, _plcMessageBufferLength, bytes.Length);
            _plcMessageBufferLength += bytes.Length;

            // 3. 一直检查缓冲区，直到找到正确的报文，然后移走正确报文的部分
            //    _plcMessageBufferLength 是剩下没有解析的报文长度
            while (true)
            {
                // 查找消息的开始标记
                int start = Array.IndexOf(_plcMessageBuffer, (byte)'*', 0, _plcMessageBufferLength);

                if (start != -1)
                {
                    // 在找到的开始标记之后查找结束标记
                    int end = Array.IndexOf(_plcMessageBuffer, (byte)'&', start + 1, _plcMessageBufferLength - start - 1);

                    // 确保开始和结束标志的位置有效
                    if (end != -1 && end > start)
                    {
                        // 提取消息（不包括开始的'*'和结束的'&'，这取决于你的需求）
                        int messageLength = end - start - 1;
                        string message = Encoding.UTF8.GetString(_plcMessageBuffer, start + 1, messageLength);

                        //处理提取的消息
                       var plcMessage = ParsePlcCompleteMessage(message);
                        if (plcMessage != null)
                        {
                            ProcessPlcMessage(plcMessage);
                        }

                        // 移除已处理的消息部分
                        int processedLength = end + 1;
                        _plcMessageBufferLength -= processedLength;
                        Array.Copy(_plcMessageBuffer, processedLength, _plcMessageBuffer, 0, _plcMessageBufferLength);
                    }
                    else
                    {
                        // 如果没有找到结束标记，但缓冲区长度超过最大报文大小，则清空缓冲区
                        if (_plcMessageBufferLength > PLCMaxPacketSize)
                        {
                            _plcMessageBufferLength = 0;
                            Array.Clear(_plcMessageBuffer, 0, _plcMessageBuffer.Length);
                            LogHelper.Error($"PLCTcp驱动 接收数据3[{_plcMessageBufferLength}]超过剩下缓冲区大小，清空旧缓冲区");
                        }
                        // 如果没有找到完整的消息，退出循环等待更多数据
                        break;
                    }
                }
                else
                {
                    // 如果没有找到开始标记，但缓冲区长度超过最大报文大小，则清空缓冲区
                    if (_plcMessageBufferLength > PLCMaxPacketSize)
                    {
                        _plcMessageBufferLength = 0;
                        Array.Clear(_plcMessageBuffer, 0, _plcMessageBuffer.Length);
                        LogHelper.Error($"PLCTcp驱动 接收数据3[{_plcMessageBufferLength}]超过剩下缓冲区大小，清空旧缓冲区");
                    }
                    // 如果没有找到开始标记，退出循环等待更多数据
                    break;
                }
            }
        }

        // 处理PLC完整消息，解析成PlcMessage对象
        private static PlcMessage ParsePlcCompleteMessage(string message)
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
                            plcMessage.ModuleNumber = uint.Parse(keyValue[1].Trim());
                            break;
                        case "batch_number":
                            plcMessage.BatchNumber = uint.Parse(keyValue[1].Trim());
                            break;
                        case "cmd":
                            plcMessage.Cmd = ushort.Parse(keyValue[1].Trim());
                            break;
                        case "pos":
                            plcMessage.Pos = ushort.Parse(keyValue[1].Trim());
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
                    return null;
                }
            }
            return plcMessage;
        }

        private static void ProcessPlcMessage(PlcMessage plcMessage)
        {
            Console.WriteLine($"Recv：{plcMessage.ToMessage()}");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string[] messages = {
            "*#module_number:1#batch_number:1234567892#cmd:11#pos:11#sn1:1#sn2:123456789qwe#sn3:1#sn3:1qwe#dt:2023-03-23 12:13:19&",
            "*#module_number:101#result:101#pos:101#batch_number:1234567892#number1:1#number2:2#number3:3#number4:4#sn1:123456789qwe#sn2:123456789qwe#sn3:123456789qwe#sn4:123456789qwe#dt:2023-03-23 12:13:19&"
        };

            List<byte> combinedMessages = new List<byte>();
            Random random = new Random();

            foreach (var message in messages)
            {
                byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                int currentIndex = 0;

                while (currentIndex < messageBytes.Length)
                {
                    int chunkSize = random.Next(1, 10); // 随机分段大小
                    int remaining = messageBytes.Length - currentIndex;
                    chunkSize = Math.Min(chunkSize, remaining);

                    byte[] chunk = new byte[chunkSize];
                    Array.Copy(messageBytes, currentIndex, chunk, 0, chunkSize);
                    currentIndex += chunkSize;

                    ProcessUpdatePlcData(chunk);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var message = "mds 1234 *#module_number:1#batch_number:1234567892#cmd:11#pos:11#sn1:1#sn2:123456789qwe#sn3:1#sn3:1qwe#dt:2023-03-23 12:13:19& 234141 234sdxc" +
                             "dsfja s;fh f *#module_number:101#result:101#pos:101#batch_number:1234567892#number1:1#number2:2#number3:3#number4:4#sn1:123456789qwe#sn2:123456789qwe#sn3:123456789qwe#sn4:123456789qwe#dt:2023-03-23 12:13:19&fdsfa fdfafd23" +
                             "mds 1234 *#module_number:3#batch_number:123456892#cmd:11#pos:11#sn1:tqt1#sn2:123456789qwe#sn3:1#sn3:1qwe#dt:2023-03-23 12:13:19& 234141 234sdxc";
            Random random = new Random();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            int currentIndex = 0;
            int count = 0;
            while (currentIndex < messageBytes.Length)
            {
                int chunkSize = random.Next(1, 10); // 随机分段大小
                int remaining = messageBytes.Length - currentIndex;
                chunkSize = Math.Min(chunkSize, remaining);

                byte[] chunk = new byte[chunkSize];
                Array.Copy(messageBytes, currentIndex, chunk, 0, chunkSize);
                currentIndex += chunkSize;

                ProcessUpdatePlcData(chunk);
                count++;
            }
            Console.WriteLine($"发送了 {count} 次");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            List<BubbleDetectionResultDetail> BubbleDetections = new List<BubbleDetectionResultDetail>
        {
            new BubbleDetectionResultDetail { AreaID = 1, BubbleRatio = 0.5, Area = 100, Height = 10, Width = 10 },
            new BubbleDetectionResultDetail { AreaID = 2, BubbleRatio = 0.22, Area = 80, Height = 8, Width = 10 },
            new BubbleDetectionResultDetail { AreaID = 3, BubbleRatio = 0.81, Area = 120, Height = 12, Width = 10 },
            new BubbleDetectionResultDetail { AreaID = 4, BubbleRatio = 11, Area = 60, Height = 6, Width = 10 }
        };
            var bubbleDetectRatios = BubbleDetections.OrderBy(d => d.BubbleRatio);
            var maxBubbleRatioDetail = bubbleDetectRatios.Last();
            var minBubbleRatioDetail = bubbleDetectRatios.First();
            var averageBubbleRatio = BubbleDetections.Average(d => d.BubbleRatio);

            Console.WriteLine($"Max BubbleRatio: {maxBubbleRatioDetail.BubbleRatio:0.###}, AreaID: {maxBubbleRatioDetail.AreaID}");
            Console.WriteLine($"Min BubbleRatio: {minBubbleRatioDetail.BubbleRatio:0.###}, AreaID: {minBubbleRatioDetail.AreaID}");
            Console.WriteLine($"Average BubbleRatio: {averageBubbleRatio:0.###}");

            var sortedDetections = BubbleDetections.OrderBy(d => d.AreaID).ToList();

            Console.WriteLine("Sorted BubbleDetections by AreaID:");
            foreach (var detection in sortedDetections)
            {
                Console.WriteLine(detection.ToLine());
            }
        }
    }

    public class BubbleDetectionResultDetail
    {
        public int AreaID { get; set; }
        public double BubbleRatio { get; set; }
        public double Area { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }
        public string ToLine() { return $"B:{AreaID},{BubbleRatio},{Area},{Height},{Width}"; }
        public static string Format(int areaID, double bubbleRatio, double area, double height, double width)
        {
            return $"B:{areaID},{bubbleRatio},{area},{height},{width}";
        }
        public static BubbleDetectionResultDetail Parse(string line)
        {
            var values = line.Split(',');
            return new BubbleDetectionResultDetail
            {
                AreaID = int.Parse(values[0]),
                BubbleRatio = double.Parse(values[1]),
                Area = double.Parse(values[2]),
                Height = double.Parse(values[3]),
                Width = double.Parse(values[4])
            };
        }
    }
}
