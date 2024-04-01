using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloOverlayImage : Form
    {
        public HelloOverlayImage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HelloOverlayImage1();
        }

        void HelloOverlayImage1()
        {
            List<string> imagePaths = new List<string>
            {
                "E:\\solder-ball-tif\\Org_14AS.tif",
                "E:\\solder-ball-tif\\Org_15AS.tif",
                //"E:\\solder-ball-tif\\Org_16AS.tif",
                //"E:\\solder-ball-tif\\Org_17AS.tif",
                "E:\\solder-ball-tif\\Org_18AS.tif",
            };

            Stopwatch stopwatch = new Stopwatch();
            List<Mat> images16bit = new List<Mat>();

            // 加载图像
            stopwatch.Start();
            foreach (var path in imagePaths)
            {
                images16bit.Add(Cv2.ImRead(path, ImreadModes.Unchanged));
            }
            stopwatch.Stop();
            long loadingTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"加载时间: {loadingTime} 毫秒");

            // 转换并叠加图像
            stopwatch.Restart();
            var averagedImage = OverlayImage(images16bit);
            stopwatch.Stop();
            long conversionAndAccumulationTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine($"转换及叠加时间: {conversionAndAccumulationTime} 毫秒");

            // 保存或显示结果
            Cv2.ImWrite("averagedImage.tif", averagedImage);
        }

        static Mat OverlayImage(List<Mat> images16bit)
        {
            if (images16bit == null || images16bit.Count == 0)
            {
                throw new ArgumentException("图像列表为空或未提供图像。");
            }

            // 获取第一张图像的尺寸和深度作为参考
            var referenceSize = images16bit[0].Size();
            int referenceDepth = images16bit[0].Depth();

            // 检查所有图像是否具有相同的尺寸和深度
            foreach (var img in images16bit)
            {
                if (img.Size() != referenceSize || img.Depth() != referenceDepth)
                {
                    throw new ArgumentException("所有图像必须具有相同的尺寸和深度。");
                }
            }

            Mat accumulatedImage = new Mat(images16bit[0].Rows, images16bit[0].Cols, MatType.CV_32F);
            foreach (var img in images16bit)
            {
                Mat img32f = new Mat();
                img.ConvertTo(img32f, MatType.CV_32F); // 转换为32位浮点
                Cv2.Add(accumulatedImage, img32f, accumulatedImage);
            }

            // 计算平均值
            Cv2.Divide(accumulatedImage, images16bit.Count, accumulatedImage);

            // 将平均后的图像转换回16位以查看或保存
            Mat averagedImage = new Mat();
            accumulatedImage.ConvertTo(averagedImage, MatType.CV_16U);
            return averagedImage;
        }

    }
}
