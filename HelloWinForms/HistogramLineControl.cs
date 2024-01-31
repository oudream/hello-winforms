using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace HelloWinForms
{
    public class HistogramLineControl : Control
    {
        public static ushort[,] ReadTiffAsGrayscale(string filePath)
        {
            try
            {
                // Open a Stream and decode a TIFF image.
                Stream imageStreamSource = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var decoder = new TiffBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
                BitmapSource bitmapSource = decoder.Frames[0];
                Int32 PixelHeight = bitmapSource.PixelHeight; // 图像高度
                Int32 PixelWidth = bitmapSource.PixelWidth;   // 图像宽度
                Int32 Stride = (PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;  // 跨距宽度
                if (bitmapSource.Format.BitsPerPixel == 8)
                {
                    //Byte[] gray8 = new Byte[PixelHeight * PixelWidth];//8bit 位深
                    //bitmapSource.CopyPixels(gray8, Stride, 0);
                    return null;
                }
                else if (bitmapSource.Format.BitsPerPixel == 16)
                {
                    ushort[] gray16 = new ushort[PixelWidth * PixelHeight];//16bit 位深
                    bitmapSource.CopyPixels(gray16, Stride, 0);
                    ushort[,] gray16_2D = new ushort[PixelWidth, PixelHeight];
                    for (int i = 0; i < PixelWidth; i++)
                    {
                        for (int j = 0; j < PixelHeight; j++)
                        {
                            int index = i * PixelHeight + j;
                            gray16_2D[i, j] = gray16[index];
                        }
                    }
                    return gray16_2D;
                    //ushort[,] gray16 = new ushort[PixelHeight, PixelWidth]; // 16bit 位深

                    // 复制像素值到数组
                    //bitmapSource.CopyPixels(new Int32Rect(0, 0, PixelWidth, PixelHeight), gray16, PixelWidth * sizeof(ushort), 0);
                    //return gray16;
                }
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static int[] CalculateHistogramByWidth(ushort[,] image, int windowWidth)
        {
            int imageWidth = image.GetLength(0);
            int imageHeight = image.GetLength(1);

            int[] histogram = new int[windowWidth];

            // 遍历图像像素
            for (int i = 0; i < imageWidth; i++)
            {
                for (int j = 0; j < imageHeight; j++)
                {
                    // 将 pixelValue 映射到 [0, windowWidth-1] 范围内
                    int mappedPixelValue = MapPixelValue(image[i, j], windowWidth);

                    // 更新直方图
                    histogram[mappedPixelValue]++;
                }
            }

            return histogram;
        }

        private static int MapPixelValue(ushort pixelValue, int windowWidth)
        {
            // 将 pixelValue 映射到 [0, windowWidth-1] 范围内
            int mappedValue = (int)((double)pixelValue / ushort.MaxValue * (windowWidth - 1));

            // 确保映射值在合理范围内
            mappedValue = Math.Max(0, Math.Min(mappedValue, windowWidth - 1));

            return mappedValue;
        }

        private static double CalculateDistance(System.Drawing.Point point1, System.Drawing.Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        public event Action<string, int, int, double> NodePositionsChanged;

        private int[] histogram;

        private System.Drawing.Point _minNodePosition;
        private System.Drawing.Point _middleNodePosition;
        private System.Drawing.Point _maxNodePosition;

        private int _minFix;
        private int _maxFix;


        private bool isDraggingMinNode = false;
        private bool isDraggingMiddleNode = false;
        private bool isDraggingMaxNode = false;

        public HistogramLineControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.White;
            //
            _minNodePosition = new System.Drawing.Point(0, Height / 2);
            _middleNodePosition = new System.Drawing.Point(128, 128);
            _maxNodePosition = new System.Drawing.Point(Width, Height / 2);

        }

        public int[] Histogram
        {
            get { return histogram; }
            set
            {
                histogram = value;
                {
                    // 找到直方图中第一个非零元素的索引
                    _minFix = Array.FindIndex(histogram, v => v != 0);

                    _minNodePosition.X = _minFix;
                    // 如果直方图中所有元素都是零，将 _minNodePosition.X 设置为 0
                    if (_minNodePosition.X == -1)
                    {
                        _minNodePosition.X = 0;
                    }
                }
                {
                    // 找到直方图中最后一个非零元素的索引
                    _maxFix = Array.FindLastIndex(histogram, v => v != 0);
                    _maxNodePosition.X = _maxFix;
                    // 如果直方图中所有元素都是零，将 _maxNodePosition.X 设置为 windowWidth - 1
                    if (_maxNodePosition.X == -1)
                    {
                        _maxNodePosition.X = Width - 1;
                    }
                }
                Invalidate();
            }
        }

        public int Min
        {
            get { return XToGray(_minNodePosition.X); }
            set
            {
                // 进行逆向映射
                int minX = GrayToX(value);

                // 影响到 _minNodePosition.X 的赋值
                _minNodePosition.X = Math.Max(0, Math.Min(minX, Width - 1));

                Invalidate();
            }
        }

        public int Max
        {
            get { return XToGray(_maxNodePosition.X); }
            set
            {
                // 进行逆向映射
                int maxX = GrayToX(value);

                // 影响到 _maxNodePosition.X 的赋值
                _maxNodePosition.X = Math.Max(0, Math.Min(maxX, Width - 1));

                Invalidate();
            }
        }

        public double Gamma
        {
            get { return CalculateGamma(); }
            set
            {
                // 影响到 _middleNodePosition.X 的赋值
                _middleNodePosition = CalculateMiddleNodePosition(value);

                Invalidate();
            }
        }

        private int GrayToX(int grayValue)
        {
            // 根据需要的映射规则进行实现
            // 这里简单地演示一个线性映射，你可能需要更复杂的映射规则
            return (int)((double)grayValue / 65535 * Width);
        }

        private int XToGray(int x)
        {
            // 根据需要的映射规则进行实现
            // 这里简单地演示一个线性映射，你可能需要更复杂的映射规则
            return (int)((double)x / Width * 65535);
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (histogram == null)
                return;

            Graphics g = e.Graphics;

            int barWidth = Width / histogram.Length;

            // 绘制直方图
            for (int i = 0; i < histogram.Length; i++)
            {
                int barHeight = histogram[i] * Height / histogram.Max();
                Rectangle barRect = new Rectangle(i * barWidth, Height - barHeight, barWidth, barHeight);
                g.FillRectangle(Brushes.Blue, barRect);
                g.DrawRectangle(Pens.Black, barRect); // Optional: Draw borders for each bar
            }

            // 绘制折线路径
            Pen linePen = new Pen(Color.Red, 2);

            // 左下角到 minNodePosition
            // g.DrawLine(linePen, 0, Height, minNodePosition.X, minNodePosition.Y);

            // minNodePosition 到 middleNodePosition
            g.DrawLine(linePen, _minNodePosition.X, _minNodePosition.Y, _middleNodePosition.X, _middleNodePosition.Y);

            // middleNodePosition 到 maxNodePosition
            g.DrawLine(linePen, _middleNodePosition.X, _middleNodePosition.Y, _maxNodePosition.X, _maxNodePosition.Y);

            // maxNodePosition 到 右下角
            // g.DrawLine(linePen, maxNodePosition.X, maxNodePosition.Y, Width, Height);

            // 绘制小圆节点
            int nodeSize = 10;
            g.FillEllipse(Brushes.Green, _minNodePosition.X - nodeSize / 2, _minNodePosition.Y - nodeSize / 2, nodeSize, nodeSize);
            g.FillEllipse(Brushes.Green, _middleNodePosition.X - nodeSize / 2, _middleNodePosition.Y - nodeSize / 2, nodeSize, nodeSize);
            g.FillEllipse(Brushes.Green, _maxNodePosition.X - nodeSize / 2, _maxNodePosition.Y - nodeSize / 2, nodeSize, nodeSize);

            linePen.Dispose();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            // 判断鼠标点击在哪个节点上
            if (Math.Abs(e.X - _minNodePosition.X) < 10 && Math.Abs(e.Y - _minNodePosition.Y) < 10)
            {
                isDraggingMinNode = true;
            }
            else if (Math.Abs(e.X - _middleNodePosition.X) < 10 && Math.Abs(e.Y - _middleNodePosition.Y) < 10)
            {
                isDraggingMiddleNode = true;
            }
            else if (Math.Abs(e.X - _maxNodePosition.X) < 10 && Math.Abs(e.Y - _maxNodePosition.Y) < 10)
            {
                isDraggingMaxNode = true;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            // 如果鼠标正在拖动节点，则更新节点的位置
            if (isDraggingMinNode)
            {
                // 计算新的 X 位置，确保它不超过 _middleNodePosition.X
                //int newX = Math.Min(e.X, _middleNodePosition.X);
                int newX = Math.Max(_minFix, Math.Min(e.X, _middleNodePosition.X));
                int newY = Math.Max(0, Math.Min(e.Y, Height));

                // 限制在底部移动
                // _minNodePosition = new System.Drawing.Point(newX, Height);
                _minNodePosition = new System.Drawing.Point(newX, newY);

                OnNodePositionsChanged("min", _minNodePosition.X, _maxNodePosition.X, _middleNodePosition.Y);

                Invalidate(); // 重新绘制以显示新的节点位置
            }
            else if (isDraggingMiddleNode)
            {
                // 计算新的 X 位置，确保它保持在范围内
                int newX = Math.Max(_minNodePosition.X, Math.Min(e.X, _maxNodePosition.X));
                // 确保 middleNode 不超出控件边界
                int newY = Math.Max(0, Math.Min(e.Y, Height));

                // 更新中间节点的 Y 位置
                _middleNodePosition = new System.Drawing.Point(newX, newY);

                // 通知节点位置更改
                OnNodePositionsChanged("gamma", _minNodePosition.X, _maxNodePosition.X, _middleNodePosition.Y);

                // 重绘控件
                Invalidate();
            }
            else if (isDraggingMaxNode)
            {
                //int newX = Math.Max(_middleNodePosition.X, e.X);

                //_maxNodePosition = new System.Drawing.Point(newX, e.Y);
                // 计算新的 X 位置，确保它不超过 _maxFix
                int newX = Math.Min(_maxFix, Math.Max(_middleNodePosition.X, e.X));
                int newY = Math.Max(0, Math.Min(e.Y, Height));

                // 限制在底部移动
                // _maxNodePosition = new System.Drawing.Point(newX, Height);
                _maxNodePosition = new System.Drawing.Point(newX, newY);

                OnNodePositionsChanged("max", _minNodePosition.X, _maxNodePosition.X, _middleNodePosition.Y);

                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isDraggingMinNode = false;
            isDraggingMiddleNode = false;
            isDraggingMaxNode = false;
        }


        protected virtual void OnNodePositionsChanged(string name, int minX, int maxX, double middleY)
        {
            // 进行逆向映射
            int minPixelValue = XToGray(minX);
            int maxPixelValue = XToGray(maxX);

            double gamma = CalculateGamma();

            NodePositionsChanged?.Invoke(name, minPixelValue, maxPixelValue, gamma);
        }

        private double CalculateGamma()
        {
            double d1 = CalculateDistance(_minNodePosition, _middleNodePosition);
            double d2 = CalculateDistance(_middleNodePosition, _maxNodePosition);
            return d1 / (d1 + d2) * (2.0 - 0.2);
        }

        public System.Drawing.Point CalculateMiddleNodePosition(double gamma)
        {
            // 计算 middleNodePosition 的 x 坐标
            double middleX = _minNodePosition.X + (_maxNodePosition.X - _minNodePosition.X) * gamma / 2.0;

            // 计算 middleNodePosition 的 y 坐标
            double middleY;

            // 根据 _minNodePosition.Y 和 _maxNodePosition.Y 的相对大小来判断如何计算 middleY
            if (_minNodePosition.Y > _maxNodePosition.Y)
            {
                middleY = _minNodePosition.Y - (_minNodePosition.Y - _maxNodePosition.Y) * gamma / 2.0;
            }
            else
            {
                middleY = _minNodePosition.Y + (_maxNodePosition.Y - _minNodePosition.Y) * gamma / 2.0;
            }

            return new System.Drawing.Point((int)middleX, (int)middleY);
        }

    }

}
