﻿namespace HelloWinForms
{
    partial class HelloExeInForm
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
            this.SuspendLayout();
            // 
            // HelloExeInForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1260, 895);
            this.Name = "HelloExeInForm";
            this.Text = "HelloExeInForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.HelloExeInForm_FormClosing);
            this.Load += new System.EventHandler(this.HelloExeInForm_Load);
            this.Resize += new System.EventHandler(this.HelloExeInForm_Resize);
            this.ResumeLayout(false);

        }

        #endregion
    }
}