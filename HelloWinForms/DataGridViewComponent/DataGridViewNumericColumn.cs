using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

namespace CxWinFormsComponents.DataGridViewColumns
{

    /// <summary>
    /// 数值列
    /// </summary>
    [ToolboxItem(false)]
    public class DataGridViewNumericColumn : DataGridViewColumn
    {
        #region Fields and properties

        private bool showLine = false;
        /// <summary>
        /// 是否显示账本线条
        /// </summary>
        [DefaultValue(false)]
        [Description("指示是否显示账本线条。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ShowLine
        {
            get { return this.showLine; }
            set
            {
                if (this.showLine != value)
                {
                    this.showLine = value;
                }
            }
        }

        private int lineSpace = 12;
        /// <summary>
        /// 线间隔
        /// </summary>
        [DefaultValue(12)]
        [Description("线间隔。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int LineSpace
        {
            get { return lineSpace; }
            set
            {
                if (value <= 0 || value >= this.Width)
                    throw new ArgumentOutOfRangeException("线间隔必须大于零且小于列宽度。");
                else
                {
                    if (value != this.lineSpace)
                    {
                        lineSpace = value;
                    }
                }
            }
        }

        private Color decimalPlaceColor = Color.Red;
        /// <summary>
        /// 小数位分隔线颜色
        /// </summary>
        [DefaultValue(typeof(Color), "Red")]
        [Description("小数位分隔线颜色。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color DecimalPlaceColor
        {
            get { return decimalPlaceColor; }
            set
            {
                if (decimalPlaceColor != value)
                    decimalPlaceColor = value;
            }
        }

        private Color thousandsSeparatorColor = Color.DarkBlue;
        /// <summary>
        /// 千位分隔线颜色
        /// </summary>
        [DefaultValue(typeof(Color), "DarkBlue")]
        [Description("千位分隔线颜色。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color ThousandsSeparatorColor
        {
            get { return thousandsSeparatorColor; }
            set
            {
                if (thousandsSeparatorColor != value)
                {
                    thousandsSeparatorColor = value;
                }
            }
        }

        private Color normalColor = Color.LightBlue;
        /// <summary>
        /// 普通分隔线颜色
        /// </summary>
        [DefaultValue(typeof(Color), "LightBlue")]
        [Description("普通分隔线颜色。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public Color NormalColor
        {
            get { return normalColor; }
            set
            {
                if (normalColor != value)
                    normalColor = value;
            }
        }

        private int scale = 2;
        /// <summary>
        /// 小数位数
        /// </summary>
        [DefaultValue(2)]
        [Description("小数位数。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Scale
        {
            get { return scale; }
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException("小数位数不允许小于零。");
                else
                    scale = value;
            }
        }

        private bool negativeShowRed = true;
        /// <summary>
        /// 负数显示红字
        /// </summary>
        [DefaultValue(true)]
        [Description("指示负数是否显示红字。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool NegativeShowRed
        {
            get { return negativeShowRed; }
            set
            {
                if (negativeShowRed != value)
                    negativeShowRed = value;
            }
        }

        private char moneySymbol = '￥';
        /// <summary>
        /// 货币符号
        /// </summary>
        [DefaultValue('￥')]
        [Description("货币符号。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public char MoneySymbol
        {
            get { return moneySymbol; }
            set { moneySymbol = value; }
        }

        private bool showMoneySymbol = true;
        /// <summary>
        /// 是否显示货币符号，仅当ShowLine属性为true时有效。
        /// </summary>
        [DefaultValue(true)]
        [Description("是否显示货币符号。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public bool ShowMoneySymbol
        {
            get { return showMoneySymbol; }
            set
            {
                if (showMoneySymbol != value)
                    showMoneySymbol = value;
            }
        }

        private decimal minimum = -99999999999M;
        /// <summary>
        /// 最小值
        /// </summary>
        [DefaultValue(typeof(decimal), "-99999999999")]
        [Description("最小值。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public decimal Minimum
        {
            get { return minimum; }
            set
            {
                if (value >= maximum)
                    throw new ArgumentOutOfRangeException("最小值必须小于最大值。");
                else
                    minimum = value;
            }
        }

        private decimal maximum = 99999999999M;
        /// <summary>
        /// 最大值
        /// </summary>
        [DefaultValue(typeof(decimal), "99999999999")]
        [Description("最大值。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public decimal Maximum
        {
            get { return maximum; }
            set
            {
                if (value <= minimum)
                    throw new ArgumentOutOfRangeException("最大值必须大于最小值。");
                else
                    maximum = value;
            }
        }

        private HorizontalAlignment editTextAlign = HorizontalAlignment.Left;
        /// <summary>
        /// 指示编辑时文本在编辑框中的对齐方式
        /// </summary>
        [DefaultValue(HorizontalAlignment.Left)]
        [Description("指示编辑时文本在编辑框中的对齐方式。")]
        [RefreshProperties(RefreshProperties.Repaint)]
        public HorizontalAlignment EditTextAlign
        {
            get { return editTextAlign; }
            set { editTextAlign = value; }
        }

        /// <summary>
        /// 单元格模板
        /// </summary>
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if (value == null || (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewNumericCell))))
                    throw new ArgumentException("单元格模板类型不是FinanceCell或其子类。");
                else
                    base.CellTemplate = value;
            }
        }

        #endregion Fields and properties

        /// <summary>
        /// 数值列
        /// </summary>
        public DataGridViewNumericColumn()
            : base()
        {
            this.CellTemplate = new DataGridViewNumericCell();
        }

        /// <summary>
        /// 重写克隆方法
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            DataGridViewNumericColumn column = (DataGridViewNumericColumn)base.Clone();
            column.CellTemplate = new DataGridViewNumericCell();
            column.DecimalPlaceColor = this.DecimalPlaceColor;
            column.LineSpace = this.LineSpace;
            column.MoneySymbol = this.MoneySymbol;
            column.NegativeShowRed = this.NegativeShowRed;
            column.NormalColor = this.NormalColor;
            column.Scale = this.Scale;
            column.ShowMoneySymbol = this.ShowMoneySymbol;
            column.ShowLine = this.ShowLine;
            column.ThousandsSeparatorColor = this.ThousandsSeparatorColor;
            column.EditTextAlign = this.EditTextAlign;
            return column;
        }

    }

}
