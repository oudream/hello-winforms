using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloDataGridViewKV : Form
    {
        private DataGridView dataGridView1;

        public HelloDataGridViewKV()
        {
            InitializeComponent();

            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            var point1 = new System.Drawing.Point(8, 8);
            dataGridView1 = new DataGridView
            {
                Location = point1,
                Size = new Size(784, 440),
                AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing,
                ColumnHeadersHeight = 60,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Arial", 16, FontStyle.Bold),
                    BackColor = Color.Navy,
                    ForeColor = Color.White
                },
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Font = new Font("Arial", 14),
                    ForeColor = Color.Black
                },
                RowTemplate = { Height = 50 },
                AllowUserToAddRows = false
            };

            Controls.Add(dataGridView1);

            SetupDataGridView();
            PopulateDataGridView();
        }

        private void SetupDataGridView()
        {
            dataGridView1.ColumnCount = 2;

            dataGridView1.Columns[0].Name = "配置项";
            dataGridView1.Columns[0].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.Columns[0].Width = 200;

            dataGridView1.Columns[1].Name = "配置值";
            dataGridView1.Columns[1].DefaultCellStyle.BackColor = Color.LightYellow;
            dataGridView1.Columns[1].Width = 300;
        }

        private void PopulateDataGridView()
        {
            AddRow("Resolution", "1920x1080");
            AddRow("Brightness", "75%");
            AddRow("Contrast", "50%");
            AddRow("Volume", "80%");
        }

        private void AddRow(string item, string value)
        {
            string[] row = { item, value };
            dataGridView1.Rows.Add(row);
        }
    }
}
