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
    public partial class HelloPagination : Form
    {
        public HelloPagination()
        {
            InitializeComponent();
        }
    }

    public class ImageRecord
    {
        // 主键ID
        public int Id { get; set; }
        // 产品
        public string Product { get; set; }
        // 产品SN码（序列号）
        public string SN { get; set; }
        // 检测批次号
        public int BatchNumber { get; set; }
        // 检测模组
        public int ModuleNumber { get; set; }
        // 夹具穴位
        public int Position { get; set; }
        // 光源电压
        public double Voltage { get; set; }
        // 光源电流
        public double Current { get; set; }
        // 探测器参数
        public string DetectorMode { get; set; }

        // 正面检测时间
        public DateTime FrontTime { get; set; }
        // 正面检测结果状态
        public DetectionResultStatus FrontResultStatus { get; set; }
        // 正面检测结果Points
        public string FrontResultContent { get; set; }

        // 侧面检测时间
        public DateTime SideTime { get; set; } // 时间
        // 侧面检测结果状态
        public DetectionResultStatus SideResultStatus { get; set; }
        // 侧面检测结果Points
        public string SideResultContent { get; set; }

        // 生成文件名的方法
        public static string GenerateFileName(int moduleNumber, int batchNumber, int position, Orientation Orientation, DateTime Time, string SN)
        {
            // 格式化时间为"yyyyMMdd_HHmmss"格式
            string formattedTime = Time.ToString("yyyyMMdd_HHmmss");
            // 拼接文件名
            string fileName = $"{formattedTime}-{SN}-{moduleNumber}-{batchNumber}-{position}-{Orientation}";
            return fileName;
        }
    }

    // 图像方向
    public enum Orientation
    {
        Front = 1, // 正面
        Side = 2   // 侧面
    }

    // 检测结果状态枚举
    public enum DetectionResultStatus
    {
        None,   // 检测过程异常，不能正常完成检测
        OK,     // 正常
        NG      // 图像检测有锡珠
    }

    public class ImageRecordsView
    {
        private DataGridView dataGridView;

        public delegate void SelectionChangedHandler(ImageRecord selectedRecord);
        public event SelectionChangedHandler SelectionChanged;

        public ImageRecordsView(DataGridView dgv)
        {
            this.dataGridView = dgv;
            SetDoubleBuffer(dgv);
        }

        ~ImageRecordsView()
        {
            // 释放资源
        }

        public void InitializeDataGridView()
        {
            // 设置DataGridView的基础属性以提升外观
            dataGridView.ColumnHeadersVisible = true;
            dataGridView.RowHeadersVisible = false;

            // 列全填满
            dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 只能行选，禁止编辑单元格，禁止鼠标拖动修改行高
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.ReadOnly = true;
            dataGridView.AllowUserToResizeRows = false;

            dataGridView.Columns.Clear();

            // 添加列
            dataGridView.Columns.Add("Id", "ID");
            dataGridView.Columns.Add("Product", "产品");
            dataGridView.Columns.Add("SN", "序列号");
            dataGridView.Columns.Add("BatchNumber", "批次");
            dataGridView.Columns.Add("ModuleNumber", "模组");
            dataGridView.Columns.Add("Position", "穴位");
            dataGridView.Columns.Add("Voltage", "电压");
            dataGridView.Columns.Add("Current", "电流");
            dataGridView.Columns.Add("FrontTime", "正面时间");
            dataGridView.Columns.Add("FrontResultStatus", "正面结果");
            dataGridView.Columns.Add("SideTime", "侧面时间");
            dataGridView.Columns.Add("SideResultStatus", "侧面结果");

            // Adjust column widths as needed
            dataGridView.Columns["Id"].Width = 60;
            dataGridView.Columns["Product"].Width = 80;
            dataGridView.Columns["SN"].Width = dataGridView.Parent.Width - 860 - 50;
            dataGridView.Columns["BatchNumber"].Width = 60;
            dataGridView.Columns["ModuleNumber"].Width = 60;
            dataGridView.Columns["Position"].Width = 60;
            dataGridView.Columns["Voltage"].Width = 60;
            dataGridView.Columns["Current"].Width = 60;
            dataGridView.Columns["FrontTime"].Width = 130;
            dataGridView.Columns["FrontResultStatus"].Width = 80;
            dataGridView.Columns["SideTime"].Width = 130;
            dataGridView.Columns["SideResultStatus"].Width = 80;

            CustomizeDataGridView();

            dataGridView.CellPainting += DataGridView_CellPainting;
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        private void CustomizeDataGridView()
        {
            // 设置列头的默认样式为居中对齐
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView.ColumnHeadersHeight = 50;

            //// 设置列头和单元格的字体大小和字体样式
            //var font = new Font(dataGridView.Font.FontFamily, 15, FontStyle.Regular); // 创建一个新的字体实例，字号设置为12，你可以根据需要调整字号大小
            //dataGridView.ColumnHeadersDefaultCellStyle.Font = font; // 应用字体到列头
            //dataGridView.DefaultCellStyle.Font = font; // 应用字体到单元格

            // 适用于已经添加的列，确保它们的列头也是居中对齐
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 设置 DataGridView 的背景颜色
            dataGridView.BackgroundColor = Color.FromArgb(38, 38, 38);

            // 设置边框颜色和样式
            dataGridView.GridColor = Color.White;
            dataGridView.BorderStyle = BorderStyle.FixedSingle;

            // 禁用行头和列头的视觉样式，以便自定义颜色
            dataGridView.EnableHeadersVisualStyles = false;

            // 设置列头的样式
            dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
            dataGridView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 设置行头的样式（如果你使用行头）
            dataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
            dataGridView.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 设置单元格的默认样式
            dataGridView.DefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
            dataGridView.DefaultCellStyle.ForeColor = Color.White;
            dataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(58, 58, 58); // 设置选中单元格的背景颜色
            dataGridView.DefaultCellStyle.SelectionForeColor = Color.White;

            // 设置网格线的颜色
            dataGridView.GridColor = Color.White;

            // 其他美化设置...
        }

        // 这里应该有一个方法来生成或获取ImageRecord的列表
        // 填充DataGridView
        public void PopulateDataGridView(List<ImageRecord> records)
        {
            try
            {
                dataGridView.SuspendLayout(); // 暂停布局逻辑
                dataGridView.Visible = false; // 隐藏DataGridView以减少闪烁

                // 禁用用户添加行
                dataGridView.AllowUserToAddRows = false;

                // 清除现有的行
                dataGridView.Rows.Clear();

                foreach (var record in records)
                {
                    // 将每个ImageRecord的属性添加到DataGridView的新行中
                    int rowIndex = dataGridView.Rows.Add(record.Id, record.Product, record.SN, record.BatchNumber, record.ModuleNumber, record.Position,
                                                         record.Voltage, record.Current,
                                                         record.FrontTime.ToString("yyyy-MM-dd HH:mm:ss"), record.FrontResultStatus,
                                                         record.SideTime.ToString("yyyy-MM-dd HH:mm:ss"), record.SideResultStatus
                                                         );

                    // 将当前的ImageRecord对象存储在新添加行的Tag属性中
                    dataGridView.Rows[rowIndex].Tag = record;
                    dataGridView.Rows[rowIndex].Height = 33;
                }
            }
            finally
            {
                dataGridView.Visible = true; // 数据填充完成后重新显示DataGridView
                dataGridView.ResumeLayout(true); // 恢复布局逻辑并强制一个重新布局
            }
        }

        private void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex >= 0 && (e.ColumnIndex == 9 || e.ColumnIndex == 11))
            {
                // 通过平滑模式使圆形边缘更平滑
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                if (e.Value != null)
                {
                    DetectionResultStatus status = (DetectionResultStatus)e.Value;

                    Color color = status == DetectionResultStatus.OK ? Color.Green : status == DetectionResultStatus.NG ? Color.Red : Color.Gray;

                    // 动态计算圆的直径为单元格高度和宽度的较小者减去一定的边距，以保证圆形在单元格中居中且不触碰边界
                    int margin = 12; // 边距
                    int diameter = Math.Min(e.CellBounds.Width, e.CellBounds.Height) - margin;
                    int radius = diameter / 2;

                    using (Brush brush = new SolidBrush(color))
                    {
                        e.Graphics.FillEllipse(brush,
                            e.CellBounds.Left + (e.CellBounds.Width / 2) - radius,
                            e.CellBounds.Top + (e.CellBounds.Height / 2) - radius,
                            diameter, diameter);
                    }
                }

                e.Handled = true;
            }

        }

        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                var selectedRecord = selectedRow.Tag as ImageRecord;
                if (selectedRecord != null)
                {
                    SelectionChanged?.Invoke(selectedRecord);
                }
            }
        }

        public static void SetDoubleBuffer(object obj)
        {
            //DataGridView
            PropertyInfo Property = obj.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            Property?.SetValue(obj, true, null);
        }
    }

}
