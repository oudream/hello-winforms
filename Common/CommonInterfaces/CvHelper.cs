using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;

namespace CommonInterfaces
{
    public static class CvHelper
    {
        /// <summary>
        /// 将16位灰度图像的ushort数组转换为TIF格式的Base64字符串
        /// </summary>
        /// <param name="grayScaleData">16位灰度图像数据</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>TIF格式图像的Base64编码字符串</returns>
        public static string ConvertToBase64(Mat image, string ext)
        {
            if (image == null || image.Empty())
            {
                return string.Empty;
            }
            
            // 将Mat对象编码 ext 格式的字节流
            byte[] buf;
            // ext: ".tif"表示TIF格式；".png"表示PNG格式
            Cv2.ImEncode(ext, image, out buf);

            // 将字节流转换为Base64字符串
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// 将16位灰度图像的ushort数组转换为TIF格式的Base64字符串
        /// </summary>
        /// <param name="grayScaleData">16位灰度图像数据</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>TIF格式图像的Base64编码字符串</returns>
        public static string ConvertToBase64(ushort[,] grayScaleData, int width, int height, string ext)
        {
            // 创建一个临时的Mat对象来存储图像数据
            using (Mat mat = new Mat(height, width, MatType.CV_16U, grayScaleData))
            {
                // 将Mat对象编码 ext 格式的字节流
                byte[] buf;
                // ext: ".tif"表示TIF格式；".png"表示PNG格式
                Cv2.ImEncode(ext, mat, out buf);

                // 将字节流转换为Base64字符串
                return Convert.ToBase64String(buf);
            }
        }

        /// <summary>
        /// 将16位灰度图像的byte数组转换为TIF格式的Base64字符串
        /// </summary>
        /// <param name="grayScaleData">16位灰度图像数据（byte数组）</param>
        /// <param name="width">图像宽度</param>
        /// <param name="height">图像高度</param>
        /// <returns>TIF格式图像的Base64编码字符串</returns>
        public static string ConvertToBase64(byte[] grayScaleData, int width, int height, string ext)
        {
            // 确保输入的byte数组长度与16位图像数据的大小匹配
            if (grayScaleData == null || grayScaleData.Length == 0 || grayScaleData.Length != width * height * 2)
            {
                return "";
                //throw new ArgumentException("The size of the grayScaleData does not match the expected size based on width and height.");
            }

            // 使用byte数组创建一个Mat对象
            using (Mat mat = new Mat(height, width, MatType.CV_16U, grayScaleData))
            {
                // 将Mat对象编码 ext 格式的字节流
                byte[] buf;
                // ext: ".tif"表示TIF格式；".png"表示PNG格式
                Cv2.ImEncode(ext, mat, out buf);

                // 将字节流转换为Base64字符串
                return Convert.ToBase64String(buf);
            }
        }

        /// <summary>
        /// 将Base64编码的字符串转换为Mat图像对象。
        /// </summary>
        /// <param name="base64String">表示图像的Base64编码字符串。</param>
        /// <returns>表示解码图像的Mat对象。</returns>
        public static Mat ConvertBase64ToMat(string base64String)
        {
            // 将Base64字符串解码为字节数组。
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // 将字节数组解码为Mat对象。假设图像以标准格式（如PNG或TIF）编码。
            Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Unchanged);

            return image;
        }
        public static Mat ConvertBase64ToMat(byte[] imageBytes)
        {
            // 将字节数组解码为Mat对象。假设图像以标准格式（如PNG或TIF）编码。
            Mat image = Cv2.ImDecode(imageBytes, ImreadModes.Unchanged);

            return image;
        }

        public static Mat ConvertBase64ToMat(byte[] imageBytes, int rows, int cols, int channels)
        {
            // 将字节数组解码为Mat对象。假设图像以标准格式（如PNG或TIF）编码。
            MatType matType = MatType.CV_8UC1;
            if (channels == 3) matType = MatType.CV_8UC3;
            Mat image = new Mat(rows, cols, matType, imageBytes);
            return image;
        }

        // 计算：平均强度、标准偏差。类似halcon HOperatorSet.Intensity
        public static (double, double) CalcMeanStdDev(Mat image)
        {
            if (image == null || image.Empty())
            {
                return (0, 0);
            }

            // 定义你感兴趣的区域，image全部
            Rect roi = new Rect(0, 0, image.Width, image.Height);

            // 使用ROI创建图像的子集
            Mat imageRoi = new Mat(image, roi);

            // 准备变量来接收计算结果
            Scalar mean, stddev;
            Cv2.MeanStdDev(imageRoi, out mean, out stddev);

            // 输出计算结果
            //Console.WriteLine($"平均强度: {mean[0]}, 标准偏差: {stddev[0]}");
            return (mean[0], stddev[0]);
        }

