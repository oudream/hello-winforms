using CxWorkStation.Utilities;
using HelloWinForms.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloComboBox : Form
    {
        private IpV4Component ipConponent;


        private string currentSelection;
        private DateTime lastSwitchTime;
        private bool isRestoringPreviousSelection = false;

        public HelloComboBox()
        {
            InitializeComponent();

            InitBoxComboBox();

            InitNumericUpDown();

            ipConponent = new IpV4Component(this.ipTextBox);

            numericUpDown3.Controls[0].Visible = false;

            //this.deleteDateTimePicker.CalendarFont = new Font("宋体", 30.0f);
            //this.deleteDateTimePicker.CalendarForeColor = System.Drawing.Color.FromArgb(255, 0, 115, 0);
            dateTimePicker1.Paint += new PaintEventHandler(dateTimePicker1_Paint);
            dateTimePicker1.Format = DateTimePickerFormat.Custom;
            dateTimePicker1.CustomFormat = "MM/dd/yyyy hh:mm:ss";

            comboBox2.DropDown += ComboBox2_DropDown;
            //ComboBox2_DropDown(null, null);

            currentSelection = defaultManualRadioButton.Tag.ToString(); // 初始选择
            lastSwitchTime = DateTime.Now;
            defaultOKRadioButton.Tag = "OK";
            defaultManualRadioButton.Tag = "Manual";
            defaultNGRadioButton.Tag = "NG";

            defaultOKRadioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            defaultManualRadioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            defaultNGRadioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);

            // 添加选项到下拉菜单
            toolStripDropDownButton1.DropDownItems.Add("导出记录", null, ExportRecord_Click);
            toolStripDropDownButton1.DropDownItems.Add("上传MES", null, UploadMES_Click);
            toolStripDropDownButton1.DropDownItems.Add("导出MES", null, ExportMES_Click);

        }

        private void ExportMES_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UploadMES_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ExportRecord_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (isRestoringPreviousSelection)
                return;

            RadioButton radioButton = sender as RadioButton;
            if (radioButton != null && radioButton.Checked)
            {
                string newSelection = radioButton.Tag.ToString(); // 从 Tag 属性获取新选择的值

                // 检查上次切换时间
                TimeSpan timeSinceLastSwitch = DateTime.Now - lastSwitchTime;
                if (timeSinceLastSwitch.TotalSeconds < 5)
                {
                    MessageBox.Show("切换间隔不能低于5秒", "提示", MessageBoxButtons.OK);
                    RestorePreviousSelection();
                    return;
                }

                if (newSelection != currentSelection)
                {
                    DialogResult result = MessageBox.Show("确定要切换吗？", "确认", MessageBoxButtons.YesNo);
                    if (result == DialogResult.Yes)
                    {
                        currentSelection = newSelection;
                        lastSwitchTime = DateTime.Now; // 更新上次切换时间
                    }
                    else
                    {
                        RestorePreviousSelection();
                    }
                }
            }
        }

        private void RestorePreviousSelection()
        {
            isRestoringPreviousSelection = true;
            // 恢复之前的选择状态
            this.SuspendLayout();
            if (currentSelection == "OK")
            {
                defaultOKRadioButton.Checked = true;
            }
            else if (currentSelection == "Manual")
            {
                defaultManualRadioButton.Checked = true;
            }
            else if (currentSelection == "NG")
            {
                defaultNGRadioButton.Checked = true;
            }
            this.ResumeLayout();
            isRestoringPreviousSelection = false;
        }

        private void ComboBox2_DropDown(object sender, EventArgs e)
        {
            string directoryPath = @"E:\temp\Products"; // 请更改为您的实际目录路径
            UpdateComboBoxWithDirectorySubfolders(comboBox2, directoryPath);
        }

        // 更新 ComboBox 列表项以匹配目录的子文件夹
        private void UpdateComboBoxWithDirectorySubfolders(ComboBox comboBox, string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                // 获取目录中的子文件夹名称
                var subfolders = Directory.GetDirectories(directoryPath)
                                          .Select(Path.GetFileName)
                                          .ToArray();

                // 比较现有的 ComboBox 项目与目录内容，不一致时更新
                var existingItems = comboBox.Items.Cast<string>().ToArray();
                if (!existingItems.SequenceEqual(subfolders))
                {
                    comboBox.Items.Clear();
                    comboBox.Items.AddRange(subfolders);
                    // 显示列表中的第一个项目，如果列表不为空
                    if (comboBox.Items.Count > 0)
                    {
                        comboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        comboBox.SelectedIndex = -1;
                    }
                }
            }
            else
            {
                if (comboBox.Items.Count > 0)
                {
                    comboBox.Items.Clear();
                }
            }
        }

        private void dateTimePicker1_Paint(object sender, PaintEventArgs e)
        {
            // 使用Graphics对象进行绘制
            Graphics g = e.Graphics;
            // 定义字体样式
            using (Font font = new Font("Segoe UI", 15, FontStyle.Bold))
            {
                using (Brush brush = new SolidBrush(Color.Blue))  // 字体颜色
                {
                    // 获取日期显示的字符串
                    string dateString = dateTimePicker1.Value.ToShortDateString();
                    // 绘制字符串
                    g.DrawString(dateString, font, brush, new PointF(0, 2));
                }
            }
        }

        private void InitNumericUpDown()
        {
            if (this.numericUpDown1.Controls.Count > 0)
            {
                this.numericUpDown1.Controls[0].Visible = false;
            }
        }

        private void InitializeComboBox()
        {
            this.SuspendLayout();

            // ComboBox settings
            this.comboBox1.DrawMode = DrawMode.OwnerDrawFixed; // 设置为自定义绘制
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList; // 设置下拉样式
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.ItemHeight = 40; // 设置项的高度
            this.comboBox1.Size = new Size(200, 40); // 设置ComboBox的尺寸
            this.comboBox1.Font = new Font("Arial", 10.8F, FontStyle.Bold, GraphicsUnit.Point); // 设置字体
            this.comboBox1.Location = new System.Drawing.Point(50, 50); // 设置位置
            this.comboBox1.DrawItem += new DrawItemEventHandler(ComboBox1_DrawItem); // 添加自定义绘制的事件处理程序

            // Add items
            this.comboBox1.Items.AddRange(new object[] {
        "Option 1",
        "Option 2",
        "Option 3"});

            // Form settings
            this.Controls.Add(this.comboBox1);
            this.Name = "BeautifulComboBoxForm";
            this.Text = "Beautiful ComboBox Example";
            this.ResumeLayout(false);
        }

        private void ComboBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Check if the index is valid
            if (e.Index < 0) return;

            // Draw the background
            e.DrawBackground();

            // Get the item text
            string text = comboBox1.Items[e.Index].ToString();

            // Draw the text
            using (SolidBrush br = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(text, e.Font, br, e.Bounds);
            }

            // Draw the focus rectangle if the item has focus
            e.DrawFocusRectangle();
        }

        private void InitBoxComboBox()
        {
            //this.BoxComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.BoxComboBox.DropDownHeight = 750;
            this.BoxComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BoxComboBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BoxComboBox.FormattingEnabled = true;
            this.BoxComboBox.IntegralHeight = false;
            //this.BoxComboBox.Location = new System.Drawing.Point(3, 75);
            this.BoxComboBox.Name = "BoxComboBox";
            this.BoxComboBox.Items.Clear();
            this.BoxComboBox.Items.Add("Hello");
            this.BoxComboBox.Items.Add("World");
            this.BoxComboBox.Items.Add("AAA");
            this.BoxComboBox.Items.Add("bbb");
            //this.BoxComboBox.Size = new System.Drawing.Size(1058, 45);
            this.BoxComboBox.TabIndex = 1;
            this.BoxComboBox.SelectedIndexChanged += new System.EventHandler(this.BoxComboBox_SelectedIndexChanged);
            this.BoxComboBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.BoxBarcodeTextBox_KeyDown);
        }

        private void BoxBarcodeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void BoxComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Console.WriteLine(this.BoxComboBox.SelectedItem.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 2024-03-29 转为DateTime
            DateTime dt = Convert.ToDateTime("2024-03-29");
            Console.WriteLine(dt.ToString());
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            if (numericUpDown1.Text == "")
            {
                // If the value in the numeric updown is an empty string, replace with 0.
                numericUpDown1.Text = numericUpDown1.Value.ToString();
            }
        }

        private void label1_Paint(object sender, PaintEventArgs e)
        {
            // 获取Label的宽度和高度
            int width = label1.Width;
            int height = label1.Height;

            // 确保绘制区域与控件边界的兼容性
            int radius = height / 2; // 半圆半径基于高度，确保绘制内容不会超出控件底部

            GraphicsPath path = new GraphicsPath();
            // 开始点
            path.StartFigure();
            // 上边
            path.AddLine(radius, 0, width - radius, 0);
            // 右侧半圆 -2 是为了避免与右侧边界重合
            path.AddArc(width - 2 * radius - 2, 0, 2 * radius, height - 1, -90, 180);
            // 下边
            path.AddLine(width - radius, height - 1, radius, height - 1);
            // 左侧半圆
            path.AddArc(0, 0, 2 * radius, height - 1, 90, 180);
            // 结束点
            path.CloseFigure();

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            //e.Graphics.FillPath(new SolidBrush(BackColor), path); // 填充形状
            //e.Graphics.DrawPath(new Pen(ForeColor, 2), path); // 绘制边界，可以适当调整Pen的大小以确保边界的可见性
            e.Graphics.FillPath(new SolidBrush(Color.Green), path); // 填充形状
            e.Graphics.DrawPath(new Pen(Color.Green, 2), path); // 绘制边界，可以适当调整Pen的大小以确保边界的可见性

            // 绘制文本
            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            e.Graphics.DrawString(label1.Text, label1.Font, new SolidBrush(ForeColor), new RectangleF(0, 0, width, height), format);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            VirtualKeyboardNumber keyboardForm = new VirtualKeyboardNumber();
            keyboardForm.Initialize(this.numericUpDown1.Value, this.numericUpDown1.Maximum, this.numericUpDown1.Minimum);
            if (keyboardForm.ShowDialog() == DialogResult.OK)
            {
                decimal userInput = keyboardForm.UserInputNumber;
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label1.Text = trackBar1.Value.ToString();
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = !checkBox1.Checked;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            var dtNow = TimeHelper.GetNow();
            if (dtNow % 2 == 0 && checkBox1.Checked)
            {
                checkBox1.Checked = false;
            }
            Console.WriteLine($"checkBox1 CheckedChanged");
        }

        private void checkBox1_Click(object sender, EventArgs e)
        {
            Console.WriteLine($"checkBox1 Click");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    Console.WriteLine("---", DateTime.Now);
                }));
            }
            else
            {
                Console.WriteLine(DateTime.Now);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetCodeArea();
            Point pt = new Point(0, 0);
            pt.X = 1720;
            pt.Y = 732;
            int index = FitArea(pt);
            if (index != -1)
                Console.WriteLine($"在第{index}个区域");
        }

        private List<CodeArea> CodeAreas = new List<CodeArea>();
        private void SetCodeArea()
        {
            //条码1范围
            CodeAreas.Add(new CodeArea()
            {
                LeftTop = new Point(0, 0),
                RightBottom = new Point(0, 0),
            });
            //条码2范围
            CodeAreas.Add(new CodeArea()
            {
                LeftTop = new Point(1400, 1020),
                RightBottom = new Point(1970, 1470),
            });
            //条码3范围
            CodeAreas.Add(new CodeArea()
            {
                LeftTop = new Point(1450, 630),
                RightBottom = new Point(1930, 960),
            });
        }


        /// <summary>
        /// 判定点在三个条码范围的哪个范围，0、1、2，返回-1表示三个范围都不属于
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private int FitArea(Point point)
        {
            for (int i = 0; i < CodeAreas.Count; i++)
            {
                CodeArea area = CodeAreas[i];
                if (point.X >= area.LeftTop.X && point.X <= area.RightBottom.X)
                {
                    if (point.Y >= area.LeftTop.Y && point.Y <= area.RightBottom.Y)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            comboBox3.Text = "";
        }
    }

    public class CodeArea
    {
        public Point LeftTop { get; set; }      //左上角位置
        public Point RightBottom { get; set; }  //右下角位置
    }
}
