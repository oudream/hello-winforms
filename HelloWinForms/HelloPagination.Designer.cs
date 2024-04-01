namespace HelloWinForms
{
    partial class HelloPagination
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelloPagination));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pagerControl1 = new HelloWinForms.Controls.PagerControl();
            this.dateScrollerControl1 = new HelloWinForms.Controls.DateScrollerControl();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 152);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 30;
            this.dataGridView1.Size = new System.Drawing.Size(1326, 577);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.pagerControl1);
            this.panel1.Location = new System.Drawing.Point(12, 735);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1326, 100);
            this.panel1.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.Controls.Add(this.dateScrollerControl1);
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1326, 134);
            this.panel2.TabIndex = 2;
            // 
            // pagerControl1
            // 
            this.pagerControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.pagerControl1.DataSource = ((System.Collections.Generic.List<object>)(resources.GetObject("pagerControl1.DataSource")));
            this.pagerControl1.Location = new System.Drawing.Point(3, 14);
            this.pagerControl1.Name = "pagerControl1";
            this.pagerControl1.PageCount = 0;
            this.pagerControl1.PageIndex = 1;
            this.pagerControl1.PageSize = 10;
            this.pagerControl1.Size = new System.Drawing.Size(1320, 70);
            this.pagerControl1.StartIndex = 0;
            this.pagerControl1.TabIndex = 0;
            // 
            // dateScrollerControl1
            // 
            this.dateScrollerControl1.Location = new System.Drawing.Point(3, 0);
            this.dateScrollerControl1.Name = "dateScrollerControl1";
            this.dateScrollerControl1.Size = new System.Drawing.Size(615, 128);
            this.dateScrollerControl1.TabIndex = 0;
            // 
            // HelloPagination
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1350, 845);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "HelloPagination";
            this.Text = "HelloPagination";
            this.Load += new System.EventHandler(this.HelloPagination_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private Controls.PagerControl pagerControl1;
        private System.Windows.Forms.Panel panel2;
        private Controls.DateScrollerControl dateScrollerControl1;
    }
}