        // 计算图像的最小和最大灰度值
        public static (double, double) CalcMinMaxGray(Mat image)
        {
            if (image == null || image.Empty())
            {
                return (0, 0);
            }

            // 定义你感兴趣的区域(Rectangle)，例如：
            Rect roi = new Rect(0, 0, image.Width, image.Height);

            // 使用ROI创建图像的子集
            Mat imageRoi = new Mat(image, roi);

            // 准备变量来接收计算结果
            double minGray, maxGray;
            Point minLoc, maxLoc;

            // 计算最小和最大灰度值及其位置
            Cv2.MinMaxLoc(imageRoi, out minGray, out maxGray, out minLoc, out maxLoc);

            // 灰度范围可以通过最大值减去最小值获得
            //double rangeGray = maxGray - minGray;

            // 输出计算结果
            //Console.WriteLine($"最小灰度值: {minGray}, 最大灰度值: {maxGray}, 灰度范围: {rangeGray}");
            return (minGray, maxGray);
        }

        // 判断 image 可不可以做窗宽窗位：是不是16位单通道图像
        public static bool CanWLWW(Mat image)
        {
            if (image == null || image.Empty())
            {
                return false;
            }
            if (image.Type() != MatType.CV_16UC1)
            {
                return false;
            }
            return true;
        }

        // 根据 image 计算窗宽窗位
        public static bool WLWWAuto(Mat origImage, out int wl, out int ww, Rect? roi = null)
        {
            wl = 0;
            ww = 0;
            if (origImage == null || origImage.Empty()) return false;

            // 假设已经通过某种方式获取了 ROI 的坐标
            // 这里直接定义一个示例 ROI
            // 如果未指定 ROI 或 ROI 为空，则使用整个图像作为 ROI
            Rect effectiveRoi = roi ?? new Rect(0, 0, origImage.Width, origImage.Height);

            // 检查 ROI 是否超出原始图像的边界
            if (effectiveRoi.X < 0 || effectiveRoi.Y < 0 ||
                effectiveRoi.X + effectiveRoi.Width > origImage.Width ||
                effectiveRoi.Y + effectiveRoi.Height > origImage.Height)
            {
                return false;
            }

            // 使用 ROI 创建图像的子集
            Mat imageRoi = new Mat(origImage, effectiveRoi);

            // 计算平均强度和标准偏差
            Scalar mean, stddev;
            Cv2.MeanStdDev(imageRoi, out mean, out stddev);

            // 计算最小和最大灰度值
            double minGray, maxGray;
            Cv2.MinMaxLoc(imageRoi, out minGray, out maxGray);

            // 根据计算结果确定 WL 和 WW 的值
            wl = (int)mean.Val0;
            int min1 = (int)(mean.Val0 - minGray);
            int max1 = (int)(maxGray - mean.Val0);
            ww = Math.Max(min1, max1);

            return true;
        }

        public static bool WLWWAuto3(Mat origImage, out int wl, out int ww)
        {
            // 初始化输出参数
            wl = 0;
            ww = 0;

            // 检查输入图像是否为16位单通道图像
            if (origImage.Empty() || origImage.Type() != MatType.CV_16UC1)
            {
                return false;  // 图像不符合预期，返回 false
            }

            // 计算直方图
            Mat hist = new Mat();
            int histSize = 65536; // 对于16位图像
            Rangef range = new Rangef(0, 65536);
            Cv2.CalcHist(
                new Mat[] { origImage },
                new int[] { 0 },
                null,
                hist,
                1,
                new int[] { histSize },
                new Rangef[] { range }
            );

            // 手动计算累积分布函数 (CDF)
            double[] cdf = new double[histSize];
            cdf[0] = hist.Get<float>(0);
            for (int i = 1; i < histSize; i++)
            {
                cdf[i] = cdf[i - 1] + hist.Get<float>(i);
            }
            double total = cdf[histSize - 1]; // 总像素数

            // 使用累积分布函数找到合适的最小和最大灰度值
            double minGray = FindPercentile(cdf, 0.00195, total);  // 5% 百分位数
            double maxGray = FindPercentile(cdf, 0.99805, total);  // 95% 百分位数

            // 计算窗宽和窗位
            ww = (int)(maxGray - minGray);
            wl = (int)(minGray + ww / 2);

            return true;  // 成功设置窗宽和窗位，返回 true
        }

        private static double FindPercentile(double[] cdf, double percentile, double total)
        {
            double target = percentile * total;
            for (int i = 0; i < cdf.Length; i++)
            {
                if (cdf[i] >= target)
                    return i;
            }
            return 0;
        }


        // 根据窗宽窗位进行图像调整
        public static Mat WLWWTransferWithParallel(Mat image, int wl, int ww)
        {
            if (image == null || image.Empty() || wl == 0 || ww == 0)
            {
                return null;
            }

            // 确保输入图像是单通道16位图像
            if (image.Type() != MatType.CV_16U)
            {
                throw new ArgumentException("图像必须是16位单通道图像。");
            }

            int width = image.Width;
            int height = image.Height;
            var adjustedImage = new Mat(height, width, MatType.CV_8U);

            // 计算窗宽窗位的最小和最大值
            double min = wl - ww / 2.0;
            double max = wl + ww / 2.0;

            // 使用并行处理进行图像调整
            Parallel.For(0, height, y =>
            {
                for (int x = 0; x < width; x++)
                {
                    ushort pixelValue = image.At<ushort>(y, x);
                    byte adjustedValue;
                    if (pixelValue < min) adjustedValue = 0;
                    else if (pixelValue > max) adjustedValue = 255;
                    else adjustedValue = (byte)((pixelValue - min) / ww * 255);
                    adjustedImage.Set(y, x, adjustedValue);
                }
            });

            return adjustedImage;
        }

