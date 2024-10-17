using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloPython : Form
    {
        public HelloPython()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 定义Python脚本路径和文件路径
            string pythonScriptPath = "data/hello1.py";

            // 多次调用脚本，每次传递不同的文件路径作为参数
            RunPythonScript2(pythonScriptPath, "data/hello.txt");
            RunPythonScript2(pythonScriptPath, "data/tmp.txt");
        }


        // 用于执行 Python 代码并传递参数的函数
        private void RunPythonScript1(string scriptPath, string filePath)
        {
            // 创建并配置进程
            Process process = new Process();
            process.StartInfo.FileName = "python"; // Python可执行文件路径
            process.StartInfo.Arguments = $"{scriptPath} \"{filePath}\""; // 将Python脚本路径和文件路径作为参数传递
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // 启动进程
            process.Start();

            // 异步读取输出和错误
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            // 等待进程结束
            process.WaitForExit();

            // 将结果显示在RichTextBox中，每次执行都会追加新结果
            richTextBoxOutput.AppendText("Executing Python Script: " + scriptPath + "\n");
            richTextBoxOutput.AppendText("File Path Argument: " + filePath + "\n");
            richTextBoxOutput.AppendText("Standard Output:\n" + output + "\n");
            if (!string.IsNullOrWhiteSpace(error))
            {
                richTextBoxOutput.AppendText("Standard Error:\n" + error + "\n");
            }
            richTextBoxOutput.AppendText("\n");
        }

        // 用于执行 Python 代码并实时捕获输出的函数
        // 其实是python的问题，python默认缓冲区满了才会输出， 运行时加 -u 参数就可以立即输出了。
        private void RunPythonScript2(string scriptPath, string filePath)
        {
            // 创建并配置进程
            Process process = new Process();
            process.StartInfo.FileName = "python"; // Python 可执行文件路径
            process.StartInfo.Arguments = $"-u {scriptPath} \"{filePath}\""; // Python 脚本路径和文件路径作为参数
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;

            // 启动进程并开始异步读取输出
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // 事件处理函数，用于处理每行标准输出
            process.OutputDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    // 使用Invoke更新UI，因为数据可能是从另一个线程触发的
                    this.Invoke(new Action(() =>
                    {
                        richTextBoxOutput.AppendText(args.Data + Environment.NewLine);
                    }));
                }
            };

            // 事件处理函数，用于处理每行标准错误输出
            process.ErrorDataReceived += (sender, args) =>
            {
                if (!string.IsNullOrEmpty(args.Data))
                {
                    // 使用Invoke更新UI
                    this.Invoke(new Action(() =>
                    {
                        richTextBoxOutput.AppendText("Error: " + args.Data + Environment.NewLine);
                    }));
                }
            };

            // 异步等待进程结束
            process.Exited += (sender, args) =>
            {
                this.Invoke(new Action(() =>
                {
                    richTextBoxOutput.AppendText("Process finished." + Environment.NewLine);
                }));
            };
        }

    }
}
