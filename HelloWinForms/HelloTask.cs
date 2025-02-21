using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloTask : Form
    {
        public HelloTask()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            // 获取当前UI线程ID
            int uiThreadId = Thread.CurrentThread.ManagedThreadId;
            MessageBox.Show($"按钮点击时的UI线程ID：{uiThreadId}, Apartment: {Thread.CurrentThread.GetApartmentState()}", "开始");

            // 在Task.Run中执行任务
            try
            {
                await Task.Run(() =>
                {
                    // Task.Run 内部线程的信息
                    int taskThreadId = Thread.CurrentThread.ManagedThreadId;
                    //Thread.Sleep(1000 * 10);
                    var aptState = Thread.CurrentThread.GetApartmentState();
                    MessageBox.Show($"Task.Run 线程 ID: {taskThreadId}, Apartment: {aptState}",
                        "Task.Run 内部");

                    // 尝试在 Task.Run 中创建并显示窗体
                    Form form = new Form();
                    form.Text = "Task.Run 中创建的窗体";
                    Label label = new Label
                    {
                        Text = $"窗体运行于线程 {Thread.CurrentThread.ManagedThreadId}",
                        AutoSize = true,
                        Location = new System.Drawing.Point(10, 10)
                    };
                    form.Controls.Add(label);

                    // 这里使用 ShowDialog 显示模态窗体
                    form.ShowDialog();
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Task.Run 内部异常: {ex.Message}", "错误");
            }

            // await后继续运行的代码
            int afterAwaitThreadId = Thread.CurrentThread.ManagedThreadId;
            MessageBox.Show($"await 后的线程ID：{afterAwaitThreadId}", "await后");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Console.WriteLine(DateTime.Now);
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            Console.Write($"创建线程来测试: Async operation started. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            await Task.Delay(5000);
            Console.Write($"创建线程来测试: Async operation started. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Console.Write($"创建线程来测试: Button. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            // 创建并启动一个新线程
            Thread newThread = new Thread(() =>
            {
                Console.Write($"创建线程来测试: Thread started. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");

                // 启动异步操作，但不立即等待它
                var task = PerformAsyncOperation();

                // 在等待异步操作完成的同时，显示进度信息
                while (!task.IsCompleted)
                {
                    Console.Write($"创建线程来测试: Thread ID: {Thread.CurrentThread.ManagedThreadId} - Waiting for async operation to complete...\n");
                    Thread.Sleep(1000); // 模拟正在进行的工作或进度更新
                }

                // 现在等待异步操作完成以确保任何异常都能被捕获
                task.Wait();

                Console.Write($"创建线程来测试: Thread completed. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            });
            newThread.Start();
            //newThread.Join(); // 等待新线程完成

            Console.Write("创建线程来测试，End\n");
        }

        private async Task PerformAsyncOperation()
        {
            // 注意：此方法内的直接UI操作将在启动该Task的线程上下文中执行
            // 若该方法在新线程中被调用，需要使用Invoke进行UI操作
            Console.Write($"创建线程来测试: Async operation started. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
            // 模拟耗时的异步操作
            await Task.Delay(5000);
            //await Task.Delay(5000).ConfigureAwait(false);
            Console.Write($"创建线程来测试: Async operation completed. Thread ID: {Thread.CurrentThread.ManagedThreadId}\n");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var path = Path.GetDirectoryName("C:\\Users\\Administrator\\Desktop\\CxSolderBall");
            // 模拟文件列表
            var fakeFiles = new List<FileInfo>
            {
                new FileInfo("image_0_20240101010101.tif"),
                new FileInfo("image_0_20240101010102.tif"),
                new FileInfo("image_0_20240101010103.tif"),
                new FileInfo("image_0_20240101010104.tif"),
                new FileInfo("image_0_20240101010105.tif"),
                new FileInfo("image_0_20240101010106.tif"),
                new FileInfo("image_0_20240101010107.tif"),
                new FileInfo("image_0_20240101010108.tif"),
                new FileInfo("image_0_20240101010109.tif"),
                new FileInfo("image_0_20240101010110.tif"),  // 10 张，符合条件

                new FileInfo("image_1_20240101020101.tif"),
                new FileInfo("image_1_20240101020102.tif"),
                new FileInfo("image_1_20240101020103.tif"),
                new FileInfo("image_1_20240101020104.tif"),
                new FileInfo("image_1_20240101020105.tif"),
                new FileInfo("image_1_20240101020106.tif"),
                new FileInfo("image_1_20240101020107.tif"),
                new FileInfo("image_1_20240101020108.tif"),
                new FileInfo("image_1_20240101020109.tif"),
                new FileInfo("image_1_20240101020110.tif")  // 10 张，符合条件
            };

            // 模拟分组
            var groupedImages = fakeFiles.GroupBy(file =>
            {
                var parts = file.Name.Split('_');
                return int.Parse(parts[1]);  // 解析 Key (0 或 1)
            }).ToList();

            // 计算 Key=0 和 Key=1 的图片数量
            int count0 = groupedImages.Where(g => g.Key == 0).Sum(g => g.Count());
            int count1 = groupedImages.Where(g => g.Key == 1).Sum(g => g.Count());

            var groupInfo = string.Join("；", groupedImages.Select(g => $"探测器-{g.Key}: {g.Count()} 张"));

            Console.WriteLine($"Key=0 的图片数量：{count0}");
        }
    }
}
