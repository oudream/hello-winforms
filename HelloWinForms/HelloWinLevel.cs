using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HelloWinForms
{
    public partial class HelloWinLevel : Form
    {
        HistogramLineControl histogramControl;

        public HelloWinLevel()
        {
            InitializeComponent();

            histogramControl = new HistogramLineControl
            {
                Dock = DockStyle.Fill
            };
            histogramControl.NodePositionsChanged += HistogramControl_NodePositionChanged;
            panel1.Controls.Add(histogramControl);
        }

        private void HistogramControl_NodePositionChanged(string name, int min, int max, double gamma)
        {
            Console.WriteLine($"Node Position Changed: Name={name}, min={min}, max={max}, gamma={gamma}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 此处演示一个简单的直方图数据生成，实际应用中需要根据图像数据生成
            //Random random = new Random();
            //int[] histogramData = new int[256];
            //for (int i = 0; i < 256; i++)
            //{
            //    histogramData[i] = random.Next(0, 100);
            //}

            var image = HistogramLineControl.ReadTiffAsGrayscale(@"E:\tmp\CT_1.tif");

            histogramControl.Histogram = HistogramLineControl.CalculateHistogramByWidth(image, panel1.Width);
        }
    }

    /**
    public partial class Form1 : Form
    {
        private WindowWidthAndLevel.HistogramLineControl histogramControl;

        public Form1()
        {
            InitializeComponent();

            histogramControl = new WindowWidthAndLevel.HistogramLineControl
            {
                Dock = DockStyle.Fill
            };
            histogramControl.NodePositionsChanged += HistogramControl_NodePositionChanged;
            panel1.Controls.Add(histogramControl);
        }
        private void HistogramControl_NodePositionChanged(string name, int min, int max, double gamma)
        {
            Console.WriteLine($"Node Position Changed: Name={name}, min={min}, max={max}, gamma={gamma}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 此处演示一个简单的直方图数据生成，实际应用中需要根据图像数据生成
            //Random random = new Random();
            //int[] histogramData = new int[256];
            //for (int i = 0; i < 256; i++)
            //{
            //    histogramData[i] = random.Next(0, 100);
            //}
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string imagePath = Path.Combine(exePath, "CT_1.tif");
            //var image = WindowWidthAndLevel.HistogramLineControl.ReadTiffAsGrayscale(imagePath);
            var image = ReadTiffToUShortArray(imagePath);
            histogramControl.Histogram = WindowWidthAndLevel.HistogramLineControl.CalculateHistogramByWidth(image, panel1.Width);
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        public static ushort[,] ReadTiffToUShortArray(string imagePath)
        {
            // 创建 Halcon 图像对象
            HObject image;
            HOperatorSet.GenEmptyObj(out image);

            // 读取图像
            HOperatorSet.ReadImage(out image, imagePath);

            // 获取图像尺寸
            HTuple width, height;
            HOperatorSet.GetImageSize(image, out width, out height);

            // 创建 ushort 数组
            ushort[,] resultArray = new ushort[height, width];

            // 将 Halcon 图像转换为 ushort 数组
            unsafe
            {
                HTuple pointer, type, stride;
                HOperatorSet.GetImagePointer1(image, out pointer, out type, out width, out height);

                ushort* p = (ushort*)pointer.IP;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        resultArray[y, x] = p[y * width + x];
                    }
                }
            }

            // 释放图像资源
            image.Dispose();

            return resultArray;
        }
    }
    */
}
