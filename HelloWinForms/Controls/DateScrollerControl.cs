using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.Controls
{
    public partial class DateScrollerControl : UserControl
    {
        public event EventHandler<DateTime> DateSelected;

        public DateScrollerControl()
        {
            InitializeComponent();

            btnNext.Click += btnNext_BtnClick;
            btnPrevious.Click += btnPrevious_BtnClick;
            // 在窗体的初始化代码中或者设计视图中为每个按钮添加点击事件处理器
            p1.Click += (sender, e) => SelectDate((Button)sender, startDate);
            p2.Click += (sender, e) => SelectDate((Button)sender, startDate.AddDays(1));
            p3.Click += (sender, e) => SelectDate((Button)sender, startDate.AddDays(2));
            p4.Click += (sender, e) => SelectDate((Button)sender, startDate.AddDays(3));
            p5.Click += (sender, e) => SelectDate((Button)sender, endDate);

            UpdateDateButtons();
        }

        private void btnPrevious_BtnClick(object sender, EventArgs e)
        {
            ScrollRight();
        }

        private void btnNext_BtnClick(object sender, EventArgs e)
        {
            ScrollLeft();
        }

        DateTime endDate = DateTime.Today; // 结束日期初始化为今天
        DateTime startDate = DateTime.Today.AddDays(-4); // 开始日期初始化为5天前

        // 更新按钮显示的日期
        void UpdateDateButtons()
        {
            DateTime currentDate = startDate;
            for (int i = 1; i <= 5; i++)
            {
                if (tableLayoutPanel1.Controls.Find($"p{i}", true).FirstOrDefault() is Button button)
                {
                    button.Text = currentDate.ToString("MM/dd");
                    currentDate = currentDate.AddDays(1);
                };
            }
            // 更新滚动按钮的状态
            UpdateScrollButtonsState();
            SelectDate(p5, endDate);
        }

        // 更新滚动按钮的启用状态
        void UpdateScrollButtonsState()
        {
            // 如果startDate比30天前的日期要大，说明可以继续向左滚动
            btnNext.Enabled = startDate.AddDays(-5) >= DateTime.Today.AddDays(-30);

            // 如果endDate比今天小，说明可以继续向右滚动
            btnPrevious.Enabled = endDate < DateTime.Today;
        }

        // 向左滚动日期
        void ScrollLeft()
        {
            if (startDate.AddDays(-5) >= DateTime.Today.AddDays(-30))
            {
                startDate = startDate.AddDays(-5);
                endDate = endDate.AddDays(-5);
                UpdateDateButtons();
            }
        }

        // 向右滚动日期
        void ScrollRight()
        {
            if (endDate < DateTime.Today)
            {
                startDate = startDate.AddDays(5);
                endDate = endDate.AddDays(5);
                UpdateDateButtons();
            }
        }

        void SelectDate(Button selectedButton, DateTime date)
        {
            // 首先重置所有日期按钮的背景色
            for (int i = 1; i <= 5; i++)
            {
                if (tableLayoutPanel1.Controls.Find($"p{i}", true).FirstOrDefault() is Button button)
                {
                    button.BackColor = Color.FromArgb(223, 223, 223); 
                };
            }

            // 设置选中按钮的背景色
            selectedButton.BackColor = Color.FromArgb(255, 77, 59);

            // 发布选中的日期
            DateSelected?.Invoke(this, date);
        }
    }
}
