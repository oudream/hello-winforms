using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloProcess : Form
    {
        public HelloProcess()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            string processName = "infer_demo"; // 不要包含 .exe 后缀
            //bool isRunning = IsProcessRunning(processName);
            bool isRunning = IsProcessAlive(processName);
            stopwatch.Stop();
            Console.WriteLine($"视觉检测 took {stopwatch.ElapsedMilliseconds} ms");

            if (isRunning)
            {
                Console.WriteLine($"{processName}.exe is running.");
            }
            else
            {
                Console.WriteLine($"{processName}.exe is not running.");
            }
        }

        static bool IsProcessRunning(string processName)
        {
            // 获取所有正在运行的进程
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        static bool IsProcessAlive(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    // 检查进程是否响应
                    if (process.Responding)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void button1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
