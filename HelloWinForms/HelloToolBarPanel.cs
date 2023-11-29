using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelloWinForms.Action;
using WeifenLuo.WinFormsUI.Docking;

namespace HelloWinForms
{
    public partial class HelloToolBarPanel : Form
    {
        private PRight pRight;
        private PTray pTray;

        public HelloToolBarPanel()
        {
            InitializeComponent();

            pRight = new PRight();
            pTray = new PTray();

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

        private void HelloToolBarPanel_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < flowLayoutPanel.Controls.Count; i++)
            {
                var c = flowLayoutPanel.Controls[i];
                c.Height = flowLayoutPanel.Height - flowLayoutPanel.Margin.Top - flowLayoutPanel.Margin.Bottom;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel.Controls.Count >= 3)
            {
                flowLayoutPanel.Controls.Remove(flowLayoutPanel.Controls[2]);
            }
            flowLayoutPanel.Controls.Add(pRight);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel.Controls.Count >= 3)
            {
                flowLayoutPanel.Controls.Remove(flowLayoutPanel.Controls[2]);
            }
            flowLayoutPanel.Controls.Add(pTray);
        }
    }
}