        // 拉满窗宽窗位进行图像调整
        public static Mat WLWWTransferWithFull(Mat image)
        {
            var dst = new Mat();
            // 对图像进行归一化处理，MinMax相当于把窗宽窗位拉满
            Cv2.Normalize(image, dst, 0, 255, NormTypes.MinMax, MatType.CV_8UC1);
            return dst;
        }

        // 对图像进行叠加
        public static Mat OverlayImage(List<Mat> images16bit)
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
                img32f.Dispose();
            }

            // 计算平均值
            Cv2.Divide(accumulatedImage, images16bit.Count, accumulatedImage);

            // 将平均后的图像转换回16位以查看或保存
            Mat averagedImage = new Mat();
            accumulatedImage.ConvertTo(averagedImage, MatType.CV_16U);
            accumulatedImage.Dispose();
            return averagedImage;
        }

        // 将图像缩放到指定宽度
        public static Mat ResizeImage(Mat image, int newWidth)
        {
            // Assuming 'image' is your original Mat image
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            // Calculate the new height to maintain the aspect ratio
            int newHeight = (int)((originalHeight / (double)originalWidth) * newWidth);

            // Resize the image to the new dimensions
            return image.Resize(new Size(newWidth, newHeight));
        }

        // 旋转
        public static Mat Rotate(Mat src, RotateFlags rotateFlags = RotateFlags.Rotate90Clockwise)
        {
            Mat dst = new Mat();
            Cv2.Rotate(src, dst, rotateFlags);
            return dst;
        }

        // 镜像
        // Horizontally: FlipMode.X
        // Vertically: FlipMode.Y
        public static Mat Mirror(Mat src, FlipMode flipMode = FlipMode.X)
        {
            Mat dst = new Mat();
            Cv2.Flip(src, dst, flipMode);
            return dst;
        }

        // 旋转 + 镜像
        public static Mat RotateThenMirror(Mat src, RotateFlags rotateFlags = RotateFlags.Rotate90Clockwise, FlipMode flipMode = FlipMode.X)
        {
            // 首先旋转 90 度
            Mat rotated = new Mat();
            Cv2.Rotate(src, rotated, rotateFlags);

            // 然后水平镜像
            Mat mirrored = new Mat();
            Cv2.Flip(rotated, mirrored, flipMode);

            // 释放旋转后的中间图像
            rotated.Dispose();

            return mirrored;
        }

    }

    public static class CvHelperTest
    {
        static void ConvertToBase64Tif()
        {
            // 示例：创建一个16位灰度图像的ushort数组
            int width = 640; // 图像宽度
            int height = 480; // 图像高度
            ushort[,] grayScaleData = new ushort[width, height];
            // 填充你的数据到grayScaleData中

            // 调用函数并获取结果
            string base64Tif = CvHelper.ConvertToBase64(grayScaleData, width, height, ".tif");

            // 打印或使用Base64字符串
            Console.WriteLine(base64Tif);
        }

        static void ConvertToBase64TifFromBytes()
        {
            // 示例：创建一个16位灰度图像的byte数组
            int width = 640; // 图像宽度
            int height = 480; // 图像高度
            byte[] grayScaleData = new byte[width * height * 2];
            // 填充你的数据到grayScaleData中

            // 调用函数并获取结果
            string base64Tif = CvHelper.ConvertToBase64(grayScaleData, width, height, ".tif");

            // 打印或使用Base64字符串
            Console.WriteLine(base64Tif);
        }

        static void OverlayImage1()
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
            try
            {
                stopwatch.Restart();
                var averagedImage = CvHelper.OverlayImage(images16bit);
                stopwatch.Stop();
                long conversionAndAccumulationTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"转换及叠加时间: {conversionAndAccumulationTime} 毫秒");

                // 保存或显示结果
                Cv2.ImWrite("averagedImage.png", averagedImage);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"对图像进行叠加时产生错误，信息：{ex.Message}");
            }
        }

        public static Mat RebuildMat(IntPtr dataStart, int width, int height, int channels, int elemSize)
        {
            // 假设图像是8位三通道（RGB图像）
            //int channels = 3;
            //int elemSize = 1; // 8位 = 1字节

            // 计算图像的步幅
            int stride = width * channels * elemSize;

            // 从指针创建一个新的Mat对象
            Mat newImage = new Mat(height, width, MatType.CV_8UC3, dataStart, stride);

            return newImage;
        }
    }
}
