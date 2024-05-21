using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class ImagePng8 : Form
    {
        public ImagePng8()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 读取图像（此处将其转换为 BGR 格式的 OpenCV 图像）
            var src = Cv2.ImRead(@"C:\Users\Administrator\Desktop\CxSolderBall\20240428_192548--1-333-1-Front.jpg", ImreadModes.Color);

            // 将图像转换为 Bitmap
            Bitmap bmp = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(src);

            // 计算图像转换所需的时间
            Stopwatch stopwatch = Stopwatch.StartNew();

            // 将图像转换为 PNG-8 格式
            Bitmap png8 = ConvertToPng8(bmp);

            // 保存为 PNG-8 格式
            png8.Save("output_image.png", ImageFormat.Png);

            stopwatch.Stop();
            var costImageDetection = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("图像转换耗时：" + costImageDetection + "ms");
        }

        // 将 Bitmap 图像转换为 8 位索引的 PNG-8 格式
        static Bitmap ConvertToPng8_(Bitmap image)
        {
            // 创建一个 8 位每像素的 Bitmap
            Bitmap newBitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);

            // 设置简单彩色的 256 色调色板
            ColorPalette palette = newBitmap.Palette;
            var colorPalette = GenerateSimpleColorPalette();
            for (int i = 0; i < colorPalette.Length; i++)
            {
                palette.Entries[i] = colorPalette[i];
            }
            newBitmap.Palette = palette;

            // 锁定目标 Bitmap 的像素以进行写入
            BitmapData bmpData = newBitmap.LockBits(
                new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            // 创建索引图像数据
            int bytes = Math.Abs(bmpData.Stride) * bmpData.Height;
            byte[] pixelValues = new byte[bytes];
            int stride = bmpData.Stride;

            //for (int y = 0; y < image.Height; y++)
            //{
            //    for (int x = 0; x < image.Width; x++)
            //    {
            //        // 获取当前像素的 RGB 值并映射到调色板索引
            //        Color pixelColor = image.GetPixel(x, y);
            //        byte index = MapColorToPaletteIndex(pixelColor);
            //        pixelValues[y * stride + x] = index;
            //    }
            //}

            Parallel.For(0, image.Height, y =>
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // 获取当前像素的 RGB 值并映射到调色板索引
                    Color pixelColor = image.GetPixel(x, y);
                    byte index = MapColorToPaletteIndex(pixelColor);
                    pixelValues[y * stride + x] = index;
                }
            });


            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, bytes);
            newBitmap.UnlockBits(bmpData);

            return newBitmap;
        }

        // 将 Bitmap 图像转换为 8 位索引的 PNG-8 格式
        static Bitmap ConvertToPng8(Bitmap image)
        {
            // 创建一个 8 位每像素的 Bitmap
            Bitmap newBitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format8bppIndexed);

            // 设置简单彩色的 256 色调色板
            ColorPalette palette = newBitmap.Palette;
            var colorPalette = GenerateSimpleColorPalette();
            for (int i = 0; i < colorPalette.Length; i++)
            {
                palette.Entries[i] = colorPalette[i];
            }
            newBitmap.Palette = palette;

            // 锁定目标 Bitmap 的像素以进行写入
            BitmapData bmpData = newBitmap.LockBits(
                new Rectangle(0, 0, newBitmap.Width, newBitmap.Height),
                ImageLockMode.WriteOnly,
                PixelFormat.Format8bppIndexed);

            // 获取源图像的像素数据
            BitmapData srcData = image.LockBits(
                new Rectangle(0, 0, image.Width, image.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format24bppRgb);

            int srcStride = srcData.Stride;
            int dstStride = bmpData.Stride;
            int width = image.Width;
            int height = image.Height;

            // 创建索引图像数据
            byte[] pixelValues = new byte[Math.Abs(dstStride) * height];

            // 使用 Parallel.For 并行处理每一行像素
            Parallel.For(0, height, y =>
            {
                IntPtr srcRowPtr = srcData.Scan0 + y * srcStride;
                byte[] srcRow = new byte[width * 3];
                System.Runtime.InteropServices.Marshal.Copy(srcRowPtr, srcRow, 0, srcRow.Length);

                for (int x = 0; x < width; x++)
                {
                    byte b = srcRow[x * 3];
                    byte g = srcRow[x * 3 + 1];
                    byte r = srcRow[x * 3 + 2];
                    Color pixelColor = Color.FromArgb(r, g, b);
                    byte index = MapColorToPaletteIndex(pixelColor);
                    pixelValues[y * dstStride + x] = index;
                }
            });

            System.Runtime.InteropServices.Marshal.Copy(pixelValues, 0, bmpData.Scan0, pixelValues.Length);
            newBitmap.UnlockBits(bmpData);
            image.UnlockBits(srcData);

            return newBitmap;
        }

        // 将 RGB 颜色简单映射到调色板索引
        static byte MapColorToPaletteIndex(Color color)
        {
            // 简单的 RGB -> 索引映射
            // 使用最简单的映射方式：
            int r = (color.R >> 5) & 0x07; // 3 bits
            int g = (color.G >> 5) & 0x07; // 3 bits
            int b = (color.B >> 6) & 0x03; // 2 bits
            return (byte)((r << 5) | (g << 2) | b);
        }

        // 生成简单的 256 色彩色调色板
        static Color[] GenerateSimpleColorPalette()
        {
            Color[] palette = new Color[256];

            // 生成调色板
            for (int r = 0; r < 8; r++) // 3 bits
            {
                for (int g = 0; g < 8; g++) // 3 bits
                {
                    for (int b = 0; b < 4; b++) // 2 bits
                    {
                        int index = (r << 5) | (g << 2) | b;
                        palette[index] = Color.FromArgb(
                            (r << 5) | (r << 2) | (r >> 1), // 映射到 0 - 255
                            (g << 5) | (g << 2) | (g >> 1), // 映射到 0 - 255
                            (b << 6) | (b << 4) | (b << 2) | b); // 映射到 0 - 255
                    }
                }
            }

            return palette;
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            var src = Cv2.ImRead(@"C:\Users\Administrator\Desktop\CxSolderBall\20240428_192548--1-333-1-Front.jpg", ImreadModes.Color);
            Cv2.ImWrite(@"E:\image9\2.jpg", src);
        }
    }
}
