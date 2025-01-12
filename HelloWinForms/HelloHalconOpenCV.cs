using CxAAC.Utilities;
using HalconDotNet;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloHalconOpenCV : Form
    {
        public HelloHalconOpenCV()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 打开文件对话框
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "图像文件|*.bmp;*.jpg;*.png;*.tiff;*.jpeg|所有文件|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;

                    //for (int i = 0; i < 1000; i++)
                    //{
                    //    try
                    //    {
                    //        // 读取图像到 Halcon 的 HObject
                    //        Stopwatch stopwatch = Stopwatch.StartNew();
                    //        HOperatorSet.ReadImage(out var ho_Image, filePath);
                    //        HOperatorSet.Emphasize(ho_Image, out var newImage, 7, 7, 1);
                    //        stopwatch.Stop();
                    //        var diffToRead = stopwatch.ElapsedMilliseconds;
                    //        Console.WriteLine($"耗时 - diffToRead:{diffToRead} ms");

                    //        newImage.Dispose();
                    //        newImage = null;
                    //        ho_Image.Dispose();
                    //        ho_Image = null;

                    //        Thread.Sleep(10);
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        MessageBox.Show($"加载图片失败：{ex.Message}");
                    //    }
                    //}
                    //return;
                    for (int i = 0; i < 10000; i++)
                    {
                        try
                        {
                            // 读取图像到 Halcon 的 HObject
                            HOperatorSet.ReadImage(out var ho_Image, filePath);
                            Stopwatch stopwatch = Stopwatch.StartNew();

                            // 转换为 OpenCV Mat
                            Mat matImage = HObjectToMat(ho_Image);
                            stopwatch.Stop();
                            var diffToMat = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                            var displayImage = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(matImage);

                            stopwatch.Stop();
                            var diffToBitmap = stopwatch.ElapsedMilliseconds;
                            stopwatch.Restart();

                            // 显示到 PictureBox
                            pictureBox1.Image?.Dispose();
                            pictureBox1.Image = displayImage;
                            stopwatch.Stop();
                            var diffToShow = stopwatch.ElapsedMilliseconds;

                            Console.WriteLine($"耗时 - diffToMat:{diffToMat} ms diffToBitmap:{diffToBitmap} ms diffToShow:{diffToShow} ms");

                            matImage.Dispose();
                            ho_Image.Dispose();
                            ho_Image = null;
                            matImage = null;
                            Application.DoEvents();
                            Thread.Sleep(100);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"加载图片失败：{ex.Message}");
                        }
                    }
                }
            }
        }

        private Mat HObjectToMat(HObject hObject)
        {
            // 检查图像是否是多通道
            HTuple channels;
            HOperatorSet.CountChannels(hObject, out channels);

            if (channels == 1)
            {
                // 单通道图像处理
                HOperatorSet.GetImagePointer1(hObject, out var pointer, out var type, out var width, out var height);

                // 创建 Mat 对象
                Mat mat = new Mat(height.I, width.I, MatType.CV_8UC1, pointer.IP);
                return mat; // 克隆 Mat 以确保独立性
            }
            else if (channels == 3)
            {
                // 三通道图像处理
                HObject ho_R, ho_G, ho_B;
                HOperatorSet.Decompose3(hObject, out ho_R, out ho_G, out ho_B);

                // 将每个通道转换为 Mat
                Mat matR = HObjectToMat(ho_R);
                Mat matG = HObjectToMat(ho_G);
                Mat matB = HObjectToMat(ho_B);

                // 合并通道为 BGR 顺序
                Mat matRGB = new Mat();
                Cv2.Merge(new[] { matB, matG, matR }, matRGB);

                ho_R.Dispose();
                ho_G.Dispose();
                ho_B.Dispose();
                
                return matRGB;
            }
            else
            {
                throw new NotSupportedException("当前仅支持单通道和三通道图像");
            }
        }
    }
}
