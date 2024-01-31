using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace CxWinFormsComponents.DataGridViewColumns
{
    /// <summary>
    /// 日期列
    /// </summary>
    [ToolboxItem(false)]
    public class DataGridViewCalendarColumn : DataGridViewColumn
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridViewCalendarColumn()
            : base(new DataGridViewCalendarCell())
        {
            this.CellTemplate = new DataGridViewCalendarCell();
        }

        private string m_DateFormat = "yyyy/mm/dd";
        /// <summary>
        /// 日期格式字符串
        /// </summary>
        [DefaultValue("yyyy/mm/dd")]
        [Description("获取或设置自定义日期/时间格式字符串。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public string CustomFormat
        {
            get { return m_DateFormat; }
            set
            {
                if (m_DateFormat != value || this.CellTemplate.Style.Format != value)
                {
                    m_DateFormat = value;
                    this.CellTemplate.Style.Format = this.m_DateFormat;
                }
            }
        }

        private DateTime maxDate = new DateTime(9998, 12, 31, 0, 0, 0);
        /// <summary>
        /// 获取或设置可在控件中选择的最大日期和时间。
        /// </summary>
        [DefaultValue(typeof(DateTime), "12/31/9998 00:00:00")]
        [Description("获取或设置可在控件中选择的最大日期和时间。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public DateTime MaxDate
        {
            get
            {
                return this.maxDate;
            }
            set
            {
                if (this.maxDate != value && value <= new DateTime(9998, 12, 31, 0, 0, 0))
                {
                    this.maxDate = value;
                }
            }
        }

        private DateTime minDate = new DateTime(1753, 1, 1, 0, 0, 0);
        /// <summary>
        /// 获取或设置可在控件中选择的最小日期和时间。
        /// </summary>
        [DefaultValue(typeof(DateTime), "1/1/1753 00:00:00")]
        [Description("获取或设置可在控件中选择的最小日期和时间。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public DateTime MinDate
        {
            get
            {
                return this.minDate;
            }
            set
            {
                if (this.minDate != value && value >= new DateTime(1753, 1, 1, 0, 0, 0))
                {
                    this.minDate = value;
                }
            }
        }

        /// <summary>
        /// 单元格模板
        /// </summary>
        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewCalendarCell)))
                {
                    throw new InvalidCastException("单元格模板类型不是CalendarCell或其子类。");
                }
                base.CellTemplate = value;
            }
        }

        /// <summary>
        /// 克隆方法
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            DataGridViewCalendarColumn column = base.Clone() as DataGridViewCalendarColumn;
            column.CellTemplate = new DataGridViewCalendarCell();
            column.MaxDate = this.MaxDate;
            column.MinDate = this.MinDate;
            column.CustomFormat = this.CustomFormat;

            return column;
        }
    }
}
