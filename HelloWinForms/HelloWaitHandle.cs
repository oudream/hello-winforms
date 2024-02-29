using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloWaitHandle : Form
    {
        //private ManualResetEvent[] resetEvents;
        private AutoResetEvent[] resetEvents;
        private Thread waitThread;
        private bool running = true;
        private Random random;


        public HelloWaitHandle()
        {
            InitializeComponent();

            // 初始化 ManualResetEvent 数组
            resetEvents = new AutoResetEvent[3];
            for (int i = 0; i < resetEvents.Length; i++)
            {
                resetEvents[i] = new AutoResetEvent(true);
            }

            // 随机数生成器
            random = new Random();
        }

        private void WaitForEvents()
        {
            while (running)
            {
                int index = WaitHandle.WaitAny(resetEvents, 100);
                if (index >= 0 && index < resetEvents.Length)
                {
                    //resetEvents[index].Reset();
                    this.Invoke(new System.Action(() => MessageBox.Show($"事件 {index} 被触发")));
                }
                else
                {
                    Console.WriteLine($"WaitAny.Result: {index}");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index = random.Next(resetEvents.Length);
            resetEvents[index].Set();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // 清理
            foreach (var resetEvent in resetEvents)
            {
                resetEvent.Dispose();
            }
            waitThread.Join();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 启动一个线程来等待事件
            waitThread = new Thread(WaitForEvents);
            waitThread.Start();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            running = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            long milliseconds = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
            MessageBox.Show($"当前时间戳: {milliseconds}");
        }
    }


}
