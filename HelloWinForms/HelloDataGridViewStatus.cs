using CommonInterfaces;
using CxWorkStation.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloDataGridViewStatus : Form
    {
        private PredictionStatusView detectionStatusView;

        private int _count;

        public HelloDataGridViewStatus()
        {
            InitializeComponent();

            detectionStatusView = new PredictionStatusView(this.dataGridView1);
            for (int i = 0; i < 50; i++)
            {
                detectionStatusView.AddStatus(new PredictionRecord($"SN1237abc123891278898676ffab{_count++}", PredictionStatus.None, PredictionStatus.NG, DateTime.Now));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            detectionStatusView.UpdateFrontStatus($"SN1237abc123891278898676ffab{48}", PredictionStatus.NG);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            detectionStatusView.UpdateSideStatus($"SN1237abc123891278898676ffab{48}", PredictionStatus.OK);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            detectionStatusView.AddStatus(new PredictionRecord($"SN1237abc123891278898676ffab{_count++}", PredictionStatus.None, PredictionStatus.NG, DateTime.Now));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Random random = new Random();

            // 随机选择一个记录的索引
            int recordIndex = random.Next(detectionStatusView.records.Count);

            // 随机选择状态更新（正面或侧面）
            bool updateFront = random.Next(2) == 0;

            // 随机选择一个新的状态
            PredictionStatus[] statuses = { PredictionStatus.None, PredictionStatus.OK, PredictionStatus.NG };
            PredictionStatus newStatus = statuses[random.Next(statuses.Length)];

            // 获取随机选中记录的SN码
            string sn = detectionStatusView.records[recordIndex].SN;

            // 根据随机决定更新正面还是侧面状态
            if (updateFront)
            {
                detectionStatusView.UpdateFrontStatus(sn, newStatus);
            }
            else
            {
                detectionStatusView.UpdateSideStatus(sn, newStatus);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            Random random = new Random();

            // 随机选择一个记录的索引
            int recordIndex = random.Next(detectionStatusView.records.Count);

            // 随机选择状态更新（正面或侧面）
            bool updateFront = random.Next(2) == 0;

            // 随机选择一个新的状态
            PredictionStatus[] statuses = { PredictionStatus.None, PredictionStatus.OK, PredictionStatus.NG };
            PredictionStatus frontStatus = statuses[random.Next(statuses.Length)];
            PredictionStatus sideStatus = statuses[random.Next(statuses.Length)];
            detectionStatusView.AddStatus(new PredictionRecord($"SN1237abc123891278898676ffab{_count++}", frontStatus, sideStatus, DateTime.Now));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            long ms = 1710484494224L;
            txtResult.Text = $"当前时间: {TimeHelper.GetDateTime(ms)}";
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void btnGetNow_Click(object sender, EventArgs e)
        {
            long nowTimestamp = TimeHelper.GetNow();
            txtResult.Text = $"当前时间戳（毫秒）: {nowTimestamp}";
        }

        private void btnGetDateTimeString_Click(object sender, EventArgs e)
        {
            // 使用GetNow获取当前时间的时间戳，然后转换为字符串
            string dateTimeString = TimeHelper.GetDateTimeString(TimeHelper.GetNow());
            txtResult.Text = $"当前日期和时间: {dateTimeString}";
        }

        private void btnGetDateTime_Click(object sender, EventArgs e)
        {
            DateTime dateTime = TimeHelper.GetDateTime(TimeHelper.GetNow());
            txtResult.Text = $"当前日期和时间（DateTime对象）: {dateTime}";
        }

        private void btnGetNowTimeString_Click(object sender, EventArgs e)
        {
            string nowTimeString = TimeHelper.GetNowTimeString();
            txtResult.Text = $"当前时间: {nowTimeString}";
        }

        private void btnGetMs_Click(object sender, EventArgs e)
        {
            long ms = TimeHelper.GetMs(DateTime.Now);
            txtResult.Text = $"当前DateTime转换为毫秒: {ms}";
        }

        private void btnTryParseDateTime_Click(object sender, EventArgs e)
        {
            if (TimeHelper.TryParseDateTime("2024-03-15 12:34:56.789", out DateTime dateTime))
            {
                txtResult.Text = $"解析的DateTime: {dateTime}";
            }
            else
            {
                txtResult.Text = "解析失败";
            }
        }

    }

    public class PredictionRecord
    {
        public string SN { get; set; }
        public PredictionStatus FrontStatus { get; set; }
        public PredictionStatus SideStatus { get; set; }
        public DateTime PredictionTime { get; set; }

        public PredictionRecord(string sn, PredictionStatus frontStatus, PredictionStatus sideStatus, DateTime predictionTime)
        {
            SN = sn;
            FrontStatus = frontStatus;
            SideStatus = sideStatus;
            PredictionTime = predictionTime;
        }
    }

    public enum PredictionStatus
    {
        None,
        OK,
        NG
    }

    public class PredictionStatusView
    {
        private DataGridView dataGridView;
        public List<PredictionRecord> records;

        public PredictionStatusView(DataGridView dgv)
        {
            this.dataGridView = dgv;
            this.records = new List<PredictionRecord>();

            InitializeDataGridView();
        }

        private void InitializeDataGridView()
        {
            dataGridView.AutoGenerateColumns = false; // Disable auto column generation
            dataGridView.AllowUserToAddRows = false; // Prevent user from adding rows
            dataGridView.RowHeadersVisible = false; // Hide the row headers

            dataGridView.ColumnCount = 4;
            dataGridView.Columns[0].Name = "时间";
            dataGridView.Columns[1].Name = "SN码";
            dataGridView.Columns[2].Name = "正";
            dataGridView.Columns[3].Name = "侧";

            // Adjust column widths as needed
            dataGridView.Columns[0].Width = 80;
            dataGridView.Columns[1].Width = dataGridView.Parent.Width - 50 - 50 - 80 - 20;
            dataGridView.Columns[2].Width = 50;
            dataGridView.Columns[3].Width = 50;

            CustomizeDataGridView();

            //dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 订阅CellPainting事件以自定义状态单元格的绘制
            dataGridView.CellPainting += DataGridView_CellPainting;
        }

        private void CustomizeDataGridView()
        {
            // 设置列头的默认样式为居中对齐
            dataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //dataGridView.ColumnHeadersHeight = 50;

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

        public void AddStatus(PredictionRecord record)
        {
            // 添加新记录到列表
            records.Add(record);

            // 直接在DataGridView中添加新行
            dataGridView.Rows.Add(record.PredictionTime.ToString("dd/HH:mm:ss"), record.SN, record.FrontStatus, record.SideStatus);

            // 当记录数量超过300时，调整为只保留最新的100条记录
            if (records.Count > 300)
            {
                int removeCount = records.Count - 100;
                // 从记录列表中移除旧记录
                records.RemoveRange(0, removeCount);

                // 同时从DataGridView中移除对应的旧行
                for (int i = 0; i < removeCount; i++)
                {
                    dataGridView.Rows.RemoveAt(0);
                }
            }

            // 自动滚动到最新的行
            if (dataGridView.Rows.Count > 0)
            {
                dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.Rows.Count - 1;
            }
        }

        private void DataGridView_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 如果是最后一行，则绘制高亮边框
            if (e.RowIndex == dataGridView.Rows.Count - 1 && e.ColumnIndex >= 0)
            {
                // 绘制默认内容
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Border);

                // 高亮边框颜色，例如红色
                Color highlightBorderColor = Color.Lime;
                Pen highlightPen = new Pen(highlightBorderColor, 1); // 可以调整边框粗细

                // 绘制高亮边框
                e.Graphics.DrawRectangle(highlightPen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width - 2, e.CellBounds.Height - 2);

                // 阻止默认的边框绘制
                e.Handled = true;
            }

            if (e.RowIndex >= 0 && (e.ColumnIndex == 2 || e.ColumnIndex == 3))
            {
                // 通过平滑模式使圆形边缘更平滑
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                // 如果是最后一行，则绘制高亮边框
                if (e.RowIndex == dataGridView.Rows.Count - 1 && e.ColumnIndex >= 0)
                {
                    // 高亮边框颜色，例如红色
                    Color highlightBorderColor = Color.Lime;
                    Pen highlightPen = new Pen(highlightBorderColor, 1); // 可以调整边框粗细

                    // 绘制高亮边框
                    e.Graphics.DrawRectangle(highlightPen, e.CellBounds.Left, e.CellBounds.Top, e.CellBounds.Width - 2, e.CellBounds.Height - 2);
                }

                if (e.Value != null)
                {
                    PredictionStatus status = (PredictionStatus)e.Value;

                    Color color = status == PredictionStatus.OK ? Color.Green : status == PredictionStatus.NG ? Color.Red : Color.Gray;

                    // 动态计算圆的直径为单元格高度和宽度的较小者减去一定的边距，以保证圆形在单元格中居中且不触碰边界
                    int margin = 10; // 边距
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

        public void UpdateFrontStatus(string sn, PredictionStatus newStatus)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].SN == sn)
                {
                    // 更新记录列表中的状态
                    records[i] = new PredictionRecord(records[i].SN, newStatus, records[i].SideStatus, records[i].PredictionTime);

                    // 直接在DataGridView中更新对应的行
                    UpdateDataGridViewRow(i, records[i]);
                    break;
                }
            }
        }
        private void UpdateDataGridViewRow(int rowIndex, PredictionRecord record)
        {
            if (dataGridView.Rows.Count > rowIndex)
            {
                dataGridView.Rows[rowIndex].Cells[0].Value = record.PredictionTime.ToString("dd/HH:mm:ss");
                dataGridView.Rows[rowIndex].Cells[1].Value = record.SN;
                dataGridView.Rows[rowIndex].Cells[2].Value = record.FrontStatus;
                dataGridView.Rows[rowIndex].Cells[3].Value = record.SideStatus;
                // Optional: Force the cell to repaint if custom drawing is used
                dataGridView.InvalidateRow(rowIndex);
            }
        }

        public void UpdateSideStatus(string sn, PredictionStatus newStatus)
        {
            for (int i = 0; i < records.Count; i++)
            {
                if (records[i].SN == sn)
                {
                    // 更新记录列表中的状态
                    records[i] = new PredictionRecord(records[i].SN, records[i].FrontStatus, newStatus, records[i].PredictionTime);

                    // 直接在DataGridView中更新对应的行
                    UpdateDataGridViewRow(i, records[i]);
                    break;
                }
            }
        }

    }
}
