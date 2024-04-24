using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HelloWinForms.Actions;
using Microsoft.Win32;
using WeifenLuo.WinFormsUI.Docking;

namespace HelloWinForms
{
    public partial class HelloToolBarPanel : Form
    {
        private PRight pRight;
        private PTray pTray;

        private const string AppName = "CxWorkStation";
        private const string AppTitle = "长园-xray工作站系统";
        private const string RegistryKeyRun = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        //
        private ToolStripMenuItem autoStartToolStripMenuItem;

        public HelloToolBarPanel()
        {
            InitializeComponent();

            pRight = new PRight();
            pTray = new PTray();

            this.dockPanel.Theme = new WeifenLuo.WinFormsUI.Docking.VS2015BlueTheme();

            ShowDockContent();

            Text = AppTitle;
            notifyIcon.Text = AppTitle;

            // Show ToolStripMenuItem
            var showToolStripMenuItem = new ToolStripMenuItem();
            showToolStripMenuItem.Text = "显示(&O)";
            showToolStripMenuItem.Click += ShowToolStripMenuItem_Click;
            contextMenuStrip.Items.Add(showToolStripMenuItem);

            // Exit ToolStripMenuItem
            var exitToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem.Text = "退出(&E)";
            exitToolStripMenuItem.Click += ExitToolStripMenuItem_Click;
            contextMenuStrip.Items.Add(exitToolStripMenuItem);

            autoStartToolStripMenuItem = new ToolStripMenuItem();
            autoStartToolStripMenuItem.Text = "随系统自启动(&R)";
            autoStartToolStripMenuItem.CheckOnClick = true;
            autoStartToolStripMenuItem.CheckedChanged += AutoStartToolStripMenuItem_CheckedChanged;
            contextMenuStrip.Items.Add(autoStartToolStripMenuItem);
            // Check if the application is set to run at startup
            autoStartToolStripMenuItem.Checked = IsAutoStartEnabled();

            notifyIcon.ContextMenuStrip = contextMenuStrip;
            notifyIcon.DoubleClick += ShowToolStripMenuItem_Click;

            Resize += MainForm_Resize;
        }

        static string GetExecutablePath()
        {
            // 获取当前执行的程序的 Assembly
            Assembly assembly = Assembly.GetEntryAssembly();

            // 获取程序的全路径
            string executablePath = assembly.Location;

            return executablePath;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon.ShowBalloonTip(1000, AppName, $"{AppTitle}已最小化到托盘", ToolTipIcon.Info);
            }
        }

        private void ShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("您确定要退出吗？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 用户确认退出
                notifyIcon.Visible = false;
                Application.Exit();
            }
        }

        private void AutoStartToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            SetAutoStart(autoStartToolStripMenuItem.Checked);
        }

        private void SetAutoStart(bool enable)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyRun, true))
            {
                if (enable)
                {
                    // Add the application to the registry to run at startup
                    key.SetValue(AppName, GetExecutablePath());
                }
                else
                {
                    // Remove the application from the registry
                    key.DeleteValue(AppName, false);
                }
            }
        }

        private bool IsAutoStartEnabled()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyRun, true))
            {
                // Check if the application is in the registry key
                return key.GetValue(AppName) != null;
            }
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
            if (flowLayoutPanel.Controls.Count >= 5)
            {
                flowLayoutPanel.Controls.Remove(flowLayoutPanel.Controls[4]);
            }
            flowLayoutPanel.Controls.Add(pRight);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (flowLayoutPanel.Controls.Count >= 5)
            {
                flowLayoutPanel.Controls.Remove(flowLayoutPanel.Controls[4]);
            }
            flowLayoutPanel.Controls.Add(pTray);
        }
    }
}
