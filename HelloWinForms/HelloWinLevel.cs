using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloWinLevel : Form
    {
        private HistogramWithAdjustableDiagonalLineControl histogramControl;

        public HelloWinLevel()
        {
            InitializeComponent();

            histogramControl = new HistogramWithAdjustableDiagonalLineControl
            {
                Dock = DockStyle.Fill
            };
            histogramControl.NodePositionsChanged += HistogramControl_NodePositionChanged;
            panel1.Controls.Add(histogramControl);
        }

        private void HistogramControl_NodePositionChanged(string name, int minX, int minY, int middleX, int middleY, int maxX, int maxY)
        {
            Console.WriteLine($"Node Position Changed: Name={name}, minX={minX}, minY={minY}, middleX={middleX}, middleY={middleY}, maxX={maxX}, maxY={maxY}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 此处演示一个简单的直方图数据生成，实际应用中需要根据图像数据生成
            Random random = new Random();
            int[] histogramData = new int[256];
            for (int i = 0; i < 256; i++)
            {
                histogramData[i] = random.Next(0, 100);
            }

            histogramControl.Histogram = histogramData;
        }
    }
}
