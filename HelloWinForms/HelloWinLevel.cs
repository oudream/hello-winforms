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
            Controls.Add(histogramControl);

            histogramControl.NodePositionChanged += HistogramControl_NodePositionChanged;

            Button generateHistogramButton = new Button
            {
                Text = "Generate Histogram",
                Dock = DockStyle.Top
            };
            generateHistogramButton.Click += GenerateHistogramButton_Click;
            Controls.Add(generateHistogramButton);
        }

        private void GenerateHistogramButton_Click(object sender, EventArgs e)
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

        private void HistogramControl_NodePositionChanged(int x, int y)
        {
            Console.WriteLine($"Node Position Changed: X={x}, Y={y}");
        }


    }
}
