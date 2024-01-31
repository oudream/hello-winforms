using System;
using System.Windows.Forms;

namespace CxWinFormsComponents.DataGridViewColumns
{
    /// <summary>
    /// 日期单元格
    /// </summary>
    public class DataGridViewCalendarCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridViewCalendarCell()
            : base()
        {
            this.Style.Format = "d";
        }

        /// <summary>
        /// 初始化编辑控件
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="initialFormattedValue"></param>
        /// <param name="dataGridViewCellStyle"></param>
        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            var control = DataGridView.EditingControl as CalendarEditingControl;
            if (control != null)
            {
                if (Value != null && Value != DBNull.Value)
                    control.Text = Value.ToString();
                DataGridViewCalendarColumn column = this.OwningColumn as DataGridViewCalendarColumn;
                control.MaxDate = column.MaxDate;
                control.MinDate = column.MinDate;
                control.Format = DateTimePickerFormat.Custom;
                control.CustomFormat = column.CustomFormat;
            }

        }

        /// <summary>
        /// 编辑控件的类型
        /// </summary>
        public override Type EditType
        {
            get { return typeof(CalendarEditingControl); }
        }

        /// <summary>
        /// 值类型
        /// </summary>
        public override Type ValueType
        {
            get { return typeof(DateTime); }
        }

        /// <summary>
        /// 默认新值
        /// </summary>
        public override object DefaultNewRowValue
        {
            get { return DateTime.Now; }
        }


    }
}
