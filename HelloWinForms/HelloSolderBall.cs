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
    public partial class HelloSolderBall : Form
    {
        // color(0, 150, 136) 常数
        private readonly Color COLOR_BACK = Color.FromArgb(51, 51, 76);

        // 94, 98, 127
        // 143, 143, 192

        private readonly Color COLOR_Fore = Color.White;

        private readonly Color COLOR_SOLDER_OUT = Color.FromArgb(0, 150, 136);

        public HelloSolderBall()
        {
            InitializeComponent();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            //var image = pictureBox1.Image;
            //using (Graphics g = Graphics.FromImage(image))
            //{
            //    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // HelloAutoCloseForm 点击外部关闭示例，自动关闭
            using (HelloAutoCloseForm form = new HelloAutoCloseForm())
            {
                form.ShowDialog();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.BeginInvoke(new System.Action(() =>
            {
                this.Text = $"{DateTime.Now:G}";
            }));
        }
    }
}
