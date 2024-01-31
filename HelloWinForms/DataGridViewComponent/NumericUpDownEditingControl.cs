using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CxWinFormsComponents.DataGridViewColumns
{
    internal class NumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        #region Implementation of IDataGridViewEditingControl

        private bool editingControlValueChanged;

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            ForeColor = dataGridViewCellStyle.ForeColor;
            BackColor = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                case Keys.Decimal:
                    return true;
                default:
                    return false;
            }
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        { }

        public DataGridView EditingControlDataGridView
        { get; set; }

        public object EditingControlFormattedValue
        {
            get { return Value.ToString(); }
            set { Value = decimal.Parse(value.ToString()); }
        }

        public int EditingControlRowIndex { get; set; }

        public bool EditingControlValueChanged
        {
            get { return editingControlValueChanged; }
            set { editingControlValueChanged = value; }
        }

        public Cursor EditingPanelCursor { get { return base.Cursor; } }

        public bool RepositionEditingControlOnValueChange { get { return false; } }

        #endregion Implementation of IDataGridViewEditingControl

        protected override void OnValueChanged(EventArgs eventargs)
        {
            editingControlValueChanged = true;
            EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(eventargs);
        }

    }

}
