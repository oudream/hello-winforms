using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloTimer : Form
    {
        public static HelloTimer form1form1;

        private System.Timers.Timer timer;

        public HelloTimer()
        {
            InitializeComponent();
            form1form1 = this;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int result = 1 << 3;
            long dtNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;
            richTextBox1.AppendText($"OnTimerElapsed Thread ID: {mainThreadId} - {result} - dtNow:{dtNow}\r\n");


            // 创建一个定时器，设置间隔为1000毫秒（1秒）
            timer = new System.Timers.Timer(3000);

            // 定时器触发时执行的方法
            timer.Elapsed += OnTimerElapsed;

            // 启动定时器
            timer.Start();

            Console.WriteLine("定时器已启动。按任意键退出。");

        }
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            long dtNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            timer.Stop();
            int mainThreadId = Thread.CurrentThread.ManagedThreadId;
            Invoke(new System.Action(() =>
            {
                form1form1.richTextBox1.AppendText($"OnTimerElapsed Thread ID: {mainThreadId} - dtNow:{dtNow}\r\n");
            }));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 停止定时器
            timer.Stop();
        }

        private int isRunning = 0;
        private void button3_Click(object sender, EventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
                timer = null;
            }
            timer = new System.Timers.Timer(50);
            // 设置 Elapsed 事件处理程序
            timer.Elapsed += (_, __) =>
            {
                // 使用 Interlocked 操作确保原子性
                if (Interlocked.Exchange(ref isRunning, 1) == 0)
                {
                    try
                    {
                        long dtNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                        int mainThreadId = Thread.CurrentThread.ManagedThreadId;
                        Invoke(new System.Action(() =>
                        {
                            form1form1.richTextBox1.AppendText($"OnTimerElapsed Thread ID: {mainThreadId} - dtNow:{dtNow}\r\n");
                        }));
                        // 模拟处理耗时的操作
                        Thread.Sleep(3000);
                    }
                    finally
                    {
                        // 重置标志位，以允许下一次回调
                        Interlocked.Exchange(ref isRunning, 0);
                    }
                }
            };
            timer.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 停止定时器
            timer.Stop();
            timer.Dispose();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AoLeiStatus status = new AoLeiStatus();
            richTextBox1.AppendText($"HighPressureOnIndicator: {status.HighPressureOffIndicator} - HighPressureOnIndicator:{status.HighPressureOnIndicator}\r\n");
            richTextBox1.AppendText($"VoltageSet: {status.VoltageSet} - InterlockIndicator:{status.InterlockIndicator}\r\n");
        }
    }

    public struct AoLeiStatus
    {
        public int VoltageSet;               // 1043 电压给定
        public int CurrentSet;               // 1044 电流给定
        public int VoltageFeedback;          // 1011 电压反馈
        public int CurrentFeedback;          // 1012 电流反馈
        public int OvercurrentProtection;    // 2  过流保护
        public int OvervoltageProtection;    // 4  过压保护
        public int PowerSupplyAlarm;         // 10 电源报警指示
        public int InterlockIndicator;       // 11 互锁指示
        public int HighPressureOffIndicator; // 12 高压关状态指示
        public int HighPressureOnIndicator;  // 14 高压开状态指示
        public int RemoteModeEnabled;        // 4  远程模式指示
        public int HighPressureOnEnabled;    // 5  高压开指示
        public int OverPowerProtection;      // 6  过功率保护
        public int LocalModeEnabled;         // 14 本地模式指示
    }
}
