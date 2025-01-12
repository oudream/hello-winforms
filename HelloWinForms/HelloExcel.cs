using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloExcel : Form
    {
        public HelloExcel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 设置 EPPlus 的 LicenseContext（非商业用途）
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            // 模板路径
            string templatePath = "template.xlsx";
            // 输出文件路径
            string outputPath = "output.xlsx";

            // 检查模板文件是否存在
            if (!File.Exists(templatePath))
            {
                Console.WriteLine("模板文件不存在！");
                return;
            }

            // 打开文件对话框选择图片
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "选择要插入的图片",
                Filter = "图片文件|*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures)
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string imagePath = openFileDialog.FileName;

                // 检查文件扩展名是否为图片格式
                string[] supportedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
                if (Array.IndexOf(supportedExtensions, Path.GetExtension(imagePath).ToLower()) == -1)
                {
                    Console.WriteLine("选择的文件不是支持的图片格式！");
                    return;
                }

                // 加载 Excel 模板
                FileInfo fileInfo = new FileInfo(templatePath);
                using (ExcelPackage package = new ExcelPackage(fileInfo))
                {
                    // 获取工作表（假设是第一个工作表）
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    // 使用 MemoryStream 读取图片
                    using (Image image = Image.FromFile(imagePath))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, image.RawFormat); // 将图片保存到 MemoryStream
                            ms.Seek(0, SeekOrigin.Begin);    // 重置流位置

                            // 在工作表中插入图片
                            ExcelPicture excelPicture = worksheet.Drawings.AddPicture("SelectedImage", ms);

                            // 设置图片位置（行列）
                            excelPicture.SetPosition(1, 0, 1, 0); // 行1，列1（索引从0开始）

                            // 设置图片大小（可选）
                            excelPicture.SetSize(1024, 1024); // 宽 150px，高 150px
                        }
                    }

                    // 保存到输出文件
                    package.SaveAs(new FileInfo(outputPath));
                    Console.WriteLine($"图片已插入并保存到：{outputPath}");
                }
            }
            else
            {
                Console.WriteLine("未选择图片，操作已取消。");
            }
        }
    }
}
