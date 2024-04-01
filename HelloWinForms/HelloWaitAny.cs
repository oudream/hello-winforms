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
    public partial class HelloWaitAny : Form
    {
        public HelloWaitAny()
        {
            InitializeComponent();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            HelloWaitAny1();
        }

        private static AutoResetEvent step1Event = new AutoResetEvent(false);
        private static AutoResetEvent step2Event = new AutoResetEvent(false);
        private static AutoResetEvent step3Event = new AutoResetEvent(false);
        private static int[] stepCompletionCounts = new int[3];
        private static System.Windows.Forms.Timer timer;

        public static void HelloWaitAny1()
        {
            Console.WriteLine("批量压测开始...");

            // 启动工作线程，模拟步骤的异步完成
            StartWorkerThread(step1Event, 100, 0); // 步骤1
            StartWorkerThread(step2Event, 150, 1); // 步骤2
            StartWorkerThread(step3Event, 200, 2); // 步骤3

            // 设置1分钟的压测时间
            timer = new System.Windows.Forms.Timer();
            timer.Tick += TimerCallback;
            timer.Interval = 1000 * 60;

            AutoResetEvent[] events = new AutoResetEvent[] { step1Event, step2Event, step3Event };
            int totalSteps = events.Length;

            // 业务线程监控步骤完成情况
            new Thread(() =>
            {
                DateTime startTime = DateTime.Now;
                while (DateTime.Now - startTime < TimeSpan.FromMinutes(1))
                {
                    int index = WaitHandle.WaitAny(events, 30);
                    if (index == WaitHandle.WaitTimeout)
                    {
                        Console.WriteLine("等待超时。------");
                        continue;
                    }
                    stepCompletionCounts[index]++;
                    Console.WriteLine($"步骤{index + 1}完成。总完成次数: {stepCompletionCounts[index]}");
                }

                // 打印每个步骤的完成次数
                for (int i = 0; i < totalSteps; i++)
                {
                    Console.WriteLine($"步骤{i + 1}的完成次数: {stepCompletionCounts[i]}");
                }
            })
            { IsBackground = true }.Start();
        }

        private static void StartWorkerThread(AutoResetEvent autoEvent, int workDuration, int stepIndex)
        {
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(workDuration); // 模拟工作耗时
                    autoEvent.Set(); // 通知步骤完成
                }
            })
            { IsBackground = true }.Start();
        }

        private static void TimerCallback(object sender, EventArgs e)
        {
            Console.WriteLine("压测结束。");
            // 停止计时器，避免再次触发
            timer.Dispose();
        }

        private static bool TimerDisposed => timer == null;
    }
}
