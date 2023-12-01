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
    public partial class NewDockContent : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        public NewDockContent()
        {
            InitializeComponent();
        }

        public Button GetButton1()
        {
            return this.button1;
        }
        private void btnDefault_Click(object sender, EventArgs e)
        {
            // 设置默认光标
            Cursor = Cursors.Default;
        }

        private void btnHand_Click(object sender, EventArgs e)
        {
            // 设置手形光标
            Cursor = Cursors.Hand;
        }

        private void btnCross_Click(object sender, EventArgs e)
        {
            // 设置十字光标
            Cursor = Cursors.Cross;
        }

        private void btnWait_Click(object sender, EventArgs e)
        {
            // 设置等待光标
            Cursor = Cursors.WaitCursor;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // 初始化Person实例
            var person = new DockPerson
            {
                FirstName = "John",
                LastName = "Doe",
                Age = 30
            };

            // 将Person实例分配给PropertyGrid
            propertyGrid1.SelectedObject = person;
        }
    }
    public class DockPerson
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

}
