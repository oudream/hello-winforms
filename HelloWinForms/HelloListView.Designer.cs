namespace HelloWinForms
{
    partial class HelloListView
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
            this.listView = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.viewStyleGroupBox = new System.Windows.Forms.GroupBox();
            this.detailRadioButton = new System.Windows.Forms.RadioButton();
            this.smallRadioButton = new System.Windows.Forms.RadioButton();
            this.midleRadioButton = new System.Windows.Forms.RadioButton();
            this.bigRadioButton = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.panel1.SuspendLayout();
            this.viewStyleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(3, 3);
            this.listView.Name = "listView";
            this.listView.OwnerDraw = true;
            this.listView.Size = new System.Drawing.Size(853, 389);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.listView_DrawSubItem);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(98, 49);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 165);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(867, 427);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.listView);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(859, 395);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dataGridView);
            this.tabPage2.Location = new System.Drawing.Point(4, 28);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(859, 395);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // dataGridView
            // 
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.Location = new System.Drawing.Point(3, 3);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersWidth = 62;
            this.dataGridView.RowTemplate.Height = 30;
            this.dataGridView.Size = new System.Drawing.Size(853, 389);
            this.dataGridView.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.viewStyleGroupBox);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(867, 165);
            this.panel1.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(12, 67);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(98, 49);
            this.button4.TabIndex = 4;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // viewStyleGroupBox
            // 
            this.viewStyleGroupBox.Controls.Add(this.detailRadioButton);
            this.viewStyleGroupBox.Controls.Add(this.smallRadioButton);
            this.viewStyleGroupBox.Controls.Add(this.midleRadioButton);
            this.viewStyleGroupBox.Controls.Add(this.bigRadioButton);
            this.viewStyleGroupBox.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.viewStyleGroupBox.Location = new System.Drawing.Point(331, 32);
            this.viewStyleGroupBox.Name = "viewStyleGroupBox";
            this.viewStyleGroupBox.Size = new System.Drawing.Size(462, 50);
            this.viewStyleGroupBox.TabIndex = 3;
            this.viewStyleGroupBox.TabStop = false;
            // 
            // detailRadioButton
            // 
            this.detailRadioButton.AutoSize = true;
            this.detailRadioButton.Checked = true;
            this.detailRadioButton.Location = new System.Drawing.Point(307, 16);
            this.detailRadioButton.Name = "detailRadioButton";
            this.detailRadioButton.Size = new System.Drawing.Size(131, 28);
            this.detailRadioButton.TabIndex = 1;
            this.detailRadioButton.TabStop = true;
            this.detailRadioButton.Text = "详细信息";
            this.detailRadioButton.UseVisualStyleBackColor = true;
            this.detailRadioButton.CheckedChanged += new System.EventHandler(this.detailRadioButton_CheckedChanged);
            // 
            // smallRadioButton
            // 
            this.smallRadioButton.AutoSize = true;
            this.smallRadioButton.Location = new System.Drawing.Point(209, 15);
            this.smallRadioButton.Name = "smallRadioButton";
            this.smallRadioButton.Size = new System.Drawing.Size(83, 28);
            this.smallRadioButton.TabIndex = 1;
            this.smallRadioButton.TabStop = true;
            this.smallRadioButton.Text = "小图";
            this.smallRadioButton.UseVisualStyleBackColor = true;
            // 
            // midleRadioButton
            // 
            this.midleRadioButton.AutoSize = true;
            this.midleRadioButton.Location = new System.Drawing.Point(111, 15);
            this.midleRadioButton.Name = "midleRadioButton";
            this.midleRadioButton.Size = new System.Drawing.Size(83, 28);
            this.midleRadioButton.TabIndex = 1;
            this.midleRadioButton.TabStop = true;
            this.midleRadioButton.Text = "中图";
            this.midleRadioButton.UseVisualStyleBackColor = true;
            // 
            // bigRadioButton
            // 
            this.bigRadioButton.AutoSize = true;
            this.bigRadioButton.Location = new System.Drawing.Point(22, 15);
            this.bigRadioButton.Name = "bigRadioButton";
            this.bigRadioButton.Size = new System.Drawing.Size(83, 28);
            this.bigRadioButton.TabIndex = 0;
            this.bigRadioButton.TabStop = true;
            this.bigRadioButton.Text = "大图";
            this.bigRadioButton.UseVisualStyleBackColor = true;
            this.bigRadioButton.CheckedChanged += new System.EventHandler(this.bigRadioButton_CheckedChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(116, 12);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 49);
            this.button2.TabIndex = 2;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(220, 12);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(98, 49);
            this.button3.TabIndex = 1;
            this.button3.Text = "button1";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(187, 108);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 5;
            // 
            // HelloListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 592);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.panel1);
            this.Name = "HelloListView";
            this.Text = "HelloListView";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.panel1.ResumeLayout(false);
            this.viewStyleGroupBox.ResumeLayout(false);
            this.viewStyleGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox viewStyleGroupBox;
        private System.Windows.Forms.RadioButton detailRadioButton;
        private System.Windows.Forms.RadioButton smallRadioButton;
        private System.Windows.Forms.RadioButton midleRadioButton;
        private System.Windows.Forms.RadioButton bigRadioButton;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}