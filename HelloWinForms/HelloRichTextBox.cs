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
    public partial class HelloRichTextBox : Form
    {
        public HelloRichTextBox()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.AppendText("aaa bbb ccc 1 aaa bbb ccc 2 aaa bbb ccc 3 aaa bbb ccc 4 aaa bbb ccc 5 aaa bbb ccc " + Environment.NewLine);
            this.richTextBox1.AppendText("aaa bbb ccc 1 aaa bbb ccc 2 aaa bbb ccc 3 aaa bbb ccc 4 aaa bbb ccc 5 aaa bbb ccc " + Environment.NewLine);
            this.richTextBox1.AppendText("aaa bbb ccc 1 aaa bbb ccc 2 aaa bbb ccc 3 aaa bbb ccc 4 aaa bbb ccc 5 aaa bbb ccc " + Environment.NewLine);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_ImeChange(object sender, EventArgs e)
        {

        }
    }
}
