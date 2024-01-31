using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace CxWinFormsComponents.DataGridViewColumns
{
    [ToolboxItem(false)]
    public class CalendarEditingControl : DateTimePicker, IDataGridViewEditingControl
    {
        public CalendarEditingControl()
            : base()
        { }

        #region IDataGridViewEditingControl Members

        public object EditingControlFormattedValue
        {
            get { return Value.ToLongDateString(); }
            set
            {
                var newValue = value as String;
                if (newValue != null)
                {
                    Value = DateTime.Parse(newValue);
                }
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            CalendarForeColor = dataGridViewCellStyle.ForeColor;
            CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        public int EditingControlRowIndex { get; set; }

        public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey)
        {
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return false;
            }
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public DataGridView EditingControlDataGridView { get; set; }

        public bool EditingControlValueChanged { get; set; }

        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        #endregion

        protected override void OnValueChanged(EventArgs eventargs)
        {
            this.EditingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }

    }
}
