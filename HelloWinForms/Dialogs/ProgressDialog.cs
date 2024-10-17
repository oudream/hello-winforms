using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.Dialogs
{
    public partial class ProgressDialog : Form
    {
        private int progress = 0;
        private int max = 100;
        private string displayText = "Loading...";

        public int Progress
        {
            get { return progress; }
            set
            {
                progress = value;
                Invalidate(); // 触发重绘并更新进度
                if (progress >= max)
                {
                    // 当进度达到最大值时自动关闭对话框
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new System.Action(() => this.Close()));
                    }
                    else
                    {
                        this.Close();
                    }
                }
            }
        }

        public string DisplayText
        {
            get { return displayText; }
            set { displayText = value; Invalidate(); } // 更新显示的文本
        }

        public ProgressDialog()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
            this.BackColor = Color.White; // 设置背景色
            this.FormBorderStyle = FormBorderStyle.None; // 设置无边框
            this.StartPosition = FormStartPosition.CenterScreen; // 窗体居中显示
            this.Size = new Size(300, 300); // 设置窗体尺寸
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 确保圆形绘制区域为正方形，且根据窗体尺寸自适应居中
            int thickness = 30; // 圆环厚度，可以根据需要调整
            int diameter = Math.Min(this.Width, this.Height) - thickness * 2; // 圆的直径
            Point center = new Point((this.Width - diameter) / 2, (this.Height - diameter) / 2); // 计算圆心位置以居中圆形
            Rectangle rect = new Rectangle((int)center.X, (int)center.Y, diameter, diameter); // 创建正方形绘制区域以保证绘制的是正圆

            using (Pen pen = new Pen(Color.FromArgb(244, 67, 54), thickness))
            {
                pen.StartCap = pen.EndCap = LineCap.Round;
                e.Graphics.DrawArc(pen, rect, -90, (360 * progress) / max); // 绘制圆环进度条
            }

            // 绘制文本
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(displayText, new Font("Arial", 10), Brushes.Black, rect, sf);
        }

        // 以模态方式显示对话框的方法
        public static void ShowDialog(IWin32Window owner, Action<ProgressDialog> progressAction)
        {
            using (ProgressDialog progressDialog = new ProgressDialog())
            {
                progressDialog.ShowInTaskbar = false; // 不在任务栏显示
                Task.Run(() => progressAction(progressDialog));
                progressDialog.ShowDialog(owner);
            }
        }
    }
}
