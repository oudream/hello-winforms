namespace HelloWinForms
{
    partial class HelloVariable
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
            this.multiThreadReadVarList = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // multiThreadReadVarList
            // 
            this.multiThreadReadVarList.Location = new System.Drawing.Point(12, 12);
            this.multiThreadReadVarList.Name = "multiThreadReadVarList";
            this.multiThreadReadVarList.Size = new System.Drawing.Size(149, 78);
            this.multiThreadReadVarList.TabIndex = 0;
            this.multiThreadReadVarList.Text = "Multi Thread Read VarList";
            this.multiThreadReadVarList.UseVisualStyleBackColor = true;
            this.multiThreadReadVarList.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 656);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "label1";
            // 
            // HelloVariable
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1286, 686);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.multiThreadReadVarList);
            this.Name = "HelloVariable";
            this.Text = "HelloVariable";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button multiThreadReadVarList;
        private System.Windows.Forms.Label label1;
    }
}