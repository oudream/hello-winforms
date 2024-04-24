using System;
using System.Collections.Concurrent;
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
    public partial class HelloThreadTimer : Form
    {
        public HelloThreadTimer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StressTestingThreadTimer1();
        }

        private void StressTestingThreadTimer1()
        {
            const int timerCount = 1000; // 测试1000个定时器
            const int testDurationSeconds = 60; // 测试运行60秒

            ThreadTimer timer = new ThreadTimer();
            ConcurrentDictionary<int, int> invocationCounts = new ConcurrentDictionary<int, int>();

            // 添加定时器
            for (int i = 0; i < timerCount; i++)
            {
                int timerIndex = i; // 捕获循环变量
                timer.AddTimer(1000, () => // 每个定时器每秒触发一次
                {
                    invocationCounts.AddOrUpdate(timerIndex, 1, (key, oldValue) => oldValue + 1);
                });
            }

            // 启动定时器线程
            timer.Run();

            OutInfo("测试运行中，请等待...");
            Thread.Sleep(testDurationSeconds * 1000 + 110); // 运行测试一定时间

            // 停止定时器
            timer.Stop();

            // 验证结果
            bool allCorrect = true;
            foreach (var count in invocationCounts)
            {
                if (count.Value < testDurationSeconds)
                {
                    OutInfo($"定时器 {count.Key} 触发次数异常，期望至少 {testDurationSeconds} 次，实际 {count.Value} 次");
                    allCorrect = false;
                }
            }

            if (allCorrect)
            {
                OutInfo("所有定时器均按预期工作！");
            }

            //OutInfo($"线程内检测定时器次数：{timer.Times}");

            OutInfo("测试完成");


        }

        private void OutInfo(string message)
        {
            this.richTextBox1.AppendText(message);
            this.richTextBox1.AppendText("\n\n");
        }
    }
}
