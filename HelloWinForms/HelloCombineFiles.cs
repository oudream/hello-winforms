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
using YamlDotNet.Core.Tokens;

namespace HelloWinForms
{
    public partial class HelloCombineFiles : Form
    {
        public HelloCombineFiles()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                BundleFiles("振动盘图片/UpShp_Model.sbm", "振动盘图片/UpShp_ModelImage.bmp", "振动盘图片/UpShp_ModelRegion.hobj", ModelFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"生成文件失败: {ex.Message}");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                UnbundleFiles(ModelFilePath, "振动盘图片2");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解文件失败: {ex.Message}");
            }
        }

        private const string ModelFilePath = "UpShp.data";
        private const int HeaderSize = 4096; // 头部大小固定为4KB

        public static void BundleFiles(string modelFilePath, string imageFilePath, string regionFilePath, string bundledFilePath)
        {
            // 检查合并文件路径是否已存在，如果存在，则删除
            if (File.Exists(bundledFilePath))
            {
                File.Delete(bundledFilePath); // 删除已存在的文件
            }

            using (var bundledFileStream = new FileStream(bundledFilePath, FileMode.Create, FileAccess.Write))
            {
                // 构建头部信息
                var headerInfo = $"{Path.GetFileName(modelFilePath)}|{new FileInfo(modelFilePath).Length};"
                               + $"{Path.GetFileName(imageFilePath)}|{new FileInfo(imageFilePath).Length};"
                               + $"{Path.GetFileName(regionFilePath)}|{new FileInfo(regionFilePath).Length};";
                var headerBytes = Encoding.UTF8.GetBytes(headerInfo);
                if (headerBytes.Length > HeaderSize)
                {
                    throw new Exception("Header information exceeds the allocated size.");
                }

                // 写入头部，包括必要的填充以达到4KB
                bundledFileStream.Write(headerBytes, 0, headerBytes.Length);
                bundledFileStream.Write(new byte[HeaderSize - headerBytes.Length], 0, HeaderSize - headerBytes.Length);

                // 写入文件内容
                WriteFileToStream(modelFilePath, bundledFileStream);
                WriteFileToStream(imageFilePath, bundledFileStream);
                WriteFileToStream(regionFilePath, bundledFileStream);
            }
            MessageBox.Show($"生成文件: {bundledFilePath}");
        }

        public static void UnbundleFiles(string bundledFilePath, string outputDirectory)
        {
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            using (var bundledFileStream = new FileStream(bundledFilePath, FileMode.Open, FileAccess.Read))
            {
                // 读取并解析头部
                var headerBytes = new byte[HeaderSize];
                bundledFileStream.Read(headerBytes, 0, HeaderSize);
                var headerInfo = Encoding.UTF8.GetString(headerBytes).Trim('\0');
                var filesInfo = headerInfo.Split(';');

                foreach (var fileInfo in filesInfo)
                {
                    if (string.IsNullOrEmpty(fileInfo)) continue;

                    var parts = fileInfo.Split('|');
                    var fileName = parts[0];
                    var fileSize = long.Parse(parts[1]);

                    // 从流中提取文件内容
                    var fileContent = new byte[fileSize];
                    bundledFileStream.Read(fileContent, 0, (int)fileSize);
                    File.WriteAllBytes(Path.Combine(outputDirectory, fileName), fileContent);
                }
            }
        }

        private static void WriteFileToStream(string filePath, FileStream outputStream)
        {
            var fileContent = File.ReadAllBytes(filePath);
            outputStream.Write(fileContent, 0, fileContent.Length);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string dateTimeString = "2023/02/26 15:04:23:123"; // 测试字符串
            DateTime dateTime;
            if (TryParseDateTime(dateTimeString, out dateTime))
            {
                MessageBox.Show($"{dateTime.ToString(dateTimeString)}");
            }
            else
            {
                MessageBox.Show($"Invalid date time string");
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

    }
}
