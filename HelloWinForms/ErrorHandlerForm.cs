using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    // Application.ThreadException 事件
    // https://learn.microsoft.com/zh-tw/dotnet/api/system.windows.forms.application.threadexception?view=windowsdesktop-8.0
    // Application.SetUnhandledExceptionMode 方法
    // https://learn.microsoft.com/zh-tw/dotnet/api/system.windows.forms.application.setunhandledexceptionmode?view=windowsdesktop-8.0
    public partial class ErrorHandlerForm : Form
    {
        public ErrorHandlerForm()
        {
            InitializeComponent();
        }

        // 程序入口
        [STAThread]
        public static void Main1()
        {
            // 注册UI线程异常处理程序
            Application.ThreadException += new ThreadExceptionEventHandler(Form1_UIThreadException);

            // 设置未处理异常模式
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // 注册非UI线程异常处理程序
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // 启动应用程序
            Application.Run(new ErrorHandlerForm());
        }

        // 按钮1用于触发UI线程异常
        private void button1_Click(object sender, EventArgs e)
        {
            string s = null;
            Console.WriteLine($"{toInt(s)}");

            //throw new ArgumentException("参数无效");
        }

        private int toInt(string s)
        {
            return int.Parse(s);
        }

        // 按钮2用于启动一个新线程，该线程将抛出异常
        private void button2_Click(object sender, EventArgs e)
        {
            ThreadStart newThreadStart = new ThreadStart(newThread_Execute);
            Thread newThread = new Thread(newThreadStart);
            newThread.Start();
        }

        // 模拟非UI线程抛出异常
        private void newThread_Execute()
        {
            string s = null;
            Console.WriteLine($"{toInt(s)}");
            //throw new Exception("方法或操作未实现。");
        }

        // 处理UI线程异常，通过对话框提示用户是否中止程序
        private static void Form1_UIThreadException(object sender, ThreadExceptionEventArgs t)
        {
            DialogResult result = DialogResult.Cancel;
            try
            {
                // 将异常信息输出到文件
                LogExceptionToFile("UI线程异常", t.Exception);

                result = ShowThreadExceptionDialog("UI线程异常", t.Exception);
            }
            catch
            {
                try
                {
                    MessageBox.Show("致命的Windows Forms 错误",
                        "致命错误", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Restart();
                }
            }

            if (result == DialogResult.Abort)
                Application.Restart();
        }

        // 处理非UI线程异常，记录到事件日志，并通知用户
        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                //string errorMsg = "应用程序发生错误。请联系管理员，提供以下信息：\n\n";

                // 将异常信息输出到文件
                LogExceptionToFile("非UI线程异常", ex);

                LogErrorToEventLog("非UI线程异常", ex);
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show("致命的非UI错误",
                        "无法写入错误到事件日志。原因: " + exc.Message,
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }

        private static void LogErrorToEventLog(string exceptionType, Exception ex)
        {
            // 检查并创建事件源
            if (!EventLog.SourceExists("ThreadException"))
            {
                EventLog.CreateEventSource("ThreadException", "Application");
            }

            // 创建事件日志实例并写入异常信息
            EventLog myLog = new EventLog();
            myLog.Source = "ThreadException";
            string errorMsg = $"[{DateTime.Now}] {exceptionType}: {ex.Message}\n堆栈跟踪:\n{ex.StackTrace}\n\n";
            myLog.WriteEntry(errorMsg);
        }

        // 将异常信息写入文件
        private static void LogExceptionToFile(string exceptionType, Exception ex)
        {
            string filePath = "异常日志.txt";
            string errorMsg = $"[{DateTime.Now}] {exceptionType}: {ex.Message}\n堆栈跟踪:\n{ex.StackTrace}\n\n";

            // 将异常信息追加到文件中
            File.AppendAllText(filePath, errorMsg);
        }

        // 创建并显示异常信息对话框
        private static DialogResult ShowThreadExceptionDialog(string title, Exception e)
        {
            string errorMsg = "应用程序发生错误。请联系管理员，提供以下信息：\n\n";
            errorMsg += e.Message + "\n\n堆栈跟踪:\n" + e.StackTrace;
            return MessageBox.Show(errorMsg, title, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Stop);
        }
    }
}
