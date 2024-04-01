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
    public partial class HelloComboBox : Form
    {

        public HelloComboBox()
        {
            InitializeComponent();

            InitBoxComboBox();

            InitNumericUpDown();
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
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show($"{this.numericUpDown1.Value}");
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            if (numericUpDown1.Text == "")
            {
                // If the value in the numeric updown is an empty string, replace with 0.
                numericUpDown1.Text = numericUpDown1.Value.ToString();
            }
        }
    }
}
