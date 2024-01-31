namespace HelloWinForms
{
    partial class HelloPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.debugPanel = new System.Windows.Forms.Panel();
            this.debugContainerPanel = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.togglePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.debugPanel.SuspendLayout();
            this.debugContainerPanel.SuspendLayout();
            this.togglePanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // debugPanel
            // 
            this.debugPanel.Controls.Add(this.debugContainerPanel);
            this.debugPanel.Controls.Add(this.togglePanel);
            this.debugPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.debugPanel.Location = new System.Drawing.Point(0, 0);
            this.debugPanel.Name = "debugPanel";
            this.debugPanel.Size = new System.Drawing.Size(986, 144);
            this.debugPanel.TabIndex = 0;
            // 
            // debugContainerPanel
            // 
            this.debugContainerPanel.Controls.Add(this.button3);
            this.debugContainerPanel.Controls.Add(this.button4);
            this.debugContainerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debugContainerPanel.Location = new System.Drawing.Point(0, 22);
            this.debugContainerPanel.Name = "debugContainerPanel";
            this.debugContainerPanel.Size = new System.Drawing.Size(986, 122);
            this.debugContainerPanel.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(3, 3);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(93, 45);
            this.button3.TabIndex = 1;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(3, 54);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(93, 43);
            this.button4.TabIndex = 1;
            this.button4.Text = "button3";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // togglePanel
            // 
            this.togglePanel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.togglePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.togglePanel.Controls.Add(this.label1);
            this.togglePanel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.togglePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.togglePanel.Location = new System.Drawing.Point(0, 0);
            this.togglePanel.Name = "togglePanel";
            this.togglePanel.Size = new System.Drawing.Size(986, 22);
            this.togglePanel.TabIndex = 2;
            this.togglePanel.Click += new System.EventHandler(this.togglePanel_Click);
            this.togglePanel.Paint += new System.Windows.Forms.PaintEventHandler(this.togglePanel_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "Debug";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 144);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(986, 188);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "groupBox1";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 76);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(93, 43);
            this.button2.TabIndex = 0;
            this.button2.Text = "button1";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 27);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 43);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 467);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(986, 31);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(909, 24);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(62, 24);
            this.toolStripStatusLabel2.Text = "__xxxL";
            this.toolStripStatusLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // HelloPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(986, 498);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.debugPanel);
            this.Name = "HelloPanel";
            this.Text = "HelloPanel";
            this.debugPanel.ResumeLayout(false);
            this.debugContainerPanel.ResumeLayout(false);
            this.togglePanel.ResumeLayout(false);
            this.togglePanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel debugPanel;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel togglePanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Panel debugContainerPanel;
        private System.Windows.Forms.Button button2;
    }
}