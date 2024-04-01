using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloProgressDialog : Form
    {
        public HelloProgressDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ProgressDialog.ShowDialog(this, progressDialog =>
            {
                for (int i = 0; i <= 100; i++)
                {
                    Thread.Sleep(50); // 模拟长时间运行的操作
                    progressDialog.Progress = i;
                    progressDialog.DisplayText = $"Loading... {i}%";
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (ProgressDialog progressDialog = new ProgressDialog())
            {
                progressDialog.Show(this);
                for (int i = 0; i <= 100; i++)
                {
                    Thread.Sleep(50); // 模拟长时间运行的操作
                    progressDialog.Progress = i;
                    progressDialog.DisplayText = $"Loading... {i}%";
                }
            }
        }
    }
}
