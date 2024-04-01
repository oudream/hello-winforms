using CxWorkStation.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.Utilities
{
    public class LogViewer
    {
        // Log基础控件
        private DataGridView dataGridViewLogs;
        private Button clearButton;
        private CheckBox levelWithinCheckBox;
        private ComboBox filterComboBox;

        // Tag
        private CheckBox tagCheckBox;
        private ComboBox tagComboBox;
        private Button addTagButton;
        private Button clearTagButton;
        private List<string> allTags = new List<string>();
        private List<string> selectTags = new List<string>();
        private Label tagLabel;

        private List<LogItem> logItems = new List<LogItem>();

        public LogViewer(DataGridView dataGridViewLogs, Button clearButton, CheckBox levelWithinCheckBox, ComboBox filterComboBox,
            CheckBox tagCheckBox, ComboBox tagComboBox, Button addTagButton, Button clearTagButton, Label tagLabel)
        {
            this.dataGridViewLogs = dataGridViewLogs;
            this.clearButton = clearButton;
            this.levelWithinCheckBox = levelWithinCheckBox;
            this.filterComboBox = filterComboBox;

            this.tagCheckBox = tagCheckBox;
            this.tagComboBox = tagComboBox;
            this.addTagButton = addTagButton;
            this.clearTagButton = clearTagButton;
            this.tagLabel = tagLabel;

            InitializeComponents();

            LogHelper.LogEvent += HandleLogEvent;
        }

        ~LogViewer()
        {
            LogHelper.LogEvent -= HandleLogEvent;
        }

        private void InitializeComponents()
        {
            dataGridViewLogs.AutoGenerateColumns = false; // Disable auto column generation
            dataGridViewLogs.AllowUserToAddRows = false; // Prevent user from adding rows
            dataGridViewLogs.RowHeadersVisible = false; // Hide the row headers

            // Add columns for log details
            dataGridViewLogs.Columns.Add("Timestamp", "Timestamp");
            dataGridViewLogs.Columns.Add("LogLevel", "Level");
            dataGridViewLogs.Columns.Add("Message", "Message");

            // Set column types as needed, for example, for better formatting
            // dataGridViewLogs.Columns["Timestamp"].ValueType = typeof(DateTime);

            // Adjust column widths as needed
            dataGridViewLogs.Columns["Timestamp"].Width = 120;
            dataGridViewLogs.Columns["LogLevel"].Width = 70;
            dataGridViewLogs.Columns["Message"].Width = dataGridViewLogs.Parent.Width - 120 - 70 - 5;

            CustomizeDataGridView();

            // Initialize ComboBox
            filterComboBox.DataSource = new BindingSource(LogHelper.LogLevelTranslations, null);
            filterComboBox.DisplayMember = "Key";
            filterComboBox.ValueMember = "Value";
            filterComboBox.SelectedIndex = 0;

            // Clear logs when the button is clicked
            clearButton.Click += ClearButton_Click;

            // Update logs based on selection changes
            filterComboBox.SelectedIndexChanged += FilterComboBox_SelectedIndexChanged;
            levelWithinCheckBox.CheckedChanged += LevelWithinCheckBox_CheckedChanged;

            // Tag
            tagCheckBox.CheckedChanged += TagCheckBox_CheckedChanged;
            addTagButton.Click += AddTagButton_Click;
            clearTagButton.Click += ClearTagButton_Click;
        }

        private void CustomizeDataGridView()
        {
            // 设置列头的默认样式为居中对齐
            dataGridViewLogs.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // 适用于已经添加的列，确保它们的列头也是居中对齐
            foreach (DataGridViewColumn column in dataGridViewLogs.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            // 设置 DataGridView 的背景颜色
            dataGridViewLogs.BackgroundColor = Color.FromArgb(38, 38, 38);

            // 设置边框颜色和样式
            dataGridViewLogs.GridColor = Color.White;
            dataGridViewLogs.BorderStyle = BorderStyle.FixedSingle;

            // 禁用行头和列头的视觉样式，以便自定义颜色
            dataGridViewLogs.EnableHeadersVisualStyles = false;

            // 设置列头的样式
            dataGridViewLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
            dataGridViewLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewLogs.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 设置行头的样式（如果你使用行头）
            dataGridViewLogs.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
            dataGridViewLogs.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridViewLogs.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // 设置单元格的默认样式
            dataGridViewLogs.DefaultCellStyle.BackColor = Color.FromArgb(38, 38, 38);
            dataGridViewLogs.DefaultCellStyle.ForeColor = Color.White;
            dataGridViewLogs.DefaultCellStyle.SelectionBackColor = Color.FromArgb(58, 58, 58); // 设置选中单元格的背景颜色
            dataGridViewLogs.DefaultCellStyle.SelectionForeColor = Color.White;

            // 设置网格线的颜色
            dataGridViewLogs.GridColor = Color.White;

            // 其他美化设置...
        }

        private void ClearTagButton_Click(object sender, EventArgs e)
        {
            selectTags.Clear();
            tagLabel.Text = string.Empty;
            FilterLogs();
        }

        private void AddTagButton_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(tagComboBox.Text))
            {
                var tag = tagComboBox.Text;
                if (!selectTags.Contains(tag))
                {
                    selectTags.Add(tag);
                    tagLabel.Text = string.Join(",", selectTags);
                    FilterLogs();
                }
            }
        }

        private void TagCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void HandleLogEvent(long timestamp, LogLevel logLevel, string logMessage, string logTag)
        {
            dataGridViewLogs.BeginInvoke(new System.Action(() =>
            {
                Color logColor = GetLogColor(logLevel);
                List<string> logTags = null;
                // 判断logTag是否为空
                if (!string.IsNullOrEmpty(logTag))
                {
                    // 拆分标签（逗号分隔）
                    logTags = logTag.Split(',').ToList();
                    // 更新allTags列表
                    // 如果allTags被更新，需要对tagComboBox进行更新
                    var updated = false;
                    foreach (var tag in logTags)
                    {
                        if (!allTags.Contains(tag))
                        {
                            allTags.Add(tag);
                            updated = true;
                        }
                    }
                    if (updated)
                    {
                        tagComboBox.DataSource = null; // 清除现有数据源
                        tagComboBox.DataSource = allTags; // 重新设置数据源为更新后的标签列表
                        tagComboBox.SelectedIndex = allTags.Count - 1;
                    }
                }
                LogItem logItem = new LogItem { Text = logMessage, Color = logColor, Level = logLevel, Timestamp = timestamp, Tags = logTags };
                logItems.Add(logItem);
                // Filter display based on ComboBox value
                if (Enum.TryParse(filterComboBox.SelectedValue.ToString(), out LogLevel selectedLogLevel))
                {
                    if (logItems.Count > 1000)
                    {
                        // 计算要移除的项目数量
                        int itemsToRemove = logItems.Count - 100;
                        // 从列表开头开始移除指定数量的元素
                        logItems.RemoveRange(0, itemsToRemove);
                        FilterLogs();
                    }
                    else
                    {
                        if ((levelWithinCheckBox.Checked && logLevel >= selectedLogLevel) || (!levelWithinCheckBox.Checked && logLevel == selectedLogLevel))
                        {
                            // 如果不检查标签，或者包含所选标签，则显示
                            if (ContainsTags(logItem))
                            {
                                AppendText(logItem);
                            }
                        }
                    }
                }
            }));
        }

        private Color GetLogColor(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Verbose: return Color.LightGreen;
                case LogLevel.Debug: return Color.Gray;
                case LogLevel.Information: return Color.Green;
                case LogLevel.Warning: return Color.Orange;
                case LogLevel.Error: return Color.Red;
                case LogLevel.Fatal: return Color.DarkRed;
                default: return Color.Green;
            }
        }

        private void AppendText(LogItem logItem)
        {
            // Create a new row and set its values
            int rowIndex = dataGridViewLogs.Rows.Add();
            var row = dataGridViewLogs.Rows[rowIndex];
            row.Cells["Timestamp"].Value = TimeHelper.GetDateTimeString(logItem.Timestamp, "dd/HH:mm:ss.fff");
            row.Cells["LogLevel"].Value = LogHelper.GetLogLevelTranslation(logItem.Level);
            row.Cells["Message"].Value = logItem.Text;

            // 将 LogItem 对象存储在行的 Tag 属性中
            row.Tag = logItem;

            // Optional: Set row or cell color based on log level
            row.DefaultCellStyle.ForeColor = logItem.Color;

            // Auto-scroll to the newest log entry
            if (dataGridViewLogs.Rows.Count > 1)
            {
                dataGridViewLogs.FirstDisplayedScrollingRowIndex = dataGridViewLogs.Rows.Count - 1;
            }

            //logRichTextBox.SelectionStart = logRichTextBox.TextLength;
            //logRichTextBox.SelectionLength = 0;

            //logRichTextBox.SelectionColor = color;
            //logRichTextBox.AppendText(text + Environment.NewLine);
            //logRichTextBox.SelectionColor = logRichTextBox.ForeColor;

            //// Scroll to the last line
            //logRichTextBox.ScrollToCaret();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            // 清除 DataGridView 的所有行
            dataGridViewLogs.Rows.Clear();

            // 如果你还维护了与 DataGridView 相关的其他数据结构（比如日志项的列表），也应该清空
            logItems.Clear();
        }

        private void FilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void LevelWithinCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            FilterLogs();
        }

        private void FilterLogs()
        {
            if (dataGridViewLogs.InvokeRequired)
            {
                dataGridViewLogs.Invoke(new System.Action(FilterLogs));
                return;
            }

            if (Enum.TryParse(filterComboBox.SelectedValue?.ToString(), out LogLevel selectedLogLevel))
            {
                foreach (DataGridViewRow row in dataGridViewLogs.Rows)
                {
                    // 从行的 Tag 属性获取 LogItem 对象
                    if (row.Tag is LogItem logItem)
                    {
                        // 现在可以直接使用 logItem 进行过滤判断
                        bool levelMatch = levelWithinCheckBox.Checked ? logItem.Level >= selectedLogLevel : logItem.Level == selectedLogLevel;
                        bool visible = levelMatch && ContainsTags(logItem);

                        row.Visible = levelMatch && ContainsTags(logItem);
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
            }
        }

        private bool ContainsTags(LogItem logItem)
        {
            // 如果没有指定任何标签，返回true
            if (selectTags.Count == 0)
            {
                return true;
            }
            // 如果指定了标签，但此项LogItem没有标签，返回false
            if (logItem.Tags == null || logItem.Tags.Count == 0)
            {
                return false;
            }
            // 如果指定了标签，检查它们是否都在LogItem的标签列表中
            if (tagCheckBox.Checked)
            {
                return ContainsAnyTags(logItem);
            }
            else
            {
                return ContainsAllTags(logItem);
            }
            //bool tagMatch = selectTags.Count == 0 ||
            //    (tagCheckBox.Checked && selectTags.Any(tag => logItem.Tags.Contains(tag))) ||
            //    (!tagCheckBox.Checked && selectTags.All(tag => logItem.Tags.Contains(tag)));
        }

        private bool ContainsAllTags(LogItem logItem)
        {
            // 对于指定的每一个标签，检查它是否存在于LogItem的标签列表中
            foreach (var tag in selectTags)
            {
                bool found = false; // 假设没有找到
                foreach (var logItemTag in logItem.Tags)
                {
                    if (tag == logItemTag)
                    //if (string.Equals(tag, logItemTag, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true; // 找到了指定的标签
                        break; // 跳出内层循环
                    }
                }
                if (!found)
                {
                    // 如果有任何一个指定的标签没有在LogItem的标签列表中找到，返回false
                    return false;
                }
            }

            // 如果所有指定的标签都在LogItem的标签列表中找到了，返回true
            return true;
        }

        private bool ContainsAnyTags(LogItem logItem)
        {
            // 对于指定的每一个标签，检查它是否存在于LogItem的标签列表中
            foreach (var tag in selectTags)
            {
                foreach (var logItemTag in logItem.Tags)
                {
                    if (tag == logItemTag)
                    //if (string.Equals(tag, logItemTag, StringComparison.OrdinalIgnoreCase))
                    {
                        return true; // 找到了指定的标签
                    }
                }
            }

            // 如果所有指定的标签都在LogItem的标签列表中找不到，返回false
            return false;
        }
    }

}
