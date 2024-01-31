using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloSNR : Form
    {
        public HelloSNR()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string imagePath = "path_to_your_image.tiff"; // 替换为您的图像路径
            Rectangle organizationRegion = new Rectangle(10, 10, 40, 40); // 组织区域
            Rectangle backgroundRegion = new Rectangle(100, 100, 40, 40); // 背景区域

            double snr = CalculateSNR(imagePath, organizationRegion, backgroundRegion);
            Console.WriteLine($"Signal-to-Noise Ratio: {snr}");
        }

        private static double CalculateSNR(string imagePath, Rectangle organizationRegion, Rectangle backgroundRegion)
        {
            using (Bitmap image = new Bitmap(imagePath))
            {
                // 计算组织区域的平均值
                double meanOrganization = CalculateMean(image, organizationRegion);

                // 计算背景区域的标准偏差
                double stdBackground = CalculateStd(image, backgroundRegion);

                // 计算并返回信噪比
                return meanOrganization / stdBackground;
            }
        }

        private static double CalculateMean(Bitmap image, Rectangle region)
        {
            double sum = 0;
            int count = 0;
            for (int y = region.Top; y < region.Bottom; y++)
            {
                for (int x = region.Left; x < region.Right; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    sum += (pixel.R + pixel.G + pixel.B) / 3.0; // 计算灰度值
                    count++;
                }
            }
            return sum / count;
        }

        private static double CalculateStd(Bitmap image, Rectangle region)
        {
            double mean = CalculateMean(image, region);
            double sumSquares = 0;
            int count = 0;
            for (int y = region.Top; y < region.Bottom; y++)
            {
                for (int x = region.Left; x < region.Right; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    double grey = (pixel.R + pixel.G + pixel.B) / 3.0;
                    sumSquares += Math.Pow(grey - mean, 2);
                    count++;
                }
            }
            return Math.Sqrt(sumSquares / count);
        }
    }
}
