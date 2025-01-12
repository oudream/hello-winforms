using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloZip : Form
    {
        public HelloZip()
        {
            InitializeComponent();
        }

        public static async Task CreateZipFileAsync(IEnumerable<string> files, string destinationZipFilePath, string baseFolder)
        {
            await Task.Run(() =>
            {
                if (File.Exists(destinationZipFilePath))
                {
                    File.Delete(destinationZipFilePath);
                }

                using (ZipArchive zip = ZipFile.Open(destinationZipFilePath, ZipArchiveMode.Create))
                {
                    foreach (var file in files)
                    {
                        // 获取文件相对于基础文件夹的相对路径
                        string relativePath = GetRelativePath(baseFolder, file);
                        // 创建具有完整目录结构的条目
                        zip.CreateEntryFromFile(file, relativePath, CompressionLevel.Fastest);
                    }
                }
            });
        }

        public static string GetRelativePath(string fromPath, string toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button == null)
            {
                return;
            }
            button.Enabled = false;
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();


                List<string> filesToZip = new List<string>
                {
                    @"E:\solder-ball-tif1\aaa\2022\Org_1AS.tif",
                    @"E:\solder-ball-tif1\aaa\2022\Org_2AS.tif",
                    @"E:\solder-ball-tif1\aaa\2033\Org_14AS.tif",
                    @"E:\solder-ball-tif1\aaa\2033\Org_15AS.tif",
                    @"E:\solder-ball-tif1\aaa\2033\Org_16AS.tif",
                    @"E:\solder-ball-tif1\aaa\2033\Org_17AS.tif",
                    @"E:\solder-ball-tif1\aaa\2033\Org_18AS.tif",
                    @"E:\solder-ball-tif1\aaa\2033\Org_19AS.tif",
                    @"E:\solder-ball-tif1\bbb\2022\Org_1AS.tif",
                    @"E:\solder-ball-tif1\bbb\2022\Org_2AS.tif",
                    @"E:\solder-ball-tif1\bbb\2033\Org_14AS.tif",
                    @"E:\solder-ball-tif1\bbb\2033\Org_15AS.tif",
                    @"E:\solder-ball-tif1\bbb\2033\Org_16AS.tif",
                    @"E:\solder-ball-tif1\bbb\2033\Org_17AS.tif",
                    @"E:\solder-ball-tif1\bbb\2033\Org_18AS.tif",
                    @"E:\solder-ball-tif1\bbb\2033\Org_19AS.tif",
                };

                string destinationZipPath = @"E:\temp\destination.zip";

                await CreateZipFileAsync(filesToZip, destinationZipPath, @"E:\solder-ball-tif");

                stopwatch.Stop();
                var costImageProcessing = stopwatch.ElapsedMilliseconds;
                richTextBox1.AppendText($"CreateZipFile Cost [{costImageProcessing}] ms\n");
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText($"An error occurred: {ex.Message}\n");
            }
            finally
            {
                button.Enabled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var dtNow = DateTime.Now;
            while (DateTime.Now - dtNow < TimeSpan.FromSeconds(3))
            {
                richTextBox1.AppendText($"CreateZipFile Cost [{DateTime.Now}] ms\n");
                richTextBox1.Invalidate();
                Thread.Sleep(1);
            }
        }
    }
}
