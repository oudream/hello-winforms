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
    public partial class HelloAsync : Form
    {
        public HelloAsync()
        {
            InitializeComponent();

            syncButton.Click += SyncButton_Click;
            // 错误处理：当你在事件处理器中加上 await 调用另一个异步方法时，
            // 你实际上是在告诉编译器你想要等待这个异步操作完成，并且在异步操作期间保持事件处理程序的上下文。
            // 如果 AsyncButton_Click 方法内部发生了异常，它将会被这个 await 表达式捕获，并且你可以在调用点后面使用 try-catch 块来处理这个异常。
            //
            // 执行流：使用 await 保持了方法的异步性质，允许UI线程在等待异步操作完成时继续处理其他事件，
            // 例如用户的点击或者画面的更新，从而保持应用程序的响应性
            asyncButton.Click += async (sender, e) => await AsyncButton_Click(sender, e);
        }
        private void SyncButton_Click(object sender, EventArgs e)
        {
            outputRichTextBox.Text = "SyncButton_Click: Starting Sync Operation...";
            // Force UI update
            Application.DoEvents();
            string result = LongRunningOperationSync();
            outputRichTextBox.AppendText($"SyncButton_Click: {result}\n");
        }

        private string LongRunningOperationSync()
        {
            // 模拟一个耗时操作
            Thread.Sleep(5000); // 模拟5秒的耗时任务
            return "Sync Operation Completed";
        }

        private async Task AsyncButton_Click(object sender, EventArgs e)
        {
            AppendTextSafe($"创建线程来测试: Button. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            timer1.Enabled = true;
            outputRichTextBox.Text = "AsyncButton_Click: Starting Async Operation...";
            // No need to force UI update, await will yield control to UI thread

            string result = await LongRunningOperationAsync();
            outputRichTextBox.AppendText($"AsyncButton_Click: {result}\n");
            timer1.Enabled = false;
        }

        private async Task<string> LongRunningOperationAsync()
        {
            // 模拟一个耗时操作
            await Task.Delay(5000); // 异步等待5秒
            return "Async Operation Completed";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            outputRichTextBox.AppendText($"timer1_Tick: ---\n");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AppendTextSafe($"创建线程来测试: Button. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            // 创建并启动一个新线程
            Thread newThread = new Thread(() => AsyncOperationWithProgress());
            newThread.Start();
            //newThread.Join(); // 等待新线程完成

            AppendTextSafe("创建线程来测试，End\n");
        }

        private void AsyncOperationWithProgress()
        {
            AppendTextSafe($"创建线程来测试: Thread started. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");

            // 启动异步操作，但不立即等待它
            var task = PerformAsyncOperation();

            // 在等待异步操作完成的同时，显示进度信息
            while (!task.IsCompleted)
            {
                AppendTextSafe($"创建线程来测试: Thread ID: {Thread.CurrentThread.ManagedThreadId} - Waiting for async operation to complete...\n");
                Thread.Sleep(1000); // 模拟正在进行的工作或进度更新
            }

            // 现在等待异步操作完成以确保任何异常都能被捕获
            task.Wait();

            AppendTextSafe($"创建线程来测试: Thread completed. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
        }

        private async Task PerformAsyncOperation()
        {
            // 注意：此方法内的直接UI操作将在启动该Task的线程上下文中执行
            // 若该方法在新线程中被调用，需要使用Invoke进行UI操作
            AppendTextSafe($"创建线程来测试: Async operation started. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            // 模拟耗时的异步操作
            await Task.Delay(5000);
            AppendTextSafe($"创建线程来测试: Async operation completed. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
        }

        // 安全地在UI线程上更新RichTextBox文本的方法
        private void AppendTextSafe(string text)
        {
            if (outputRichTextBox.InvokeRequired)
            {
                outputRichTextBox.Invoke(new Action<string>(AppendTextSafe), text);
            }
            else
            {
                outputRichTextBox.AppendText(text);
            }
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            await AsyncButton_Click(sender, e);
        }
    }
}
