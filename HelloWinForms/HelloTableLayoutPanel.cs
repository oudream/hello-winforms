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
    public partial class HelloTableLayoutPanel : Form
    {
        public HelloTableLayoutPanel()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                Console.WriteLine("RadioButton1 Checked");
            }
        }
    }
}
