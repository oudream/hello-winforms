using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloExeInForm : Form
    {

        [DllImport("User32.dll", EntryPoint = "SetParent")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "MoveWindow")]
        private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hWnd);

        private IntPtr embeddedAppHandle = IntPtr.Zero;  // 保存嵌入应用的句柄
        private Process externalProcess = null;  // 保存外部进程对象

        public HelloExeInForm()
        {
            InitializeComponent();
        }

        private void EmbedExternalApp()
        {
            string fexePath = @"E:\运动控制卡软件资料\雷赛\MotionStudio_v1_4_10_beta_20231028\MotionStudio.exe";
            //string fexePath = @"C:\Program Files (x86)\Siemens\Automation\WinCC RT Advanced\HmiRTm.exe"; // 外部exe位置
            externalProcess = new Process();
            externalProcess.StartInfo.FileName = fexePath;
            //externalProcess.StartInfo.Arguments = @"C:\PROGRAMDATA\SIEMENS\CORTHMIRTM\HMIRTM\PROJECTS\pdata.fwc";
            //externalProcess.StartInfo.WorkingDirectory = @"C:\Program Files (x86)\Siemens\Automation\WinCC RT Advanced\";
            externalProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            externalProcess.Start();

            // 使用 WaitForInputIdle 等待进程进入空闲状态，确保窗口已经显示

            System.Threading.Thread.Sleep(3000);

            externalProcess.WaitForInputIdle();

            // 等待窗口创建
            while (externalProcess.MainWindowHandle == IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(100);
            }

            embeddedAppHandle = externalProcess.MainWindowHandle;  // 保存句柄

            // 将外部程序窗口嵌入到当前窗体
            SetParent(embeddedAppHandle, this.Handle);

            // 显示并最大化嵌入窗口
            ShowWindow(embeddedAppHandle, (int)ProcessWindowStyle.Maximized);

            // 调整窗口大小
            ResizeEmbeddedApp();
        }
        private void ResizeEmbeddedApp()
        {
            if (embeddedAppHandle != IntPtr.Zero)
            {
                // 调整嵌入的窗口大小以匹配当前窗口的大小
                MoveWindow(embeddedAppHandle, 0, 0, this.ClientSize.Width, this.ClientSize.Height, true);
            }
        }

        private void HelloExeInForm_Load(object sender, EventArgs e)
        {
            EmbedExternalApp();
        }

        private void HelloExeInForm_Resize(object sender, EventArgs e)
        {
            ResizeEmbeddedApp();
        }

        private void HelloExeInForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (externalProcess != null && !externalProcess.HasExited)
            {
                // 等待外部进程退出
                externalProcess.CloseMainWindow();  // 尝试优雅地关闭进程
                externalProcess.WaitForExit(15000);  // 等待5秒钟让进程退出
                if (!externalProcess.HasExited)
                {
                    externalProcess.Kill();  // 强制终止进程（如果需要）
                }
            }
        }
    }
}
