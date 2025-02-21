using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core;

namespace HelloWinForms
{
    public partial class HelloConfigUI : Form
    {
        private ConfigUI configUI;
        private IniParser iniParser;

        public HelloConfigUI()
        {
            InitializeComponent();

            // 先根据默认描述构造配置（你也可以根据需要扩展配置项）
            configUI = ConfigUI.LoadSampleAIConfigUI();
            iniParser = new IniParser();
        }

        private void HelloConfigUI_Load(object sender, EventArgs e)
        {
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // 打开对话框选择配置文件
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "选择配置文件";
            dialog.Filter = "配置文件|*.ini";
            dialog.InitialDirectory = Environment.CurrentDirectory;
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string configFilePath = dialog.FileName;
            try
            {
                if (!File.Exists(configFilePath))
                {
                    MessageBox.Show("配置文件不存在: " + configFilePath);
                    return;
                }
                bool loaded = iniParser.LoadFromFile(configFilePath);
                if (!loaded)
                {
                    MessageBox.Show("加载配置文件失败！");
                    return;
                }
                // 先构造界面（使用默认值），再更新为配置文件中的值
                configUI.BuildUI(this.panel1, this.toolTip1, Color.Black);
                configUI.LoadFromIni(iniParser);
                MessageBox.Show("加载配置成功！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载配置异常: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 打开对话框选择配置文件
            var dialog = new SaveFileDialog();
            dialog.Title = "保存配置文件";
            dialog.Filter = "配置文件|*.ini";
            dialog.InitialDirectory = Environment.CurrentDirectory;
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string configFilePath = dialog.FileName;
            try
            {
                configUI.UpdateIniFromUI(iniParser);
                bool saved = iniParser.SaveToFile(configFilePath);
                if (saved)
                    MessageBox.Show("保存配置成功！");
                else
                    MessageBox.Show("保存配置失败！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存配置异常: " + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 初始化一个 List<double> 类型的列表
            List<double> ratiosLine1 = new List<double> { 1.2, 2.3, 3.4, 4.5, 5.6 };

            // 计算列表的均值
            double average = ratiosLine1.Average();

            // 输出均值
            Console.WriteLine("列表的均值是: " + average);
            try
            {
                configUI.RestoreDefaults();
                MessageBox.Show("已还原默认值！");
            }
            catch (Exception ex)
            {
                MessageBox.Show("还原默认值异常: " + ex.Message);
            }
        }

        private void toolTip1_Draw(object sender, DrawToolTipEventArgs e)
        {
            //e.Graphics.Clear(((Control)sender).BackColor);
            // 设置 ToolTip 的背景颜色和文本颜色
            e.DrawBackground();
            e.DrawBorder();
            using (StringFormat sf = new StringFormat())
            {
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;
                sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.None;
                sf.FormatFlags = StringFormatFlags.NoWrap;
                e.Graphics.DrawString(e.ToolTipText, this.Font, SystemBrushes.ActiveCaptionText, e.Bounds, sf);
            }
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {
            // 获取 ToolTip 要显示的文本
            string toolTipText = toolTip1.GetToolTip(e.AssociatedControl);

            // 测量文本所需的大小
            SizeF textSize = TextRenderer.MeasureText(toolTipText, this.Font);

            // 增加一些额外的边距
            int padding = 10;
            e.ToolTipSize = new Size((int)textSize.Width + padding * 2, (int)textSize.Height + padding * 2);
        }
    }


    /// </summary>
    public enum ConfigControlType
    {
        TextBox,
        DropDown,
        NumericInteger,
        NumericFloat,
        CheckBox
    }

    /// <summary>
    /// 配置项基类，包含公共属性：Key、Label、默认值、Tip等。
    /// </summary>
    public abstract class ConfigItem
    {
        /// <summary>
        /// 配置项标识（如ini中的Key）
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 标签显示（最多10个中文字符）
        /// </summary>
        public string LabelText { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public object DefaultValue { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string Tip { get; set; }
        /// <summary>
        /// 使用的控件类型
        /// </summary>
        public ConfigControlType ControlType { get; protected set; }
        /// <summary>
        /// 绑定的控件(用于
        /// </summary>
        public Control BoundControl { get; set; }
        /// <summary>
        /// 生成编辑控件（会设置Tip）
        /// </summary>
        public abstract Control CreateControl(Rectangle bounds, Font font, ToolTip toolTip);

        /// <summary>
        /// 生成左侧的标签控件
        /// </summary>
        public Label CreateLabel(Font font, int labelWidth, int itemHeight)
        {
            Label label = new Label();
            label.Text = LabelText;
            label.TextAlign = ContentAlignment.MiddleRight;
            label.AutoSize = false;
            label.Font = font;
            label.Width = labelWidth;
            label.Height = itemHeight;
            return label;
        }
    }

    /// <summary>
    /// 文本框配置项：可设置最大长度、是否允许为空、正则表达式
    /// </summary>
    public class TextBoxConfigItem : ConfigItem
    {
        public int MaxLength { get; set; }
        public bool AllowEmpty { get; set; }
        public string RegexPattern { get; set; }

        public TextBoxConfigItem()
        {
            ControlType = ConfigControlType.TextBox;
        }

        public override Control CreateControl(Rectangle bounds, Font font, ToolTip toolTip)
        {
            TextBox tb = new TextBox();
            tb.Font = font;
            tb.Width = bounds.Width;
            tb.Height = bounds.Height;
            tb.MaxLength = MaxLength;
            tb.Text = DefaultValue?.ToString() ?? "";
            // 此处可添加正则验证逻辑
            toolTip.SetToolTip(tb, Tip);
            this.BoundControl = tb;
            return tb;
        }
    }

    /// <summary>
    /// 下拉框配置项：包含显示项和值项及是否允许空值（允许空时采用可编辑下拉，否则用DropDownList）
    /// </summary>
    public class DropDownConfigItem : ConfigItem
    {
        public List<string> DisplayItems { get; set; } = new List<string>();
        public List<string> ValueItems { get; set; } = new List<string>();
        public bool AllowEmpty { get; set; }

        public DropDownConfigItem()
        {
            ControlType = ConfigControlType.DropDown;
        }

        public override Control CreateControl(Rectangle bounds, Font font, ToolTip toolTip)
        {
            ComboBox combo = new ComboBox();
            combo.Font = font;
            combo.Width = bounds.Width;
            combo.Height = bounds.Height;
            combo.DropDownStyle = AllowEmpty ? ComboBoxStyle.DropDown : ComboBoxStyle.DropDownList;
            
            // 创建一个包含显示文本和值的数据集合
            var data = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < Math.Min(DisplayItems.Count, ValueItems.Count); i++)
            {
                data.Add(new KeyValuePair<string, string>(ValueItems[i], DisplayItems[i]));
            }

            //if (AllowEmpty)
            //{
            //    data.Insert(0, new KeyValuePair<string, string>("", ""));
            //}

            // 设置 ComboBox 的数据源
            combo.DataSource = data;
            // 设置显示的文本字段
            combo.DisplayMember = "Value";
            // 设置对应的实际值字段
            combo.ValueMember = "Key";

            combo.SelectedValue = DefaultValue?.ToString();

            toolTip.SetToolTip(combo, Tip);
            this.BoundControl = combo;
            return combo;
        }
    }

    /// <summary>
    /// 整数数字框配置项（使用NumericUpDown，最大、最小、step均为double，但显示整数）
    /// </summary>
    public class NumericIntegerConfigItem : ConfigItem
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }

        public NumericIntegerConfigItem()
        {
            ControlType = ConfigControlType.NumericInteger;
        }

        public override Control CreateControl(Rectangle bounds, Font font, ToolTip toolTip)
        {
            NumericUpDown numeric = new NumericUpDown();
            numeric.Font = font;
            numeric.Width = bounds.Width;
            numeric.Height = bounds.Height;
            numeric.Minimum = (decimal)Min;
            numeric.Maximum = (decimal)Max;
            numeric.Increment = (decimal)Step;
            numeric.DecimalPlaces = 0;
            if (DefaultValue != null)
            {
                numeric.Value = Convert.ToDecimal(DefaultValue);
            }
            toolTip.SetToolTip(numeric, Tip);
            this.BoundControl = numeric;
            return numeric;
        }
    }

    /// <summary>
    /// 浮点数字框配置项（使用NumericUpDown，可设置小数位数）
    /// </summary>
    public class NumericFloatConfigItem : ConfigItem
    {
        public double Min { get; set; }
        public double Max { get; set; }
        public double Step { get; set; }
        public int DecimalPlaces { get; set; }

        public NumericFloatConfigItem()
        {
            ControlType = ConfigControlType.NumericFloat;
        }

        public override Control CreateControl(Rectangle bounds, Font font, ToolTip toolTip)
        {
            NumericUpDown numeric = new NumericUpDown();
            numeric.Font = font;
            numeric.Width = bounds.Width;
            numeric.Height = bounds.Height;
            numeric.Minimum = (decimal)Min;
            numeric.Maximum = (decimal)Max;
            numeric.Increment = (decimal)Step;
            numeric.DecimalPlaces = DecimalPlaces;
            if (DefaultValue != null)
            {
                numeric.Value = Convert.ToDecimal(DefaultValue);
            }
            toolTip.SetToolTip(numeric, Tip);
            this.BoundControl = numeric;
            return numeric;
        }
    }

    /// <summary>
    /// 勾选框配置项
    /// </summary>
    public class CheckBoxConfigItem : ConfigItem
    {
        public string VisibleItem { get; set; }
        public string ValueItem { get; set; }

        public CheckBoxConfigItem()
        {
            ControlType = ConfigControlType.CheckBox;
        }

        public override Control CreateControl(Rectangle bounds, Font font, ToolTip toolTip)
        {
            CheckBox chk = new CheckBox();
            chk.Font = font;
            chk.Width = bounds.Width;
            chk.Height = bounds.Height;
            chk.Text = VisibleItem;
            if (DefaultValue != null)
            {
                chk.Checked = Convert.ToBoolean(DefaultValue);
            }
            toolTip.SetToolTip(chk, Tip);
            this.BoundControl = chk;
            return chk;
        }
    }

    /// <summary>
    /// 配置组，内部保存一组配置项，创建时以GroupBox方式显示
    /// </summary>
    public class ConfigGroup
    {
        public string LabelName { get;}
        public string SectionName { get; }

        public List<ConfigItem> Items { get; set; } = new List<ConfigItem>();

        public ConfigGroup(string labelName, string sectionName)
        {
            LabelName = labelName;
            SectionName = sectionName;
        }

        /// <summary>
        /// 根据组内配置项创建GroupBox
        /// </summary>
        public GroupBox CreateGroupBox(Font font, int groupBoxWidth, int startY, ToolTip toolTip, Color foreColor)
        {
            GroupBox groupBox = new GroupBox();
            groupBox.Text = LabelName;
            groupBox.Font = font;
            groupBox.Left = 10;
            groupBox.Top = startY;
            groupBox.Width = groupBoxWidth - 20;
            groupBox.ForeColor = foreColor;

            // 计算“10个中文字符”的宽度（这里简单采用汉字“汉”的宽度测量）
            SizeF charSize = TextRenderer.MeasureText("汉", font);
            int labelWidth = (int)(charSize.Width * 8);
            // 配置项高度：1.3倍字高
            int itemHeight = (int)(font.Height * 1.3);
            // 每项间隔 0.5 个字高
            int verticalSpacing = (int)(font.Height * 0.5);
            // 控件宽度为剩余宽度（预留左右边距）
            int controlWidth = groupBox.Width - labelWidth - 3 * 10;

            int currentY = font.Height;
            foreach (var item in Items)
            {
                // 左侧标签
                Label label = item.CreateLabel(font, labelWidth, itemHeight);
                label.Left = 10;
                label.Top = currentY;
                groupBox.Controls.Add(label);

                // 右侧编辑控件
                Rectangle ctrlRect = new Rectangle(label.Right + 10, currentY, controlWidth, itemHeight);
                Control ctrl = item.CreateControl(ctrlRect, font, toolTip);
                ctrl.Left = label.Right + 10;
                ctrl.Top = currentY;
                ctrl.ForeColor = foreColor;
                groupBox.Controls.Add(ctrl);

                currentY += itemHeight + verticalSpacing;
            }
            groupBox.Height = currentY + 10;
            return groupBox;
        }
    }

    /// <summary>
    /// 配置界面描述类，保存多个配置组，并提供在父Panel上生成界面的方法
    /// </summary>
    public class ConfigUI
    {
        public List<ConfigGroup> Groups { get; set; } = new List<ConfigGroup>();

        public void BuildUI(Panel parentPanel, ToolTip toolTip, Color foreColor)
        {
            parentPanel.Controls.Clear();
            Font font = parentPanel.Font;
            int parentWidth = parentPanel.Width;

            SizeF charSize = TextRenderer.MeasureText("汉", font);

            int currentX = 10;
            int currentY = 10;
            int horizontalSpacing = 10;
            int verticalSpacing = 10;
            // 固定每个组的宽度，比如300像素
            int groupWidth = (int)(charSize.Width * 16); ;
            // 当前行最高的GroupBox高度，用于换行时计算Y坐标
            int maxRowHeight = 0;

            foreach (var group in Groups)
            {
                // 调用CreateGroupBox时将宽度固定传入，Y坐标暂时设为0，后面再调整位置
                GroupBox gb = group.CreateGroupBox(font, groupWidth, 0, toolTip, foreColor);

                // 如果横向空间不足，则换行
                if (currentX + gb.Width > parentWidth)
                {
                    currentX = 10;
                    currentY += maxRowHeight + verticalSpacing;
                    maxRowHeight = 0;
                }

                gb.Left = currentX;
                gb.Top = currentY;
                parentPanel.Controls.Add(gb);

                currentX += gb.Width + horizontalSpacing;
                if (gb.Height > maxRowHeight)
                {
                    maxRowHeight = gb.Height;
                }
            }
        }

        // 将 IniParser 中的值加载到界面各个控件中
        public void LoadFromIni(IniParser parser)
        {
            foreach (var group in Groups)
            {
                foreach (var item in group.Items)
                {
                    try
                    {
                        string valueStr = parser.GetValue(group.SectionName, item.Key);
                        if (!string.IsNullOrEmpty(valueStr) && item.BoundControl != null)
                        {
                            switch (item.ControlType)
                            {
                                case ConfigControlType.TextBox:
                                    ((TextBox)item.BoundControl).Text = valueStr;
                                    break;
                                case ConfigControlType.DropDown:
                                    ((ComboBox)item.BoundControl).SelectedValue = valueStr;
                                    break;
                                case ConfigControlType.NumericInteger:
                                    if (decimal.TryParse(valueStr, out decimal intVal))
                                        ((NumericUpDown)item.BoundControl).Value = intVal;
                                    break;
                                case ConfigControlType.NumericFloat:
                                    if (decimal.TryParse(valueStr, out decimal floatVal))
                                        ((NumericUpDown)item.BoundControl).Value = floatVal;
                                    break;
                                case ConfigControlType.CheckBox:
                                    if (bool.TryParse(valueStr, out bool boolVal))
                                        ((CheckBox)item.BoundControl).Checked = boolVal;
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"加载配置项 {group.SectionName}:{item.Key} 失败: {ex.Message}");
                    }
                }
            }
        }

        // 从界面各控件取值更新到 IniParser 中
        public void UpdateIniFromUI(IniParser parser)
        {
            foreach (var group in Groups)
            {
                foreach (var item in group.Items)
                {
                    try
                    {
                        string newValue = "";
                        if (item.BoundControl != null)
                        {
                            switch (item.ControlType)
                            {
                                case ConfigControlType.TextBox:
                                    newValue = ((TextBox)item.BoundControl).Text;
                                    break;
                                case ConfigControlType.DropDown:
                                    newValue = ((ComboBox)item.BoundControl).SelectedValue?.ToString() ?? "";
                                    break;
                                case ConfigControlType.NumericInteger:
                                case ConfigControlType.NumericFloat:
                                    newValue = ((NumericUpDown)item.BoundControl).Value.ToString();
                                    break;
                                case ConfigControlType.CheckBox:
                                    newValue = ((CheckBox)item.BoundControl).Checked.ToString();
                                    break;
                            }
                            parser.SetValue(group.SectionName, item.Key, newValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"更新配置项 {group.SectionName}:{item.Key} 失败: {ex.Message}");
                    }
                }
            }
        }

        // 还原界面控件为各配置项的默认值
        public void RestoreDefaults()
        {
            foreach (var group in Groups)
            {
                foreach (var item in group.Items)
                {
                    try
                    {
                        if (item.BoundControl != null)
                        {
                            switch (item.ControlType)
                            {
                                case ConfigControlType.TextBox:
                                    ((TextBox)item.BoundControl).Text = item.DefaultValue?.ToString() ?? "";
                                    break;
                                case ConfigControlType.DropDown:
                                    ((ComboBox)item.BoundControl).SelectedValue = item.DefaultValue?.ToString() ?? "";
                                    break;
                                case ConfigControlType.NumericInteger:
                                case ConfigControlType.NumericFloat:
                                    if (decimal.TryParse(item.DefaultValue.ToString(), out decimal defVal))
                                        ((NumericUpDown)item.BoundControl).Value = defVal;
                                    break;
                                case ConfigControlType.CheckBox:
                                    ((CheckBox)item.BoundControl).Checked = Convert.ToBoolean(item.DefaultValue);
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"还原默认值 {group.SectionName}:{item.Key} 失败: {ex.Message}");
                    }
                }
            }
        }


        /// <summary>
        /// 测试窗体，根据ini描述生成如下界面：
        /// 
        /// [服务程序]
        /// ProductType=v53 //产品名，：v53、v54。
        /// Factory=AAC //工厂名，：AAC、AVC。
        /// Gpuid=0 //GPU的ID号，：0、1。
        /// InferType=1 //GPU加速，：0、1。0:快速；1:极速
        /// InferParallel=1 //GPU运行方式，：0、1。0:串行推理；1:并行推理
        ///
        /// [白点过滤]
        /// WhiteDotAreaLmt1=6
        /// WhiteDotVarianceLmt1=160
        /// WhiteDotAreaLmt2=15
        /// WhiteDotVarianceLmt2=800
        ///
        /// [异物限位]
        /// ParticleAreaLmt1=9
        /// ParticleVarianceLmt1=90
        /// ParticleAreaLmt2=23
        /// ParticleVarianceLmt2=190
        ///
        /// [杂项]
        /// MergeDistance=30
        /// RaisedAreaLmt=12
        /// ParticleAreaLmtInWeld=20
        /// PixelPerMm=0.03
        ///
        /// [异物分隔]
        /// ParticleDivideAspectRatio=1.20
        /// ParticleDivideArea = 20.00
        ///
        /// [异物过滤]
        /// ParticleFilterX1 = 10.54
        /// ParticleFilterY1 = 0.43
        /// ParticleFilterA1 = 0.00
        /// ParticleFilterU1 = 4.00
        ///
        /// [黑点过滤]
        /// ParticleFilterX2 = 0.52
        /// ParticleFilterY2 = 0.28
        /// ParticleFilterA2 = 0.08
        /// ParticleFilterU2 = 4.00
        /// </summary>         
        public static ConfigUI LoadSampleAIConfigUI()
        {
            // 构造ConfigUI
            var configUI = new ConfigUI();

            // ===== [服务程序] =====
            ConfigGroup groupService = new ConfigGroup("服务程序", "Settings");
            groupService.Items.Add(new DropDownConfigItem
            {
                Key = "ProductType",
                LabelText = "产品名",
                DefaultValue = "v53",
                Tip = "产品名，选项：v53、v54",
                AllowEmpty = false,
                DisplayItems = new List<string> { "v53", "v54" },
                ValueItems = new List<string> { "v53", "v54" }
            });
            groupService.Items.Add(new DropDownConfigItem
            {
                Key = "Factory",
                LabelText = "工厂名",
                DefaultValue = "AAC",
                Tip = "工厂名，选项：AAC、AVC",
                AllowEmpty = false,
                DisplayItems = new List<string> { "AAC", "AVC" },
                ValueItems = new List<string> { "AAC", "AVC" }
            });
            groupService.Items.Add(new NumericIntegerConfigItem
            {
                Key = "Gpuid",
                LabelText = "GPU的ID号",
                DefaultValue = 0,
                Tip = "GPU的ID号，选项：0、1",
                Min = 0,
                Max = 1,
                Step = 1
            });
            groupService.Items.Add(new DropDownConfigItem
            {
                Key = "InferType",
                LabelText = "GPU加速",
                DefaultValue = "1",
                Tip = "GPU加速，0:快速；1:极速",
                AllowEmpty = false,
                DisplayItems = new List<string> { "快速", "极速" },
                ValueItems = new List<string> { "0", "1" }
            });
            groupService.Items.Add(new DropDownConfigItem
            {
                Key = "InferParallel",
                LabelText = "GPU运行方式",
                DefaultValue = "1",
                Tip = "GPU运行方式，0:串行推理；1:并行推理",
                AllowEmpty = false,
                DisplayItems = new List<string> { "串行推理", "并行推理" },
                ValueItems = new List<string> { "0", "1" }
            });
            configUI.Groups.Add(groupService);

            // ===== [白点过滤] =====
            ConfigGroup groupWhiteDot = new ConfigGroup("白点过滤", "Settings");
            groupWhiteDot.Items.Add(new NumericIntegerConfigItem
            {
                Key = "WhiteDotAreaLmt1",
                LabelText = "白点限位面积1",
                DefaultValue = 6,
                Tip = "低于此面积的白点将直接过滤掉",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupWhiteDot.Items.Add(new NumericIntegerConfigItem
            {
                Key = "WhiteDotVarianceLmt1",
                LabelText = "白点限位方差1",
                DefaultValue = 160,
                Tip = "低于此方差的白点将直接过滤掉",
                Min = 0,
                Max = 10000,
                Step = 1
            });
            groupWhiteDot.Items.Add(new NumericIntegerConfigItem
            {
                Key = "WhiteDotAreaLmt2",
                LabelText = "白点限位面积2",
                DefaultValue = 15,
                Tip = "低于此面积且低于方差2的白点将过滤掉",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupWhiteDot.Items.Add(new NumericIntegerConfigItem
            {
                Key = "WhiteDotVarianceLmt2",
                LabelText = "白点限位方差2",
                DefaultValue = 800,
                Tip = "低于此方差且低于面积2的白点将过滤掉",
                Min = 0,
                Max = 10000,
                Step = 1
            });
            configUI.Groups.Add(groupWhiteDot);

            // ===== [异物限位] =====
            ConfigGroup groupParticleLimit = new ConfigGroup("异物限位", "Settings");
            groupParticleLimit.Items.Add(new NumericIntegerConfigItem
            {
                Key = "ParticleAreaLmt1",
                LabelText = "异物限位面积1",
                DefaultValue = 9,
                Tip = "低于此面积的异物将直接过滤掉",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupParticleLimit.Items.Add(new NumericIntegerConfigItem
            {
                Key = "ParticleVarianceLmt1",
                LabelText = "异物限位方差1",
                DefaultValue = 90,
                Tip = "低于此方差的异物将直接过滤掉",
                Min = 0,
                Max = 10000,
                Step = 1
            });
            groupParticleLimit.Items.Add(new NumericIntegerConfigItem
            {
                Key = "ParticleAreaLmt2",
                LabelText = "异物限位面积2",
                DefaultValue = 23,
                Tip = "低于此面积且低于方差2的异物将过滤掉",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupParticleLimit.Items.Add(new NumericIntegerConfigItem
            {
                Key = "ParticleVarianceLmt2",
                LabelText = "异物限位方差2",
                DefaultValue = 190,
                Tip = "低于此方差且低于面积2的异物将过滤掉",
                Min = 0,
                Max = 10000,
                Step = 1
            });
            configUI.Groups.Add(groupParticleLimit);

            // ===== [杂项] =====
            ConfigGroup groupMisc = new ConfigGroup("杂项", "Settings");
            groupMisc.Items.Add(new NumericIntegerConfigItem
            {
                Key = "MergeDistance",
                LabelText = "异物与溢胶距离",
                DefaultValue = 30,
                Tip = "异物与溢胶合并检测框的相邻距离",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupMisc.Items.Add(new NumericIntegerConfigItem
            {
                Key = "RaisedAreaLmt",
                LabelText = "凸点缺失最小面积",
                DefaultValue = 12,
                Tip = "低于此面积的凸点将过滤掉",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupMisc.Items.Add(new NumericIntegerConfigItem
            {
                Key = "ParticleAreaLmtInWeld",
                LabelText = "焊点处异物限位面积",
                DefaultValue = 20,
                Tip = "黑点在焊点处的异物限位面积，小于此值的参与过滤",
                Min = 0,
                Max = 1000,
                Step = 1
            });
            groupMisc.Items.Add(new NumericFloatConfigItem
            {
                Key = "PixelPerMm",
                LabelText = "像素大小(mm)",
                DefaultValue = 0.03,
                Tip = "一个像素对应多少毫米",
                Min = 0,
                Max = 10,
                Step = 0.01,
                DecimalPlaces = 2
            });
            configUI.Groups.Add(groupMisc);

            // ===== [异物分隔] =====
            ConfigGroup groupParticleDivide = new ConfigGroup("异物分隔", "Settings");
            groupParticleDivide.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleDivideAspectRatio",
                LabelText = "异物宽高比",
                DefaultValue = 1.20,
                Tip = "分隔异物时使用的宽高比阈值",
                Min = 0,
                Max = 10,
                Step = 0.01,
                DecimalPlaces = 2
            });
            groupParticleDivide.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleDivideArea",
                LabelText = "异物面积",
                DefaultValue = 20.00,
                Tip = "分隔异物的面积阈值",
                Min = 0,
                Max = 1000,
                Step = 0.1,
                DecimalPlaces = 2
            });
            configUI.Groups.Add(groupParticleDivide);

            // ===== [异物过滤] =====
            ConfigGroup groupParticleFilter = new ConfigGroup("异物过滤", "Settings");
            groupParticleFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterX1",
                LabelText = "过滤标准X1",
                DefaultValue = 10.54,
                Tip = "过滤标准X1（Length，单位mm）",
                Min = 0,
                Max = 100,
                Step = 0.1,
                DecimalPlaces = 2
            });
            groupParticleFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterY1",
                LabelText = "过滤标准Y1",
                DefaultValue = 0.43,
                Tip = "过滤标准Y1（Width，单位mm）",
                Min = 0,
                Max = 100,
                Step = 0.01,
                DecimalPlaces = 2
            });
            groupParticleFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterA1",
                LabelText = "过滤标准A1",
                DefaultValue = 0.00,
                Tip = "过滤标准A1（Area，单位mm²）",
                Min = 0,
                Max = 100,
                Step = 0.1,
                DecimalPlaces = 2
            });
            groupParticleFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterU1",
                LabelText = "过滤标准QTY1",
                DefaultValue = 4.00,
                Tip = "过滤标准QTY（per unit）",
                Min = 0,
                Max = 100,
                Step = 0.1,
                DecimalPlaces = 2
            });
            configUI.Groups.Add(groupParticleFilter);

            // ===== [黑点过滤] =====
            ConfigGroup groupBlackFilter = new ConfigGroup("黑点过滤", "Settings");
            groupBlackFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterX2",
                LabelText = "过滤标准X2",
                DefaultValue = 0.52,
                Tip = "过滤标准X2（X dimension，单位mm）",
                Min = 0,
                Max = 100,
                Step = 0.01,
                DecimalPlaces = 2
            });
            groupBlackFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterY2",
                LabelText = "过滤标准Y2",
                DefaultValue = 0.28,
                Tip = "过滤标准Y2（Y dimension，单位mm）",
                Min = 0,
                Max = 100,
                Step = 0.01,
                DecimalPlaces = 2
            });
            groupBlackFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterA2",
                LabelText = "过滤标准A2",
                DefaultValue = 0.08,
                Tip = "过滤标准A2（Area，单位mm²）",
                Min = 0,
                Max = 100,
                Step = 0.01,
                DecimalPlaces = 2
            });
            groupBlackFilter.Items.Add(new NumericFloatConfigItem
            {
                Key = "ParticleFilterU2",
                LabelText = "过滤标准QTY2",
                DefaultValue = 4.00,
                Tip = "过滤标准QTY（per unit）",
                Min = 0,
                Max = 100,
                Step = 0.1,
                DecimalPlaces = 2
            });
            configUI.Groups.Add(groupBlackFilter);

            return configUI;
            // 最后将界面绘制到Panel上
            //configUI.BuildUI(panel);
        }
    }

