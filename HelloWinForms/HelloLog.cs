using CxSystemConfiguration.Utilities;
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
    public partial class HelloLog : Form
    {
        public HelloLog()
        {
            InitializeComponent();

            // 如有必要，在此初始化LogHelper
            LogHelper.InitLog();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int numberOfTasks = 100; // 并行日志任务的数量
            int numberOfLogsPerTask = 100000; // 每个任务写入的日志数量

            Parallel.For(0, numberOfTasks, i =>
            {
                for (int j = 0; j < numberOfLogsPerTask; j++)
                {
                    // 随机生成日志级别以增加多样性
                    LogLevel logLevel = (LogLevel)new Random().Next(0, 6);
                    string message = $"Task {i}, Log {j}, Level {logLevel}";

                    // 根据随机生成的级别写入日志
                    switch (logLevel)
                    {
                        case LogLevel.Verbose:
                            break;
                        case LogLevel.Debug:
                            LogHelper.Debug(message);
                            break;
                        case LogLevel.Information:
                            LogHelper.Information(message);
                            break;
                        case LogLevel.Warning:
                            LogHelper.Warning(message);
                            break;
                        case LogLevel.Error:
                            LogHelper.Error(message);
                            break;
                        case LogLevel.Fatal:
                            LogHelper.Fatal(message);
                            break;
                    }
                    if (j % 1000 == 0)
                    {
                        Thread.Sleep(15);
                    }
                }
            });

        }
    }
}
