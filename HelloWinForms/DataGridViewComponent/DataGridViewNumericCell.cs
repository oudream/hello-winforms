using System;
using System.Drawing;
using System.Windows.Forms;


namespace CxWinFormsComponents.DataGridViewColumns
{
    /// <summary>
    /// 数值单元格
    /// </summary>
    internal class DataGridViewNumericCell : DataGridViewTextBoxCell
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public DataGridViewNumericCell()
            : base()
        {
        }

        #region 自定义绘制

        protected override void Paint(Graphics graphics, Rectangle clipBounds,
            Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState,
            object value, object formattedValue, string errorText,
            DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            DataGridViewNumericColumn column = this.OwningColumn as DataGridViewNumericColumn;
            if (column != null
                && column.ShowLine)
            {
                this.PaintPrivate(graphics, clipBounds, cellBounds, rowIndex,
                    cellState, formattedValue, errorText, cellStyle,
                    advancedBorderStyle, paintParts, false, false, true);
            }
            else
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, 
                    formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }

        private bool ShouldPaintBorder(DataGridViewPaintParts paintParts)
        {
            return ((paintParts & DataGridViewPaintParts.Border) != DataGridViewPaintParts.None);
        }

        private bool ShouldPaintSelectionBackground(DataGridViewPaintParts paintParts)
        {
            return ((paintParts & DataGridViewPaintParts.SelectionBackground) != DataGridViewPaintParts.None);
        }

        private bool ShouldPaintBackground(DataGridViewPaintParts paintParts)
        {
            return ((paintParts & DataGridViewPaintParts.Background) != DataGridViewPaintParts.None);
        }

        private bool ShouldPaintFocus(DataGridViewPaintParts paintParts)
        {
            return ((paintParts & DataGridViewPaintParts.Focus) != DataGridViewPaintParts.None);
        }

        private bool ShouldPaintSelected(DataGridViewElementStates cellState)
        {
            return (cellState & DataGridViewElementStates.Selected) != DataGridViewElementStates.None;
        }

        private bool ShouldPaintContentForeground(DataGridViewPaintParts paintParts)
        {
            return ((paintParts & DataGridViewPaintParts.ContentForeground) != DataGridViewPaintParts.None);
        }