    // 一个解析INI文件类，能解析这文件包括注释(//后的内容)（string section, string key, string value）
    // 能查询是否存在（section、key）、读值、更新值，能保存这文件（带上注释）。
    public class IniParser
    {
        // 定义每一行的类型
        public enum LineType { Blank, Comment, Section, KeyValue }

        // 定义表示文件中一行的结构
        public class Line
        {
            public LineType Type;
            public string Raw;          // 原始文本（备用，可用于调试）
            public string SectionName;  // 如果是 section 行，则存储 section 名称
            public string Key;          // 如果是键值行，则存储 key
            public string Value;        // 如果是键值行，则存储 value
            public string Comment;      // 整行注释或键值行内的注释
        }

        // 保存文件中所有行
        private List<Line> lines = new List<Line>();

        /// <summary>
        /// 从文件加载 INI 配置
        /// </summary>
        public bool LoadFromFile(string filename)
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine("文件不存在：" + filename);
                return false;
            }

            lines.Clear();
            string currentSection = "";
            foreach (string line in File.ReadAllLines(filename))
            {
                Line parsedLine = ParseLine(line);
                lines.Add(parsedLine);

                // 如果遇到 section 行，更新当前 section（后续键值行属于该 section）
                if (parsedLine.Type == LineType.Section)
                {
                    currentSection = parsedLine.SectionName;
                }
            }
            return true;
        }

