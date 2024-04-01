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
    public partial class CircularIndicator : UserControl
    {
        private Color _color;

        public CircularIndicator()
        {
            // 设置默认颜色
            _color = Color.Green;
            // 重要：设置控件样式以支持透明背景色，并启用双缓冲减少闪烁
            SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
            this.BackColor = Color.Transparent; // 设置背景透明
        }

        public Color IndicatorColor
        {
            get { return _color; }
            set
            {
                _color = value;
                this.Invalidate(); // 触发重绘
            }
        }

        public Label IndicatorLabel { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // 通过平滑模式使圆形边缘更平滑
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int diameter = Math.Min(Width, Height); // 确保圆形完全位于控件内部
            int margin = diameter / 2;
            int radius = diameter - margin;
            Rectangle rect = new Rectangle(margin/2, margin/2, radius, radius); // 创建一个正方形区域用于绘制圆形

            using (Brush brush = new SolidBrush(_color))
            {
                e.Graphics.FillEllipse(brush, rect); // 填充圆形
            }
        }

        // 更新状态的方法
        public void UpdateStatus(bool isGood)
        {
            IndicatorColor = isGood ? Color.Green : Color.Red;
            IndicatorLabel.Text = isGood ? "在线" : "离线";
        }
    }
}
