using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloBuffer : Form
    {
        CircularIndicator lightSourceStatusControl;
        CircularIndicator detectorStatusControl;
        CircularIndicator visionServerStatusControl;
        CircularIndicator s7DriverStatusControl;

        public HelloBuffer()
        {
            InitializeComponent();

            InitializeStatusIndicator();

            initStatuControls();
        }

        private void initStatuControls()
        {
            lightSourceStatusControl = new CircularIndicator
            {
                Size = new Size(this.panel8.Width, this.panel8.Height), // 设置控件大小
                Location = new System.Drawing.Point(0, 0), // 设置控件位置
                IndicatorLabel = this.label3,
            };
            this.panel8.Controls.Add(lightSourceStatusControl);

            detectorStatusControl = new CircularIndicator
            {
                Size = new Size(this.panel10.Width, this.panel10.Height), // 设置控件大小
                Location = new System.Drawing.Point(0, 0), // 设置控件位置
                IndicatorLabel = this.label4,
            };
            this.panel10.Controls.Add(detectorStatusControl);

            visionServerStatusControl = new CircularIndicator
            {
                Size = new Size(this.panel12.Width, this.panel12.Height), // 设置控件大小
                Location = new System.Drawing.Point(0, 0), // 设置控件位置
                IndicatorLabel = this.label5,
            };
            this.panel12.Controls.Add(visionServerStatusControl);

            s7DriverStatusControl = new CircularIndicator
            {
                Size = new Size(this.panel14.Width, this.panel14.Height), // 设置控件大小
                Location = new System.Drawing.Point(0, 0), // 设置控件位置
                IndicatorLabel = this.label6,
            };
            this.panel14.Controls.Add(s7DriverStatusControl);
        }

        // Span适用于需要高性能内存操作的场景，例如大数据处理、字符串操作等。
        // 在适用场景下使用Span可以避免不必要的内存分配和数据拷贝。
        static void SpanExampleBigString()
        {
            // 模拟大量的日志数据
            string[] logData = new string[1000000];
            for (int i = 0; i < logData.Length; i++)
            {
                logData[i] = $"Log entry {i + 1}";
            }

            // 将日志数据转换为字节数组
            //byte[] logBytes = new byte[logData.Length * sizeof(char)]; // 这行代码会导致内存分配不足
            // 预计算所需的字节总数
            int totalBytes = logData.Sum(item => Encoding.Unicode.GetByteCount(item));
            byte[] logBytes = new byte[totalBytes];
            //for (int i = 0; i < logData.Length; i++)
            //{
            //    Encoding.Unicode.GetBytes(logData[i], 0, logData[i].Length, logBytes, i * sizeof(char));
            //}
            // 将日志数据转换为字节数组
            int byteIndex = 0;
            foreach (var item in logData)
            {
                int byteCount = Encoding.Unicode.GetByteCount(item);
                Encoding.Unicode.GetBytes(item, 0, item.Length, logBytes, byteIndex);
                byteIndex += byteCount;
            }

            // 使用Span处理日志数据
            Span<byte> logSpan = new Span<byte>(logBytes);

            // 在Span中查找特定关键词
            string keyword = "error";
            byte[] keywordBytes = Encoding.Unicode.GetBytes(keyword);
            int keywordCount = 0;

            for (int i = 0; i < logSpan.Length - keywordBytes.Length; i += sizeof(char))
            {
                if (logSpan.Slice(i, keywordBytes.Length).SequenceEqual(keywordBytes))
                {
                    keywordCount++;
                }
            }

            Console.WriteLine($"关键词 '{keyword}' 出现次数：{keywordCount}");
        }

        static void FindKeywordInLogs()
        {
            // 模拟大量的日志数据
            string[] logData = new string[1000000];
            for (int i = 0; i < logData.Length; i++)
            {
                logData[i] = $"Log entry {i + 1}";
            }

            // 定义要查找的关键词
            string keyword = "error";
            int keywordCount = 0;

            // 遍历日志数据，查找关键词
            foreach (string logEntry in logData)
            {
                bool contains = logEntry.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
                // 使用Contains方法查找关键词
                if (contains)
                {
                    keywordCount++;
                }
            }

            Console.WriteLine($"关键词 '{keyword}' 出现次数：{keywordCount}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            byte[] bytes = { 1, 2, 3, 4, 5 };
            ArraySegment<byte> segment = new ArraySegment<byte>(bytes, 1, 3); // 从索引1开始，长度为3的段

            // 使用segment
            for (int i = segment.Offset; i < segment.Offset + segment.Count; i++)
            {
                ShowInfo($"{bytes[i]}");
            }
        }

        private void HelloBuffer_Load(object sender, EventArgs e)
        {


        }

        private void button2_Click(object sender, EventArgs e)
        {
            FindKeywordInLogs();
            //SpanExampleBigString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

            // 创建一个Span<byte>，它表示data数组中从索引2开始的5个元素。
            Span<byte> slice = new Span<byte>(data, 2, 5);

            // 修改slice中的数据将直接影响原始数组。
            slice[0] = 10; // 现在，data[2]的值将变为10。

            // 打印修改后的原始数组
            ShowInfo(string.Join(", ", data)); // 输出: 0, 1, 10, 3, 4, 5, 6, 7, 8, 9
        }



        private void ShowInfo(string info)
        {
            this.richTextBox1.AppendText(info + "\r\n");
        }

        private bool _status = true;
        private void button4_Click(object sender, EventArgs e)
        {
            _status = !_status;
            lightSourceStatusControl.UpdateStatus(_status);
            detectorStatusControl.UpdateStatus(_status);
            visionServerStatusControl.UpdateStatus(_status);
            s7DriverStatusControl.UpdateStatus(_status);
            //UpdateStatus(_status);
        }

        void EditList(byte[] a)
        {
            Array.Reverse(a, 1, 3);
        }

        public static async Task Main(string[] args)
        {
            byte[] data = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            Console.WriteLine("原始数组: " + string.Join(", ", data));

            // 创建一个Memory<byte>，指向data数组的一部分
            Memory<byte> memorySegment = new Memory<byte>(data, 2, 5);

            // 异步处理这部分数据
            await ModifyDataAsync(memorySegment);

            Console.WriteLine("修改后的数组: " + string.Join(", ", data));
        }

        public static async Task ModifyDataAsync(Memory<byte> memory)
        {
            // 模拟异步操作，例如，我们在这里使用Task.Delay
            await Task.Delay(1000); // 假设这是进行某种异步IO操作的时间

            // 修改数据
            for (int i = 0; i < memory.Length; i++)
            {
                // 假设我们的操作是将每个元素的值增加10
                //memory.[i] += 10;
            }
        }

        // 解析消息体 从缓存里取出一个一个完整的数据包
        public static byte[] Decode(ref List<byte> cache)
        {
            //首先要获取长度，整形4个字节，如果字节数不足4个字节
            if (cache.Count < 4)
            {
                return null;
            }
            //读取数据
            MemoryStream ms = new MemoryStream(cache.ToArray());
            BinaryReader br = new BinaryReader(ms);
            int len = br.ReadInt32();
            //根据长度，判断内容是否传递完毕, ms.Length - ms.Position为剩余流长度
            if (ms.Length - ms.Position < len || len < 0)
            {
                return null;
            }
            //获取数据，读取4个字节数据
            byte[] result = br.ReadBytes(len);
            //清空消息池
            cache.Clear();
            //讲剩余没处理的消息存入消息池
            cache.AddRange(br.ReadBytes((int)ms.Length - (int)ms.Position));
            return result;
        }

        // 方法：根据状态更新指示器颜色
        public void UpdateStatus(bool isGood)
        {
            if (isGood)
            {
                statusIndicator.BackColor = Color.Green;
            }
            else
            {
                statusIndicator.BackColor = Color.Red;
            }
        }

        private void InitializeStatusIndicator()
        {
            // 初始化状态指示器Label
            //statusIndicator = new Label();
            statusIndicator.AutoSize = false;
            statusIndicator.Width = 50;
            statusIndicator.Height = 50;
            statusIndicator.BackColor = Color.Green; // 默认状态为“good”
            statusIndicator.Text = "";

            // 设置Label为圆形
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, statusIndicator.Width/2, statusIndicator.Height/2);
            statusIndicator.Region = new Region(path);

            // 设置窗体大小和标题
            this.Width = 200;
            this.Height = 200;
            this.Text = "状态指示器示例";
        }

        private void HelloBuffer_Shown(object sender, EventArgs e)
        {
            lightSourceStatusControl.Width = this.panel4.Width;
            lightSourceStatusControl.Height = this.panel4.Height;
            lightSourceStatusControl.Invalidate();
        }

        private void HelloBuffer_ResizeEnd(object sender, EventArgs e)
        {
            lightSourceStatusControl.Width = this.panel4.Width;
            lightSourceStatusControl.Height = this.panel4.Height;
            lightSourceStatusControl.Invalidate();

        }
    }
}
