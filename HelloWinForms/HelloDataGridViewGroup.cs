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
    public partial class HelloDataGridViewGroup : Form
    {
        public HelloDataGridViewGroup()
        {
            InitializeComponent();
            InitializeDataGridView();
        }
        private void InitializeDataGridView()
        {
            // 配置DataGridView
            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "Name";
            dataGridView1.Columns[1].Name = "Description";
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
            dataGridView1.RowPrePaint += DataGridView1_RowPrePaint;

            // 添加数据
            var items = new List<Item>
            {
                new Item { Name = "Item 1", Description = "Description 1", Level = 0 },
                new Item { Name = "Item 1.1", Description = "Description 1.1", Level = 1 },
                new Item { Name = "Item 1.2", Description = "Description 1.2", Level = 1 },
                new Item { Name = "Item 2", Description = "Description 2", Level = 0 },
                new Item { Name = "Item 2.1", Description = "Description 2.1", Level = 1 }
            };

            foreach (var item in items)
            {
                dataGridView1.Rows.Add(item.Name, item.Description);
            }
        }

        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.RowIndex >= 0) // 只在名称列添加缩进
            {
                var value = dataGridView1.Rows[e.RowIndex].Cells[0].Value;
                if (value == null) return;
                var item = new Item { Level = (e.RowIndex % 3 == 0) ? 0 : 1 }; // 示例中简单地模拟层级
                var indent = 20 * item.Level;
                var newRect = new Rectangle(e.CellBounds.X + indent, e.CellBounds.Y, e.CellBounds.Width - indent, e.CellBounds.Height);
                e.PaintBackground(e.ClipBounds, true);
                e.Graphics.DrawString(value.ToString(), e.CellStyle.Font, Brushes.Black, newRect);
                e.Handled = true;
            }
        }

        private void DataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            // 这里可以根据需要添加代码，例如设置行颜色等
        }

        class Item
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Level { get; set; }
        }
    }
}
