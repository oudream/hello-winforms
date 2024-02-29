using CxWinFormsComponents.DataGridViewColumns;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloDataGridView : Form
    {
        public HelloDataGridView()
        {
            InitializeComponent();

            // 初始化上下文菜单
            InitializeContextMenu();

            dataGridView1.CellPainting += dataGridView1_CellPainting;
        }

        // DataGridViewCalendarColumn
        // DataGridViewCheckBoxColumn
        // DataGridViewComboBoxColumn
        // DataGridViewImageColumn 
        // DataGridViewLinkColumn
        // DataGridViewNumbericColumn
        // DataGridViewTextBoxColumn
        private void button1_Click(object sender, EventArgs e)
        {
            // 不自动调整列宽
            dataGridView1.AutoGenerateColumns = false;

            var paymentAmountColumn = new DataGridViewTextBoxColumn
            {
                Name = "PaymentAmount",
                DataPropertyName = "PaymentAmount",
                HeaderText = "项目金额",
                Width = 100
            };
            paymentAmountColumn.DefaultCellStyle.Format = "0.00";
            //paymentAmountColumn.SortMode = DataGridViewColumnSortMode.Automatic;

            // 列
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "GroupColumn", HeaderText = "分组", Width = 100 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProjectId", DataPropertyName = "ProjectId", HeaderText = "项目 ID", Width = 100});
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProjectName", DataPropertyName = "ProjectName", HeaderText = "项目名称", Width = 200 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "ProjectGroup", DataPropertyName = "ProjectGroup", HeaderText = "项目组", Width = 200 });
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn { Name = "Description", DataPropertyName = "Description", HeaderText = "描述", Width = 200 });
            dataGridView1.Columns.Add(paymentAmountColumn);
            dataGridView1.Columns.Add(new DataGridViewCalendarColumn { Name = "CreationTime", DataPropertyName = "CreationTime", HeaderText = "创建时间", Width = 150, SortMode = DataGridViewColumnSortMode.Programmatic });
            
            // 设置数据源为 ProjectConfig 对象的列表
            List<ProjectConfig> projectConfigs = GenerateProjectConfigs();
            // 使用 BindingList<T> 作为数据源，因为 BindingList<T> 支持排序
            //BindingList<ProjectConfig> projectConfigs = new BindingList<ProjectConfig>(GenerateProjectConfigs());
            dataGridView1.DataSource = projectConfigs;

            BeautifyDataGridView(dataGridView1);
        }

        private List<ProjectConfig> GenerateProjectConfigs()
        {
            var projectConfigs = new List<ProjectConfig>();
            for (int i = 1; i <= 30; i++)
            {
                projectConfigs.Add(new ProjectConfig
                {
                    ProjectId = $"ID{i}",
                    ProjectName = $"Project {i}",
                    ProjectGroup = $"Group {i/10}",
                    Description = $"Description for Project {i}",
                    PaymentAmount = i * 10000.12,
                    CreationTime = DateTime.Now.AddDays(i),
                });
            }
            return projectConfigs;
        }

        private string BuildProjectConfigsMessage(List<ProjectConfig> projectConfigs)
        {
            var message = new StringBuilder();

            foreach (var config in projectConfigs)
            {
                message.AppendLine($"ID: {config.ProjectId}, Name: {config.ProjectName}, Group: {config.ProjectGroup}, Description: {config.Description}");
            }

            return message.ToString();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            var projectConfigs = (List<ProjectConfig>)dataGridView1.DataSource;
            var message = BuildProjectConfigsMessage(projectConfigs);
            MessageBox.Show(message, "Project Configs Information");
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
        }

        private void dataGridView1_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            e.Row.HeaderCell.Value = string.Format("{0}", e.Row.Index + 1);
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //if (e.ColumnIndex == dataGridView1.Columns["CreationTime"].Index)
            //{
            //    dataGridView1.BeginEdit(true); // 强制开始编辑
            //}
        }

        private void InitializeContextMenu()
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("删除");
            deleteItem.Click += DeleteItem_Click;
            contextMenu.Items.Add(deleteItem);
            dataGridView1.ContextMenuStrip = contextMenu;
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.DataSource is List<ProjectConfig> data)
                {
                    // 获取选中的行
                    var selectedRows = dataGridView1.SelectedRows;

                    // 由于你不能在遍历时修改集合，所以先复制要删除的项
                    var rowsToDelete = new List<ProjectConfig>();

                    foreach (DataGridViewRow row in selectedRows)
                    {
                        if (row.DataBoundItem is ProjectConfig item)
                        {
                            rowsToDelete.Add(item);
                        }
                    }

                    // 从数据源中删除这些项
                    foreach (var item in rowsToDelete)
                    {
                        data.Remove(item);
                    }

                    // 更新 DataGridView
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource = data;
                }
            }
            else
            {
                MessageBox.Show("请选择要删除的行。");
            }
        }

        private void dataGridView1_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {

        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // 假设你只想在特定的列（例如第一列）中进行浮点数验证
            if (e.ColumnIndex == 3)
            {
                if (!float.TryParse(e.FormattedValue.ToString(), out float _))
                {
                    // 如果转换失败，显示错误消息并取消更改
                    dataGridView1.Rows[e.RowIndex].ErrorText = "请输入有效的浮点数";
                    e.Cancel = true;
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // 清除错误消息
            dataGridView1.Rows[e.RowIndex].ErrorText = null;
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridViewColumn column = dataGridView1.Columns[e.ColumnIndex];
            SortDataGridView(column);
        }

        private void SortDataGridView(DataGridViewColumn column)
        {
            if (dataGridView1.DataSource is List<ProjectConfig> data)
            {
                // 判断当前排序方向
                SortOrder sortOrder = column.HeaderCell.SortGlyphDirection == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;

                // 根据列名选择排序逻辑
                switch (column.Name)
                {
                    case "ProjectId":
                        data = sortOrder == SortOrder.Ascending ? data.OrderBy(p => p.ProjectId).ToList() : data.OrderByDescending(p => p.ProjectId).ToList();
                        break;
                    case "ProjectName":
                        data = sortOrder == SortOrder.Ascending ? data.OrderBy(p => p.ProjectName).ToList() : data.OrderByDescending(p => p.ProjectName).ToList();
                        break;
                    case "ProjectGroup":
                        data = sortOrder == SortOrder.Ascending ? data.OrderBy(p => p.ProjectGroup).ToList() : data.OrderByDescending(p => p.ProjectGroup).ToList();
                        break;
                    case "CreationTime":
                        data = sortOrder == SortOrder.Ascending ? data.OrderBy(p => p.CreationTime).ToList() : data.OrderByDescending(p => p.CreationTime).ToList();
                        break;
                    case "PaymentAmount":
                        data = sortOrder == SortOrder.Ascending ? data.OrderBy(p => p.PaymentAmount).ToList() : data.OrderByDescending(p => p.PaymentAmount).ToList();
                        break;
                        // 可以添加更多的 case 来处理其他列
                }

                // 更新数据源
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = data;

                // 更新排序图标
                column.HeaderCell.SortGlyphDirection = sortOrder;
            }
        }

        private void BeautifyDataGridView(DataGridView dgv)
        {
            // 设置列宽和行高
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersHeight = 38;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dgv.RowTemplate.Height = 30;


            // 更改字体和颜色
            dgv.DefaultCellStyle.Font = new Font("Arial", 10);
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.LightGray;

            // 设置网格线样式
            dgv.GridColor = Color.Gray;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            // 列标题的样式
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;

            // 选择和高亮样式
            dgv.DefaultCellStyle.SelectionBackColor = Color.DarkBlue;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // 启用双缓冲以提高性能
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, true, null);
        }

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 这里仅对"GroupColumn"列进行处理
            if (e.ColumnIndex == dataGridView1.Columns["GroupColumn"].Index && e.RowIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                e.Handled = true;

                var projectGroup = dataGridView1.Rows[e.RowIndex].Cells["ProjectGroup"].Value.ToString();

                // 检查当前行是否为同组的第一行
                if (e.RowIndex == 0 || projectGroup != dataGridView1.Rows[e.RowIndex - 1].Cells["ProjectGroup"].Value.ToString())
                {
                    // 计算这个组跨越多少行
                    int rowCount = 1;
                    while (e.RowIndex + rowCount < dataGridView1.RowCount &&
                           projectGroup == dataGridView1.Rows[e.RowIndex + rowCount].Cells["ProjectGroup"].Value.ToString())
                    {
                        rowCount++;
                    }

                    // 合并单元格的边界
                    Rectangle groupCellBounds = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
                    int totalHeight = 0;
                    for (int i = 0; i < rowCount; i++)
                    {
                        totalHeight += dataGridView1.GetRowDisplayRectangle(e.RowIndex + i, true).Height;
                    }

                    groupCellBounds.Height = totalHeight;

                    // 绘制文本
                    TextRenderer.DrawText(e.Graphics, projectGroup, e.CellStyle.Font,
                        groupCellBounds, e.CellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
                }
            }
        }

    }

    public class ProjectConfig
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectGroup { get; set; }
        public string Description { get; set; }
        // 添加创建时间属性
        public DateTime CreationTime { get; set; }
        // 添加付款金额属性
        public double PaymentAmount { get; set; }
    }

}
