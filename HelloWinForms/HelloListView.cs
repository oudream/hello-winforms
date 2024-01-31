using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace HelloWinForms
{
    public partial class HelloListView : Form
    {
        public HelloListView()
        {
            InitializeComponent();

            // 设置显示方式为详细信息
            listView.View = View.LargeIcon;

            // 启用列头点击排序
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            // 添加 ColumnHeaderMouseClick 事件处理程序
            dataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;

        }

        private ImageList _imageList;

        private void button1_Click(object sender, EventArgs e)
        {
            // 添加列
            listView.Columns.Add("图像", 100);
            listView.Columns.Add("列1", 100);
            listView.Columns.Add("列2", 100);
            listView.Columns.Add("列3", 100);

            // 创建 ImageList 控件用于存储图像
            _imageList = new ImageList();
            _imageList.ImageSize = new Size(128, 128); // 设置图像大小

            // 将图像添加到 ImageList 中
            _imageList.Images.Add("imageKey1", Image.FromFile("C:\\Users\\Administrator\\Pictures\\logo.png"));
            _imageList.Images.Add("imageKey2", Image.FromFile("C:\\Users\\Administrator\\Pictures\\s14.png"));
            // 将 ImageList 分配给 ListView
            listView.LargeImageList = _imageList;
            listView.SmallImageList = _imageList;


            ListViewItem item1 = new ListViewItem(new[] { "imageKey1\r\n行1列1", "行1列1", "行1列2", "行1列3" }, "imageKey1");
            ListViewItem item2 = new ListViewItem(new[] { "imageKey2\r\n行2列1", "行2列1", "行2列2", "行2列3" }, "imageKey2");
            listView.Items.Add(item1);
            listView.Items.Add(item2);

            // 添加项
            //ListViewItem item1 = new ListViewItem(new[] { "imageKey1", "行1列1", "行1列2", "行1列3" });
            //ListViewItem item2 = new ListViewItem(new[] { "imageKey2", "行2列1", "行2列2", "行2列3" });

            //listView.Items.Add(item1);
            //listView.Items.Add(item2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 添加列
            // 添加列
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.HeaderText = "图像";
            imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom; // 拉伸图像以填充整个单元格
            dataGridView.Columns.Add(imageColumn);
            dataGridView.Columns.Add("列1", "列1");
            dataGridView.Columns.Add("列2", "列2");
            dataGridView.Columns.Add("列3", "列3");

            dataGridView.Columns["列2"].Width = 150; // 设置为适当的宽度

            // 添加行
            DataGridViewRow row1 = new DataGridViewRow();
            row1.CreateCells(dataGridView, Image.FromFile("C:\\Users\\Administrator\\Pictures\\logo.png"), "行1列1", "行1列2", "行1列3");

            DataGridViewRow row2 = new DataGridViewRow();
            row2.CreateCells(dataGridView, Image.FromFile("C:\\Users\\Administrator\\Pictures\\s14.png"), "行2列1", "行2列2", "行2列3");

            dataGridView.Rows.Add(row1);
            dataGridView.Rows.Add(row2);

           

        }

        // ColumnHeaderMouseClick 事件处理程序
        private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 检查点击的是列标题
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                DataGridViewColumn clickedColumn = dataGridView.Columns[e.ColumnIndex];
                clickedColumn.SortMode = DataGridViewColumnSortMode.Programmatic;

                // 切换排序方向
                if (clickedColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
                {
                    clickedColumn.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                }
                else
                {
                    clickedColumn.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                }

                // 执行排序
                dataGridView.Sort(clickedColumn, clickedColumn.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView.LargeImageList.Images[0] = Image.FromFile("C:\\Users\\Administrator\\Pictures\\logo3.png");
            listView.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // 遍历 GroupBox 中的 RadioButton
            foreach (System.Windows.Forms.RadioButton radioButton in viewStyleGroupBox.Controls.OfType<System.Windows.Forms.RadioButton>())
            {
                // 检查每个 RadioButton 是否被选中
                if (radioButton.Checked)
                {
                    // 当前选中的 RadioButton 的文本
                    string selectedText = radioButton.Text;

                    // 可以在这里进行相应的操作
                    MessageBox.Show($"当前选中的是：{selectedText}");
                }
            }
        }

        private void bigRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (!bigRadioButton.Checked)
            {

            }
        }

        private void detailRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            MessageBox.Show($"当前选中的是：");
        }

        private void listView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            // 默认绘制，如果需要
            e.DrawDefault = true;
        }
    }
}
