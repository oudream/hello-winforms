using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace HelloWinForms
{
    public partial class HelloToolStrip : Form
    {
        private ToolStripTextBox textBox1;
        private ToolStripComboBox checkBox1;

        private ToolStripTextBox textBox2;
        private ToolStripComboBox checkBox2;

        public HelloToolStrip()
        {
            InitializeComponent();

            // Group 1 Controls
            textBox1 = new ToolStripTextBox();
            checkBox1 = new ToolStripComboBox();
            textBox1.Text = "textBox1";
            checkBox1.Text = "checkBox1";

            // Group 2 Controls
            textBox2 = new ToolStripTextBox();
            checkBox2 = new ToolStripComboBox();
            textBox2.Text = "textBox2";
            checkBox2.Text = "checkBox2";

            // 默认显示第一组
            ShowGroup1();
        }

        private void ShowGroup1()
        {
            toolStrip1.Items.Clear();
            toolStrip1.Items.Add(a1);
            toolStrip1.Items.Add(a2);
            toolStrip1.Items.Add(a3);
            toolStrip1.Items.Add(textBox1);
            toolStrip1.Items.Add(checkBox1);
        }

        private void ShowGroup2()
        {
            toolStrip1.Items.Clear();
            toolStrip1.Items.Add(a1);
            toolStrip1.Items.Add(a2);
            toolStrip1.Items.Add(a3);
            toolStrip1.Items.Add(textBox2);
            toolStrip1.Items.Add(checkBox2);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ShowGroup1();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ShowGroup2();
        }
    }
}
