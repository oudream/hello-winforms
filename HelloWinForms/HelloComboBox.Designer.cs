namespace HelloWinForms
{
    partial class HelloComboBox
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelloComboBox));
            this.BoxComboBox = new System.Windows.Forms.ComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.button1 = new System.Windows.Forms.Button();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cueTextBox1 = new HelloWinForms.Components.CueTextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.deleteDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.deleteButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.button7 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.button8 = new System.Windows.Forms.Button();
            this.defaultConfirmGroupBox = new System.Windows.Forms.GroupBox();
            this.defaultManualRadioButton = new System.Windows.Forms.RadioButton();
            this.defaultNGRadioButton = new System.Windows.Forms.RadioButton();
            this.defaultOKRadioButton = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button9 = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dateTimePickerBigOne1 = new HelloWinForms.Controls.DateTimePickerBigOne();
            this.datePickerWithWeekNumbers1 = new DatePickerWithWeekNumbers();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.defaultConfirmGroupBox.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // BoxComboBox
            // 
            this.BoxComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BoxComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BoxComboBox.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.BoxComboBox.FormattingEnabled = true;
            this.BoxComboBox.Items.AddRange(new object[] {
            "ProductA",
            "ProductB",
            "ProductC"});
            this.BoxComboBox.Location = new System.Drawing.Point(-541, 199);
            this.BoxComboBox.Name = "BoxComboBox";
            this.BoxComboBox.Size = new System.Drawing.Size(1283, 56);
            this.BoxComboBox.TabIndex = 2;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericUpDown1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericUpDown1.Font = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown1.Location = new System.Drawing.Point(61, 0);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            130,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(195, 58);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDown1.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Leave += new System.EventHandler(this.numericUpDown1_Leave);
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(258, 125);
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(120, 28);
            this.numericUpDown2.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(434, 276);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(129, 62);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolTip1.SetToolTip(this.button1, "选择按钮，显示操作");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ipTextBox
            // 
            this.ipTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ipTextBox.Location = new System.Drawing.Point(36, 125);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(203, 28);
            this.ipTextBox.TabIndex = 7;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Right;
            this.button2.Location = new System.Drawing.Point(256, 0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(61, 62);
            this.button2.TabIndex = 8;
            this.button2.Text = "<";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Dock = System.Windows.Forms.DockStyle.Right;
            this.button3.Location = new System.Drawing.Point(317, 0);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(61, 62);
            this.button3.TabIndex = 8;
            this.button3.Text = ">";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Dock = System.Windows.Forms.DockStyle.Left;
            this.button4.Location = new System.Drawing.Point(0, 0);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(61, 62);
            this.button4.TabIndex = 8;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(203, 628);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(289, 28);
            this.textBox2.TabIndex = 9;
            this.textBox2.Text = "12323.111";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.numericUpDown1);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Location = new System.Drawing.Point(36, 273);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(378, 62);
            this.panel1.TabIndex = 10;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("宋体", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePicker1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePicker1.Location = new System.Drawing.Point(36, 180);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(652, 42);
            this.dateTimePicker1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(33, 437);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(397, 68);
            this.label1.TabIndex = 12;
            this.label1.Text = "label1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.Paint += new System.Windows.Forms.PaintEventHandler(this.label1_Paint);
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button5.Location = new System.Drawing.Point(221, 0);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(51, 42);
            this.button5.TabIndex = 14;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cueTextBox1);
            this.panel2.Controls.Add(this.button5);
            this.panel2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panel2.Location = new System.Drawing.Point(36, 553);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(290, 47);
            this.panel2.TabIndex = 15;
            // 
            // cueTextBox1
            // 
            this.cueTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cueTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cueTextBox1.Cue = "请输入IP地址";
            this.cueTextBox1.Location = new System.Drawing.Point(3, 0);
            this.cueTextBox1.Name = "cueTextBox1";
            this.cueTextBox1.Size = new System.Drawing.Size(218, 42);
            this.cueTextBox1.TabIndex = 13;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button6.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.button6.Location = new System.Drawing.Point(156, 366);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(44, 47);
            this.button6.TabIndex = 52;
            this.button6.Text = "...";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.numericUpDown3.Location = new System.Drawing.Point(36, 366);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.numericUpDown3.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(120, 47);
            this.numericUpDown3.TabIndex = 53;
            this.toolTip1.SetToolTip(this.numericUpDown3, "XXX选择按钮，显示操作");
            this.numericUpDown3.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(36, 682);
            this.trackBar1.Maximum = 65535;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(399, 69);
            this.trackBar1.TabIndex = 54;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // deleteDateTimePicker
            // 
            this.deleteDateTimePicker.CalendarFont = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.deleteDateTimePicker.CalendarForeColor = System.Drawing.Color.Tomato;
            this.deleteDateTimePicker.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.deleteDateTimePicker.Location = new System.Drawing.Point(787, 180);
            this.deleteDateTimePicker.Name = "deleteDateTimePicker";
            this.deleteDateTimePicker.Size = new System.Drawing.Size(301, 47);
            this.deleteDateTimePicker.TabIndex = 56;
            // 
            // deleteButton
            // 
            this.deleteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.deleteButton.Location = new System.Drawing.Point(1094, 125);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(190, 70);
            this.deleteButton.TabIndex = 55;
            this.deleteButton.Text = "删除此日前";
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(340, 381);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 57;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(753, 426);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(163, 30);
            this.label2.TabIndex = 60;
            this.label2.Text = "可以放大的";
            // 
            // comboBox2
            // 
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(633, 346);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(361, 47);
            this.comboBox2.TabIndex = 61;
            this.comboBox2.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(1044, 487);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(165, 54);
            this.button7.TabIndex = 63;
            this.button7.Text = "button7";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(1094, 294);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(115, 22);
            this.checkBox1.TabIndex = 64;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            this.checkBox1.Click += new System.EventHandler(this.checkBox1_Click);
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.button8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button8.Location = new System.Drawing.Point(1094, 201);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(190, 70);
            this.button8.TabIndex = 55;
            this.button8.Text = "删除此日前";
            this.button8.UseVisualStyleBackColor = false;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // defaultConfirmGroupBox
            // 
            this.defaultConfirmGroupBox.Controls.Add(this.defaultManualRadioButton);
            this.defaultConfirmGroupBox.Controls.Add(this.defaultNGRadioButton);
            this.defaultConfirmGroupBox.Controls.Add(this.defaultOKRadioButton);
            this.defaultConfirmGroupBox.Controls.Add(this.BoxComboBox);
            this.defaultConfirmGroupBox.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.defaultConfirmGroupBox.ForeColor = System.Drawing.Color.White;
            this.defaultConfirmGroupBox.Location = new System.Drawing.Point(541, 496);
            this.defaultConfirmGroupBox.Name = "defaultConfirmGroupBox";
            this.defaultConfirmGroupBox.Size = new System.Drawing.Size(215, 277);
            this.defaultConfirmGroupBox.TabIndex = 3;
            this.defaultConfirmGroupBox.TabStop = false;
            this.defaultConfirmGroupBox.Text = "默认选项";
            // 
            // defaultManualRadioButton
            // 
            this.defaultManualRadioButton.AutoSize = true;
            this.defaultManualRadioButton.Checked = true;
            this.defaultManualRadioButton.Location = new System.Drawing.Point(6, 69);
            this.defaultManualRadioButton.Name = "defaultManualRadioButton";
            this.defaultManualRadioButton.Size = new System.Drawing.Size(162, 43);
            this.defaultManualRadioButton.TabIndex = 2;
            this.defaultManualRadioButton.TabStop = true;
            this.defaultManualRadioButton.Tag = "Manual";
            this.defaultManualRadioButton.Text = "手工操作";
            this.defaultManualRadioButton.UseVisualStyleBackColor = true;
            // 
            // defaultNGRadioButton
            // 
            this.defaultNGRadioButton.AutoSize = true;
            this.defaultNGRadioButton.Location = new System.Drawing.Point(6, 195);
            this.defaultNGRadioButton.Name = "defaultNGRadioButton";
            this.defaultNGRadioButton.Size = new System.Drawing.Size(148, 43);
            this.defaultNGRadioButton.TabIndex = 1;
            this.defaultNGRadioButton.Tag = "NG";
            this.defaultNGRadioButton.Text = "默认NG";
            this.defaultNGRadioButton.UseVisualStyleBackColor = true;
            // 
            // defaultOKRadioButton
            // 
            this.defaultOKRadioButton.AutoSize = true;
            this.defaultOKRadioButton.Location = new System.Drawing.Point(6, 132);
            this.defaultOKRadioButton.Name = "defaultOKRadioButton";
            this.defaultOKRadioButton.Size = new System.Drawing.Size(145, 43);
            this.defaultOKRadioButton.TabIndex = 0;
            this.defaultOKRadioButton.Tag = "OK";
            this.defaultOKRadioButton.Text = "默认OK";
            this.defaultOKRadioButton.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(762, 518);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(232, 77);
            this.button9.TabIndex = 65;
            this.button9.Text = "button9";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(384, 36);
            this.toolStrip1.TabIndex = 66;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.Font = new System.Drawing.Font("微软雅黑", 10F);
            this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.Black;
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(110, 31);
            this.toolStripDropDownButton1.Text = "导出功能";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 820);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1356, 42);
            this.panel3.TabIndex = 67;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.toolStrip1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(972, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(384, 42);
            this.panel4.TabIndex = 67;
            // 
            // dateTimePickerBigOne1
            // 
            this.dateTimePickerBigOne1.CalendarFont = new System.Drawing.Font("宋体", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerBigOne1.CustomFormat = "MM/dd/yyyy hh:mm:ss";
            this.dateTimePickerBigOne1.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dateTimePickerBigOne1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerBigOne1.Location = new System.Drawing.Point(922, 418);
            this.dateTimePickerBigOne1.Name = "dateTimePickerBigOne1";
            this.dateTimePickerBigOne1.Size = new System.Drawing.Size(366, 42);
            this.dateTimePickerBigOne1.TabIndex = 59;
            // 
            // datePickerWithWeekNumbers1
            // 
            this.datePickerWithWeekNumbers1.CalendarFont = new System.Drawing.Font("微软雅黑", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.datePickerWithWeekNumbers1.CalendarForeColor = System.Drawing.Color.Coral;
            this.datePickerWithWeekNumbers1.DisplayWeekNumbers = false;
            this.datePickerWithWeekNumbers1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.datePickerWithWeekNumbers1.Location = new System.Drawing.Point(730, 276);
            this.datePickerWithWeekNumbers1.Name = "datePickerWithWeekNumbers1";
            this.datePickerWithWeekNumbers1.Size = new System.Drawing.Size(319, 47);
            this.datePickerWithWeekNumbers1.TabIndex = 58;
            // 
            // HelloComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.ClientSize = new System.Drawing.Size(1356, 862);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.defaultConfirmGroupBox);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePickerBigOne1);
            this.Controls.Add(this.datePickerWithWeekNumbers1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.deleteDateTimePicker);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.ipTextBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.numericUpDown2);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "HelloComboBox";
            this.Text = "HelloComboBox";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.defaultConfirmGroupBox.ResumeLayout(false);
            this.defaultConfirmGroupBox.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox BoxComboBox;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private Components.CueTextBox cueTextBox1;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DateTimePicker deleteDateTimePicker;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.ComboBox comboBox1;
        private DatePickerWithWeekNumbers datePickerWithWeekNumbers1;
        private Controls.DateTimePickerBigOne dateTimePickerBigOne1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.GroupBox defaultConfirmGroupBox;
        private System.Windows.Forms.RadioButton defaultManualRadioButton;
        private System.Windows.Forms.RadioButton defaultNGRadioButton;
        private System.Windows.Forms.RadioButton defaultOKRadioButton;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
    }
}