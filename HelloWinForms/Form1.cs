using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace HelloWinForms
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.dockPanel.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();

            ShowDockContent();
        }

        public int _i;

        public int _k
        {
            get; set;
        }

        public void ShowDockContent()
        {
            var doc1 = new NewDockContent();
            doc1.Show(this.dockPanel, DockState.Document);

            var doc2 = new NewDockContent();
            doc2.GetButton1().Text = "Doc2";
            doc2.DockHandler.AllowEndUserDocking = false;
            doc2.Show(this.dockPanel, DockState.DockLeft);

            //var doc3 = new NewDockContent();
            //doc3.GetButton1().Text = "Doc3";
            //doc3.Show(doc1.Pane, null);
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
