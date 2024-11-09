using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HelloWinForms
{
    public partial class HelloDaemon : Form
    {
        public HelloDaemon()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DaemonHelper.Init();
            DaemonHelper.Run();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DaemonHelper.Stop(true);
        }

        private void Stop()
        {
            // 通过进程名称查找并终止挂起的进程
            string processName = "infer_demo";  // 替换为你的进程名称

            foreach (Process process in Process.GetProcessesByName(processName))
            {
                try
                {
                    // 强制终止进程
                    Console.WriteLine($"正在终止进程: {process.ProcessName} (PID: {process.Id})");
                    process.Kill();
                    Console.WriteLine("进程已终止.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"无法终止进程 {process.ProcessName}: {ex.Message}");
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Restore();
        }

        private void Stop2()
        {
            string processName = "infer_demo";  // 替换为你的进程名称

            try
            {
                // 使用 PowerShell 强制终止进程
                ProcessStartInfo psi = new ProcessStartInfo("powershell", $"-Command \"Stop-Process -Name {processName} -Force\"")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                Process powerShellProcess = Process.Start(psi);
                powerShellProcess.WaitForExit();

                Console.WriteLine("已通过 PowerShell 强制终止进程.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"无法通过 PowerShell 终止进程: {ex.Message}");
            }
        }

        // 导入 Windows API
        [DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern uint ResumeThread(IntPtr hThread);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        const int THREAD_SUSPEND_RESUME = 0x0002;

        // 恢复进程
        private void Restore()
        {
            string processName = "infer_demo";  // 替换为你需要恢复的进程名称
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine("未找到进程");
                return;
            }

            Process process = processes[0];

            // 恢复所有线程
            foreach (ProcessThread thread in process.Threads)
            {
                IntPtr hThread = OpenThread(THREAD_SUSPEND_RESUME, false, (uint)thread.Id);
                if (hThread == IntPtr.Zero)
                {
                    Console.WriteLine($"无法打开线程 ID: {thread.Id}");
                    continue;
                }

                // 恢复线程
                uint suspendCount = ResumeThread(hThread);
                if (suspendCount == uint.MaxValue)
                {
                    Console.WriteLine($"恢复线程 ID: {thread.Id} 失败");
                }
                else
                {
                    Console.WriteLine($"线程 ID: {thread.Id} 已恢复");
                }

                // 关闭句柄
                CloseHandle(hThread);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IsSuspended();
        }

        // 判断是否挂起的进程
        private void IsSuspended()
        {
            string processName = "infer_demo";  // 替换为你的进程名称
            Process[] processes = Process.GetProcessesByName(processName);

            if (processes.Length == 0)
            {
                Console.WriteLine("未找到进程");
                return;
            }

            Process process = processes[0];

            bool isSuspended = process.Threads.Count <= 0;

            // 检查进程的每个线程状态
            foreach (ProcessThread thread in process.Threads)
            {
                Console.WriteLine($"线程 ID: {thread.Id}, 状态: {thread.ThreadState}");

                // 检查线程是否被挂起
                if (thread.ThreadState == System.Diagnostics.ThreadState.Wait || thread.ThreadState == System.Diagnostics.ThreadState.Terminated)
                {
                    isSuspended = true;
                }
            }

            if (isSuspended)
            {
                Console.WriteLine("进程有被挂起的线程.");
            }
            else
            {
                Console.WriteLine("进程没有挂起的线程.");
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            DaemonHelper.SaveSampleDaemons();
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }
    }


    
}
