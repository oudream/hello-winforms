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
    public partial class HelloPanel : Form
    {
        public HelloPanel()
        {
            InitializeComponent(); 
        }

        private void togglePanel_Paint(object sender, PaintEventArgs e)
        {
            // 计算绘制三角形的位置和大小
            float triangleWidth = 10;
            float triangleHeight = 10;
            PointF center = new PointF(togglePanel.Width / 2f, togglePanel.Height / 2f);
            PointF[] triangle = new PointF[3];

            if (togglePanel.Tag == null)
            {
                // 绘制向下的三角形
                triangle[0] = new PointF(center.X - triangleWidth / 2, center.Y - triangleHeight / 2);
                triangle[1] = new PointF(center.X + triangleWidth / 2, center.Y - triangleHeight / 2);
                triangle[2] = new PointF(center.X, center.Y + triangleHeight / 2);
            }
            else
            {
                // 绘制向上的三角形
                triangle[0] = new PointF(center.X - triangleWidth / 2, center.Y + triangleHeight / 2);
                triangle[1] = new PointF(center.X + triangleWidth / 2, center.Y + triangleHeight / 2);
                triangle[2] = new PointF(center.X, center.Y - triangleHeight / 2);
            }

            // 使用 Graphics 对象绘制三角形
            using (SolidBrush brush = new SolidBrush(Color.Black))
            {
                e.Graphics.FillPolygon(brush, triangle);
            }
        }

        private void togglePanel_Click(object sender, EventArgs e)
        {
            if (togglePanel.Tag == null)
            {
                togglePanel.Tag = debugPanel.Height;
                // 收起面板
                debugPanel.Height = togglePanel.Height; // 设置为您所需的最小高度
                debugContainerPanel.Enabled = false;
            }
            else
            {
                int height = (int)togglePanel.Tag;
                // 展开面板
                debugPanel.Height = height; // 设置为您所需的最大高度
                togglePanel.Tag = null;
                debugContainerPanel.Enabled = true;
            }
            togglePanel.Invalidate();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int totalWidth = statusStrip1.Width - statusStrip1.Padding.Horizontal; // 获取StatusStrip的宽度减去内边距
            toolStripStatusLabel1.Width = (int)(totalWidth * 0.8); // 第一个标签占80%
            toolStripStatusLabel2.Width = totalWidth - toolStripStatusLabel1.Width; // 第二个标签占剩余的宽度
        }

        // 调整标签的大小比例
        void ResizeStatusLabels()
        {
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 创建一个引用类型对象的列表
            List<string> stringList = new List<string>();

            // 向列表中添加数据
            stringList.Add("Hello");
            stringList.Add(null);  // 添加一个 null 值
            stringList.Add("World");

            // 打印列表内容
            foreach (var item in stringList)
            {
                Console.WriteLine(item ?? "null");  // 使用 ?? 运算符处理 null 值
            }
        }
    }

}