        /// <summary>
        /// 保存 INI 配置到文件，保存时会保留注释和空行
        /// </summary>
        public bool SaveToFile(string filename)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filename))
                {
                    foreach (var line in lines)
                    {
                        writer.WriteLine(ComposeLine(line));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("写入文件失败：" + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 判断指定 section 是否存在
        /// </summary>
        public bool HasSection(string section)
        {
            foreach (var line in lines)
            {
                if (line.Type == LineType.Section && line.SectionName == section)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断指定 section 中是否存在某个 key
        /// </summary>
        public bool HasKey(string section, string key)
        {
            bool inSection = false;
            foreach (var line in lines)
            {
                if (line.Type == LineType.Section)
                {
                    inSection = (line.SectionName == section);
                }
                else if (inSection && line.Type == LineType.KeyValue)
                {
                    if (line.Key == key)
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取指定 section 下 key 的值，若不存在则返回空字符串
        /// </summary>
        public string GetValue(string section, string key)
        {
            bool inSection = false;
            foreach (var line in lines)
            {
                if (line.Type == LineType.Section)
                    inSection = (line.SectionName == section);
                else if (inSection && line.Type == LineType.KeyValue)
                {
                    if (line.Key == key)
                        return line.Value;
                }
            }
            return "";
        }

        /// <summary>
        /// 设置指定 section 下 key 的值，若不存在则自动添加（若 section 不存在，则先添加 section）
        /// </summary>
        public void SetValue(string section, string key, string value)
        {
            bool sectionFound = false;
            bool inSection = false;
            int insertPos = lines.Count; // 记录插入位置

            // 遍历文件中所有行，查找指定 section 和 key
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (line.Type == LineType.Section)
                {
                    if (line.SectionName == section)
                    {
                        sectionFound = true;
                        inSection = true;
                        // 记录 section 行后第一个可插入的位置
                        insertPos = i + 1;
                    }
                    else
                    {
                        inSection = false;
                    }
                }
                else if (inSection && line.Type == LineType.KeyValue)
                {
                    if (line.Key == key)
                    {
                        // 找到则直接更新 value
                        line.Value = value;
                        return;
                    }
                    // 更新插入位置到当前行之后
                    insertPos = i + 1;
                }
            }

            // 如果 section 存在但未找到 key，则在该 section 末尾插入新的键值行
            if (sectionFound)
            {
                Line newLine = new Line
                {
                    Type = LineType.KeyValue,
                    Key = key,
                    Value = value
                };
                lines.Insert(insertPos, newLine);
            }
            else
            {
                // 如果 section 不存在，则先添加 section，再添加键值行
                Line secLine = new Line
                {
                    Type = LineType.Section,
                    SectionName = section
                };
                lines.Add(secLine);
                Line newLine = new Line
                {
                    Type = LineType.KeyValue,
                    Key = key,
                    Value = value
                };
                lines.Add(newLine);
            }
        }

        /// <summary>
        /// 解析单行文本，判断行类型并提取 section/key/value 和注释
        /// </summary>
        private Line ParseLine(string line)
        {
            Line result = new Line();
            result.Raw = line;
            string trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                result.Type = LineType.Blank;
                return result;
            }
            // 如果整行是注释（以 ";" 或 "//" 开头）
            if (trimmed.StartsWith(";") || trimmed.StartsWith("//"))
            {
                result.Type = LineType.Comment;
                result.Comment = trimmed;
                return result;
            }
            // 如果以 [ 开头并以 ] 结尾，则为 section 行
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                result.Type = LineType.Section;
                result.SectionName = trimmed.Substring(1, trimmed.Length - 2).Trim();
                return result;
            }
            // 否则，尝试以第一个 "=" 分隔解析为 key=value
            int pos = trimmed.IndexOf('=');
            if (pos >= 0)
            {
                result.Type = LineType.KeyValue;
                result.Key = trimmed.Substring(0, pos).Trim();
                string remainder = trimmed.Substring(pos + 1).Trim();
                // 检查是否存在内嵌注释（以"//"为标记）
                int commentPos = remainder.IndexOf("//");
                if (commentPos >= 0)
                {
                    result.Value = remainder.Substring(0, commentPos).Trim();
                    result.Comment = remainder.Substring(commentPos).Trim();
                }
                else
                {
                    result.Value = remainder;
                }
                return result;
            }
            // 如果都不匹配，则当作注释行处理
            result.Type = LineType.Comment;
            result.Comment = trimmed;
            return result;
        }

        /// <summary>
        /// 根据行类型重新组合成字符串，用于写回文件
        /// </summary>
        private string ComposeLine(Line line)
        {
            switch (line.Type)
            {
                case LineType.Blank:
                    return "";
                case LineType.Comment:
                    return line.Comment;
                case LineType.Section:
                    return $"[{line.SectionName}]";
                case LineType.KeyValue:
                    {
                        string composed = $"{line.Key}={line.Value}";
                        if (!string.IsNullOrEmpty(line.Comment))
                        {
                            // 如果内嵌注释未以 "//" 或 ";" 开头，则自动添加 "//"
                            if (!(line.Comment.StartsWith("//") || line.Comment.StartsWith(";")))
                                composed += " //" + line.Comment;
                            else
                                composed += " " + line.Comment;
                        }
                        return composed;
                    }
                default:
                    return line.Raw;
            }
        }

        // ===== 使用示例 =====
        public static void Test(string filePath)
        {
            IniParser parser = new IniParser();

            // 加载 INI 文件
            if (!parser.LoadFromFile(filePath))
            {
                Console.WriteLine("加载 INI 文件失败！");
                return;
            }

            // 查询指定 section/key 是否存在
            if (parser.HasSection("Settings"))
            {
                Console.WriteLine("[Settings] 节存在。");
            }
            if (parser.HasKey("Settings", "ProductType"))
            {
                Console.WriteLine("ProductType = " + parser.GetValue("Settings", "ProductType"));
            }

            // 更新键值，如果键不存在则会自动添加
            parser.SetValue("Settings", "ProductType", "v54");

            // 添加新的 section 和键值
            parser.SetValue("NewSection", "NewKey", "NewValue");

            // 保存更新后的文件，保留原有注释信息
            if (!parser.SaveToFile("config_updated.ini"))
            {
                Console.WriteLine("保存 INI 文件失败！");
            }
            else
            {
                Console.WriteLine("更新后的 INI 文件已保存为 config_updated.ini");
            }
        }
    }
}
