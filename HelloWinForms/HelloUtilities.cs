using CxWorkStation.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HelloWinForms
{
    public partial class HelloUtilities : Form
    {
        public HelloUtilities()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string folder = @"C:\Example";
            string subfolder = "Subfolder";
            string filename = "File.txt";
            // 合并多级路径
            string combinedPath = Path.Combine(folder, subfolder, filename);
            label1.Text = combinedPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long dtNow = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            textBox1.Text = $"{dtNow}";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            double number = 123.556789;
            textBox1.Text = $"{number:0}";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string path1 = @"C:\Example\File.txt";
            string path2 = @"RelativePath\File.txt";

            label1.Text = ($"Is path '{path1}' rooted? {IsFullPath(path1)} {path2}' rooted? {IsFullPath(path2)}");
        }

        static bool IsFullPath(string path)
        {
            return Path.IsPathRooted(path);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = LoadImage(@"E:\tmp\CT_1.tif");
        }

        private static Image LoadImage(string filePath)
        {
            // 使用文件流加载图像
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // 创建图像对象
                    Image image = Image.FromStream(fs);
                    // 关闭文件流，释放文件句柄
                    fs.Close();
                    return image;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}
