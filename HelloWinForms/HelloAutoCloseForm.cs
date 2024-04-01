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
    public partial class HelloAutoCloseForm : Form
    {
        // 定义Windows消息常量
        private const int WM_NCACTIVATE = 0x0086;

        public HelloAutoCloseForm()
        {
            InitializeComponent();

            // Form初始化代码
            this.Text = "点击外部关闭示例";
        }

        // 重写WndProc方法以监听Windows消息
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCACTIVATE && m.WParam == IntPtr.Zero)
            {
                // 如果WM_NCACTIVATE消息的wParam参数为0，则表示点击了Form外部
                this.Close(); // 关闭Form
            }
            base.WndProc(ref m);
        }
    }
}
