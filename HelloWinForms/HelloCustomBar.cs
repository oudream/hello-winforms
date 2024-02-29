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
    public partial class HelloCustomBar : Form
    {
        public HelloCustomBar()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None; // 设置窗体为无边框
            InitializeCustomTitleBar();
        }

        private void InitializeCustomTitleBar()
        {
            // 创建一个Panel作为自定义标题栏
            Panel titleBar = new Panel();
            titleBar.BackColor = Color.Blue;
            titleBar.Height = 30; // 设置标题栏高度
            titleBar.Dock = DockStyle.Top;
            titleBar.MouseDown += new MouseEventHandler(TitleBar_MouseDown);
            this.Controls.Add(titleBar);

            // 创建一个Button作为自定义最大化按钮
            Button maximizeButton = new Button();
            maximizeButton.Text = "□";
            maximizeButton.ForeColor = Color.White;
            maximizeButton.BackColor = Color.Gray;
            maximizeButton.Height = 30;
            maximizeButton.Width = 30;
            maximizeButton.Dock = DockStyle.Right;
            maximizeButton.Click += new EventHandler(MaximizeButton_Click);
            titleBar.Controls.Add(maximizeButton);

            // 创建一个Button作为自定义关闭按钮
            Button closeButton = new Button();
            closeButton.Text = "X";
            closeButton.ForeColor = Color.White;
            closeButton.BackColor = Color.Red;
            closeButton.Height = 30;
            closeButton.Width = 30;
            closeButton.Dock = DockStyle.Right;
            closeButton.Click += new EventHandler(CloseButton_Click);
            titleBar.Controls.Add(closeButton);

        }
        private bool isMaximized = false; // 用来跟踪窗体是否已最大化
        private void MaximizeButton_Click(object sender, EventArgs e)
        {
            if (!isMaximized)
            {
                // 最大化到工作区域，不覆盖任务栏
                this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
                this.WindowState = FormWindowState.Maximized; // 窗体最大化
                isMaximized = true;
            }
            else
            {
                this.WindowState = FormWindowState.Normal; // 恢复窗体大小
                isMaximized = false;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            // 调用Win32 API函数来拖动无边框窗体
            if (e.Button == MouseButtons.Left)
            {
                this.Capture = false;
                Message msg = Message.Create(this.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
                this.WndProc(ref msg);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); // 关闭窗体
        }
    }

}
