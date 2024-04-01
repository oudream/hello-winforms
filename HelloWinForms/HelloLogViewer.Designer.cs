namespace HelloWinForms
{
    partial class HelloLogViewer
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
            this.funControl = new System.Windows.Forms.Panel();
            this.logDataGridView = new System.Windows.Forms.DataGridView();
            this.panel7 = new System.Windows.Forms.Panel();
            this.tagComboBox = new System.Windows.Forms.ComboBox();
            this.tagLabel = new System.Windows.Forms.Label();
            this.clearTagButton = new System.Windows.Forms.Button();
            this.addTagButton = new System.Windows.Forms.Button();
            this.tagCheckBox = new System.Windows.Forms.CheckBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.filterComboBox = new System.Windows.Forms.ComboBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.levelWithinCheckBox = new System.Windows.Forms.CheckBox();
            this.funControl.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logDataGridView)).BeginInit();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // funControl
            // 
            this.funControl.Controls.Add(this.logDataGridView);
            this.funControl.Controls.Add(this.panel7);
            this.funControl.Controls.Add(this.panel5);
            this.funControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.funControl.Location = new System.Drawing.Point(0, 0);
            this.funControl.Margin = new System.Windows.Forms.Padding(2);
            this.funControl.Name = "funControl";
            this.funControl.Size = new System.Drawing.Size(800, 450);
            this.funControl.TabIndex = 2;
            // 
            // logDataGridView
            // 
            this.logDataGridView.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.logDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.logDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logDataGridView.GridColor = System.Drawing.Color.White;
            this.logDataGridView.Location = new System.Drawing.Point(0, 109);
            this.logDataGridView.Name = "logDataGridView";
            this.logDataGridView.RowHeadersWidth = 62;
            this.logDataGridView.RowTemplate.Height = 30;
            this.logDataGridView.Size = new System.Drawing.Size(800, 341);
            this.logDataGridView.TabIndex = 9;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.tagComboBox);
            this.panel7.Controls.Add(this.tagLabel);
            this.panel7.Controls.Add(this.clearTagButton);
            this.panel7.Controls.Add(this.addTagButton);
            this.panel7.Controls.Add(this.tagCheckBox);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 40);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(800, 69);
            this.panel7.TabIndex = 8;
            // 
            // tagComboBox
            // 
            this.tagComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.tagComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tagComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tagComboBox.ForeColor = System.Drawing.Color.White;
            this.tagComboBox.Location = new System.Drawing.Point(104, 3);
            this.tagComboBox.Name = "tagComboBox";
            this.tagComboBox.Size = new System.Drawing.Size(577, 26);
            this.tagComboBox.TabIndex = 1;
            // 
            // tagLabel
            // 
            this.tagLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tagLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(38)))), ((int)(((byte)(38)))), ((int)(((byte)(38)))));
            this.tagLabel.ForeColor = System.Drawing.Color.White;
            this.tagLabel.Location = new System.Drawing.Point(6, 37);
            this.tagLabel.Name = "tagLabel";
            this.tagLabel.Size = new System.Drawing.Size(791, 29);
            this.tagLabel.TabIndex = 4;
            this.tagLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // clearTagButton
            // 
            this.clearTagButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.clearTagButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.clearTagButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.clearTagButton.ForeColor = System.Drawing.Color.White;
            this.clearTagButton.Location = new System.Drawing.Point(740, 0);
            this.clearTagButton.Name = "clearTagButton";
            this.clearTagButton.Size = new System.Drawing.Size(57, 34);
            this.clearTagButton.TabIndex = 3;
            this.clearTagButton.Text = "清空";
            this.clearTagButton.UseVisualStyleBackColor = false;
            // 
            // addTagButton
            // 
            this.addTagButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addTagButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.addTagButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.addTagButton.ForeColor = System.Drawing.Color.White;
            this.addTagButton.Location = new System.Drawing.Point(683, 0);
            this.addTagButton.Name = "addTagButton";
            this.addTagButton.Size = new System.Drawing.Size(57, 34);
            this.addTagButton.TabIndex = 3;
            this.addTagButton.Text = "添加";
            this.addTagButton.UseVisualStyleBackColor = false;
            // 
            // tagCheckBox
            // 
            this.tagCheckBox.AutoSize = true;
            this.tagCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.tagCheckBox.Checked = true;
            this.tagCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tagCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.tagCheckBox.ForeColor = System.Drawing.Color.White;
            this.tagCheckBox.Location = new System.Drawing.Point(6, 5);
            this.tagCheckBox.Name = "tagCheckBox";
            this.tagCheckBox.Size = new System.Drawing.Size(93, 22);
            this.tagCheckBox.TabIndex = 2;
            this.tagCheckBox.Text = "Any标签";
            this.tagCheckBox.UseVisualStyleBackColor = false;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.filterComboBox);
            this.panel5.Controls.Add(this.clearButton);
            this.panel5.Controls.Add(this.levelWithinCheckBox);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(800, 40);
            this.panel5.TabIndex = 6;
            // 
            // filterComboBox
            // 
            this.filterComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.filterComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.filterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.filterComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.filterComboBox.ForeColor = System.Drawing.Color.White;
            this.filterComboBox.Location = new System.Drawing.Point(175, 6);
            this.filterComboBox.Name = "filterComboBox";
            this.filterComboBox.Size = new System.Drawing.Size(622, 26);
            this.filterComboBox.TabIndex = 1;
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.clearButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.clearButton.ForeColor = System.Drawing.Color.White;
            this.clearButton.Location = new System.Drawing.Point(4, 3);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(57, 34);
            this.clearButton.TabIndex = 3;
            this.clearButton.Text = "清除";
            this.clearButton.UseVisualStyleBackColor = false;
            // 
            // levelWithinCheckBox
            // 
            this.levelWithinCheckBox.AutoSize = true;
            this.levelWithinCheckBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(93)))), ((int)(((byte)(93)))), ((int)(((byte)(93)))));
            this.levelWithinCheckBox.Checked = true;
            this.levelWithinCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.levelWithinCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.levelWithinCheckBox.ForeColor = System.Drawing.Color.White;
            this.levelWithinCheckBox.Location = new System.Drawing.Point(67, 8);
            this.levelWithinCheckBox.Name = "levelWithinCheckBox";
            this.levelWithinCheckBox.Size = new System.Drawing.Size(102, 22);
            this.levelWithinCheckBox.TabIndex = 2;
            this.levelWithinCheckBox.Text = "等级以内";
            this.levelWithinCheckBox.UseVisualStyleBackColor = false;
            // 
            // HelloLogViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.funControl);
            this.Name = "HelloLogViewer";
            this.Text = "HelloLogViewer";
            this.funControl.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logDataGridView)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel funControl;
        private System.Windows.Forms.DataGridView logDataGridView;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.ComboBox tagComboBox;
        private System.Windows.Forms.Label tagLabel;
        private System.Windows.Forms.Button clearTagButton;
        private System.Windows.Forms.Button addTagButton;
        private System.Windows.Forms.CheckBox tagCheckBox;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.ComboBox filterComboBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.CheckBox levelWithinCheckBox;
    }
}