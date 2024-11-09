using CxWorkStation.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloEquals : Form
    {
        public HelloEquals()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProductKeyInfo keyInfo1 = new ProductKeyInfo { LineNumber = 1, BatchNumber = 100, Position = 5 };
            ProductKeyInfo keyInfo2 = new ProductKeyInfo { LineNumber = 1, BatchNumber = 100, Position = 5 };
            ProductKeyInfo keyInfo3 = new ProductKeyInfo { LineNumber = 2, BatchNumber = 101, Position = 10 };

            // 检查相等性
            Console.WriteLine(keyInfo1 == keyInfo2); // 输出: True
            Console.WriteLine(keyInfo1.Equals(keyInfo2)); // 输出: True
            Console.WriteLine(keyInfo1 == keyInfo3); // 输出: False

            // 检查不相等性
            Console.WriteLine(keyInfo1 != keyInfo3); // 输出: True
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (TryParseDateTime("2024-11-05 13:59:38", out var dateTime))
            {
                var success = IsAppRunningSuccessfully("infer.log", @"D:\cyg\FastDeployV31", dateTime);
                Console.WriteLine($"IsAppRunningSuccessfully: {success}");
            }

            Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("log.txt",
        rollingInterval: RollingInterval.Day,
        rollOnFileSizeLimit: true)
    .CreateLogger();

            try
            {
                // Your program here...
                const string name = "Serilog";
                Log.Information("Hello, {Name}!", name);
                throw new InvalidOperationException("Oops...");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception");
            }
            finally
            {
                Log.CloseAndFlush(); // ensure all logs written before app exits
            }
        }

        public static bool TryParseDateTime(string dateTimeString, out DateTime dateTime)
        {
            // 定义可能的日期时间格式
            string[] formats = new string[]
            {
            "yyyy-MM-dd HH:mm:ss.fff", // 包括毫秒
            "yyyy-MM-dd HH:mm:ss:fff", // 包括毫秒
            "yyyy-MM-dd HH:mm:ss",     // 不包括毫秒
            "yyyy/MM/dd HH:mm:ss:fff", // 使用斜杠分隔符，包括毫秒
            "yyyy/MM/dd HH:mm:ss.fff", // 使用斜杠分隔符，包括毫秒
            "yyyy/MM/dd HH:mm:ss",     // 使用斜杠分隔符，不包括毫秒
                                       // 在这里可以根据需要添加更多格式
            };

            // 尝试解析字符串
            return DateTime.TryParseExact(dateTimeString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        // 检查 infer.log 是否包含成功启动的信息，且时间大于进程启动时间
        private static bool IsAppRunningSuccessfully(string sAppLogFileName, string sWorkingDirectory, DateTime startTime)
        {
            try
            {
                string logFilePath = Path.IsPathRooted(sAppLogFileName) ? sAppLogFileName : Path.Combine(sWorkingDirectory, sAppLogFileName);
                if (File.Exists(logFilePath))
                {
                    string lastLogLine = ReadLastLineWithContent(logFilePath);

                    if (!string.IsNullOrEmpty(lastLogLine))
                    {
                        // 检查日志是否包含启动成功的消息
                        string successMessage = "Server listening on port";
                        if (lastLogLine.Contains(successMessage))
                        {
                            // 从日志行中提取时间
                            string timestamp = lastLogLine.Substring(1, 19); // 假设时间戳是前19个字符
                            if (TimeHelper.TryParseDateTime(timestamp, out DateTime logTime))
                            {
                                // 检查日志时间是否大于进程启动时间
                                if (logTime > startTime)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查日志文件时出错: {ex.Message}");
            }

            return false;
        }

        static string ReadLastLineWithContent(string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (fs.Length == 0)
                {
                    return string.Empty; // 文件为空
                }

                long fileLength = fs.Length;
                long startPosition = Math.Max(0, fileLength - 4096); // 从文件末尾向前移动 4KB

                fs.Seek(startPosition, SeekOrigin.Begin);

                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string content = sr.ReadToEnd();

                    // 按行拆分，保留行分隔符
                    string[] lines = content.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                    int emptyLineCount = 0;
                    for (int i = lines.Length - 1; i >= 0; i--)
                    {
                        if (string.IsNullOrWhiteSpace(lines[i].Trim()))
                        {
                            emptyLineCount++;
                            //if (emptyLineCount > 5)
                            //{
                            //    continue; // 跳过超过 5 个的空行
                            //}
                        }
                        else
                        {
                            // 找到最后一个非空行
                            string lastLine = lines[i];
                            //Console.WriteLine("最后一行内容为：" + lastLine);
                            return lastLine;
                            //break;
                        }
                    }
                }
            }
            return string.Empty;
        }
    }


    public struct ProductKeyInfo : IEquatable<ProductKeyInfo>
    {
        public uint LineNumber;
        public uint BatchNumber;
        public int Position;

        public override bool Equals(object obj)
        {
            if (obj is ProductKeyInfo other)
            {
                return Equals(other);
            }
            return false;
        }

        public bool Equals(ProductKeyInfo other)
        {
            return LineNumber == other.LineNumber &&
                   BatchNumber == other.BatchNumber &&
                   Position == other.Position;
        }

        public override int GetHashCode()
        {
            // 使用质数计算哈希码
            unchecked // 防止溢出
            {
                int hash = 17;
                hash = hash * 23 + LineNumber.GetHashCode();
                hash = hash * 23 + BatchNumber.GetHashCode();
                hash = hash * 23 + Position.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ProductKeyInfo left, ProductKeyInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ProductKeyInfo left, ProductKeyInfo right)
        {
            return !(left == right);
        }


    }

}