        protected void PaintPrivate(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, 
            DataGridViewElementStates cellState, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, 
            DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts, 
            bool computeContentBounds, bool computeErrorIconBounds, bool paint)
        {
            DataGridViewNumericColumn column = this.OwningColumn as DataGridViewNumericColumn;
            CheckPaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle, paintParts, paint);
            cellBounds = CalcRealCellBounds(cellBounds, advancedBorderStyle);
            bool isCell = (DataGridView.CurrentCellAddress.X == base.ColumnIndex) && (DataGridView.CurrentCellAddress.Y == rowIndex) && (DataGridView.EditingControl != null);
            CheckPaintBackground(graphics, cellBounds, rowIndex, cellState, cellStyle, paintParts, paint, isCell);//检查绘制背景
            DrawLines(graphics, cellBounds);//绘制线条
            DrawString(graphics, cellBounds, formattedValue, cellStyle, paintParts, computeContentBounds, paint, isCell);//绘制文本
        }

        private void CheckPaintBorder(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts, bool paint)
        {
            if (paint && ShouldPaintBorder(paintParts))//绘制边框
            {
                this.PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }
        }

        private void CheckPaintBackground(Graphics graphics, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, DataGridViewCellStyle cellStyle, DataGridViewPaintParts paintParts, bool paint, bool isCell)
        {
            SolidBrush solidBrush;
            bool isCellSelected = (cellState & DataGridViewElementStates.Selected) != DataGridViewElementStates.None;

            if ((ShouldPaintSelectionBackground(paintParts) && isCellSelected) && !isCell)//确定绘制背景的画刷
                solidBrush = new SolidBrush(cellStyle.SelectionBackColor);
            else
                solidBrush = new SolidBrush(cellStyle.BackColor);

            //绘制背景
            if (paint && ShouldPaintBackground(paintParts) && cellBounds.Width > 0 && cellBounds.Height > 0)
            {
                graphics.FillRectangle(solidBrush, cellBounds);
            }
        }

        //绘制文本
        private void DrawString(Graphics graphics, Rectangle cellBounds, object formattedValue, DataGridViewCellStyle cellStyle, DataGridViewPaintParts paintParts, bool computeContentBounds, bool paint, bool isCell)
        {
            int scale = 2;
            string moneySymbol = "￥";
            bool showMoneySymbol = true;
            bool negativeShowRed = true;
            int lineSpace = 12;

            DataGridViewNumericColumn column = this.OwningColumn as DataGridViewNumericColumn;
            
            if (column != null)
            {
                scale = column.Scale;
                moneySymbol = column.MoneySymbol.ToString();
                showMoneySymbol = column.ShowMoneySymbol;
                negativeShowRed = column.NegativeShowRed;
                lineSpace = column.LineSpace;
            }

            string formattedValueString = formattedValue as string;
            if (!String.IsNullOrEmpty(formattedValueString) && ((paint && !isCell) || computeContentBounds) && ShouldPaintContentForeground(paintParts))
            {
                decimal d = 0;
                Decimal.TryParse(formattedValueString, out d);
                bool isNegative = (d < 0M);
                if (negativeShowRed && isNegative)
                {
                    d = d * -1M;
                }

                string format = new string('0', scale);
                if (scale > 0)
                {
                    format = "#0." + format;
                }
                else
                {
                    format = "#0";
                }

                formattedValueString = d.ToString(format);
                formattedValueString = showMoneySymbol ? moneySymbol + formattedValueString : formattedValueString;

                int left = cellBounds.Width;
                int digitIndex = formattedValueString.Length - 1;
                while (left > 0)
                {
                    if (digitIndex == -1)
                    {
                        break;
                    }

                    if (left - lineSpace > 0)
                    {
                        left -= lineSpace;
                        if (formattedValueString[digitIndex].ToString() == ".")
                        {
                            digitIndex--;
                        }

                        Color foreColor = this.Selected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                        foreColor = isNegative && negativeShowRed ? Color.Red : foreColor;

                        using (SolidBrush brush = new SolidBrush(foreColor))
                        {
                            string myChar = formattedValueString[digitIndex].ToString();
                            SizeF myCharSize = graphics.MeasureString(myChar, cellStyle.Font);
                            int charLeft = cellBounds.Left + left + (int)(lineSpace - (int)myCharSize.Width) / 2;
                            int charTop = cellBounds.Top + (int)(cellBounds.Height - (int)myCharSize.Height) / 2;

                            graphics.DrawString(myChar, cellStyle.Font, brush, charLeft, charTop);
                        }
                    }
                    else
                    {
                        left = 0;
                    }
                    digitIndex--;
                }
            }
        }

        /// <summary>
        /// 计算实际单元格区间
        /// </summary>
        /// <param name="cellBounds">单元格区间</param>
        /// <param name="advancedBorderStyle">边框风格</param>
        /// <returns>实际单元格区间</returns>
        private Rectangle CalcRealCellBounds(Rectangle cellBounds, DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            Rectangle advanceRectangle = this.BorderWidths(advancedBorderStyle);
            cellBounds.Offset(advanceRectangle.X, advanceRectangle.Y);
            cellBounds.Width -= advanceRectangle.Right;
            cellBounds.Height -= advanceRectangle.Bottom;
            return cellBounds;
        }

        //绘制线条
        private void DrawLines(Graphics graphics, Rectangle cellBounds)
        {
            int left = cellBounds.Width;
            int digitIndex = 1;
            int lineSpace = 12;
            Color DecimalPlaceColor = Color.Red;
            Color ThousandsSeparatorColor = Color.DarkBlue;
            Color NormalColor = Color.LightBlue;

            DataGridViewNumericColumn column = this.OwningColumn as DataGridViewNumericColumn;
            int scale = 2;
            if (column != null)
            {
                scale = column.Scale;
                lineSpace = column.LineSpace;
            }
            Point PointStart, PointEnd;

            while (left > 0)
            {
                if (left - lineSpace > 0)
                {
                    left -= lineSpace;
                    PointStart = new Point(cellBounds.Left + left, cellBounds.Top);
                    PointEnd = new Point(cellBounds.Left + left, cellBounds.Top + cellBounds.Height);

                    if (digitIndex == scale)
                    {
                        using (Pen redPen = new Pen(DecimalPlaceColor, 1.0F))//绘制小数线
                            graphics.DrawLine(redPen, PointStart, PointEnd);
                    }
                    else
                    {
                        if (digitIndex > scale && (digitIndex - scale) % 3 == 0)//绘制千分位线
                        {
                            using (Pen specialPen = new Pen(ThousandsSeparatorColor, 2.0F))
                                graphics.DrawLine(specialPen, PointStart, PointEnd);
                        }
                        else
                        {
                            using (Pen normalPen = new Pen(NormalColor, 1.0F))//绘制普通线
                                graphics.DrawLine(normalPen, PointStart, PointEnd);
                        }
                    }
                }
                else
                {
                    left = 0;
                }
                digitIndex++;
            }
        }

        #endregion 自定义绘制

        /// <summary>
        /// 编辑类型
        /// </summary>
        public override Type EditType
        {
            get { return typeof(NumericUpDownEditingControl); }
        }

        /// <summary>
        /// 值类型
        /// </summary>
        public override Type ValueType
        {
            get { return typeof(decimal); }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        public override object DefaultNewRowValue
        {
            get { return 0M; }
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
            NumericUpDownEditingControl num = this.DataGridView.EditingControl as NumericUpDownEditingControl;
            if (num != null)
            {
                DataGridViewNumericColumn column = this.OwningColumn as DataGridViewNumericColumn;
                num.DecimalPlaces = column.Scale;
                num.ThousandsSeparator = true;
                num.Minimum = column.Minimum;
                num.Maximum = column.Maximum;
                if(this.Value != null && this.Value != DBNull.Value)
                    num.Value = Convert.ToDecimal(this.Value);
            }
        }

    }//class FinanceTextBoxCell

}//namespace
