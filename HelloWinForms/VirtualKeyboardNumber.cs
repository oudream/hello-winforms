using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{

    public partial class VirtualKeyboardNumber : Form
    {
        // 定义Windows消息常量
        private const int WM_NCACTIVATE = 0x0086;

        public enum InputMode
        {
            Number,
            IPAddress
        }

        private InputMode currentInputMode = InputMode.Number;

        private string[,] keys = {
                {"1", "2", "3", "←"},
                {"4", "5", "6", "R"},
                {"7", "8", "9", "↵"},
                {"0", "", ".", ""}
            };

        private decimal minValue = 0; // 最小值示例
        private decimal maxValue = 100; // 最大值示例

        public decimal UserInputNumber { get; private set; }
        public string UserInputIPAddress { get; private set; }

        public VirtualKeyboardNumber()
        {
            InitializeComponent();

            InitButtons();
        }

        public void Initialize(decimal value, decimal minValue, decimal maxValue)
        {
            currentInputMode = InputMode.Number;
            UserInputNumber = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
            valueTextBox.Text = UserInputNumber.ToString();
        }

        public void Initialize(string value)
        {
            currentInputMode = InputMode.IPAddress;
            UserInputIPAddress = value;
            valueTextBox.Text = value;
        }

        private void InitButtons()
        {
            for (int i = 0; i < tableLayoutPanel.Controls.Count; i++)
            {
                var button = tableLayoutPanel.Controls[i] as Button;
                if (button == null) continue;
                button.Click += Button_Click;
            }
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button == null) return;

            errorLabel.Text = "";

            switch (button.Text)
            {
                case "←":
                    if (valueTextBox.TextLength > 0)
                    {
                        valueTextBox.Text = valueTextBox.Text.Substring(0, valueTextBox.Text.Length - 1);
                    }
                    break;
                case "R":
                    valueTextBox.Text = UserInputNumber.ToString();
                    break;
                case "OK":
                    if (currentInputMode == InputMode.IPAddress)
                    {
                        if (ValidateInput(valueTextBox.Text))
                        {
                            // 如果输入验证通过，则设置UserInputIPAddress并关闭窗体
                            UserInputIPAddress = valueTextBox.Text;
                            this.DialogResult = DialogResult.OK; // 设置对话框结果为OK
                            this.Close(); // 关闭窗体
                        }
                        else
                        {
                            errorLabel.Text = "无效的IP地址输入";
                        }
                    }
                    else
                    {
                        if (ValidateInput(valueTextBox.Text))
                        {
                            // 如果输入验证通过，则设置UserInputValue并关闭窗体
                            UserInputNumber = decimal.Parse(valueTextBox.Text);
                            this.DialogResult = DialogResult.OK; // 设置对话框结果为OK
                            this.Close(); // 关闭窗体
                        }
                        else
                        {
                            errorLabel.Text = $"无效输入，请输入 {minValue} 到 {maxValue} 的数字";
                        }
                    }

                    if (ValidateInput(valueTextBox.Text))
                    {
                        // 如果输入验证通过，则设置UserInputValue并关闭窗体
                        UserInputNumber = decimal.Parse(valueTextBox.Text);
                        this.DialogResult = DialogResult.OK; // 设置对话框结果为OK
                        this.Close(); // 关闭窗体
                    }
                    else
                    {
                        if (currentInputMode == InputMode.IPAddress)
                        {
                            errorLabel.Text = "无效的IP地址输入";
                        }
                        else
                        {
                            errorLabel.Text = $"无效输入，请输入 {minValue} 到 {maxValue} 的数字";
                        }
                    }
                    break;
                case ".":
                    if (currentInputMode == InputMode.IPAddress)
                    {
                        // 对于IP地址模式，允许多个点存在，但需要验证点的数量
                        if (valueTextBox.Text.Count(f => f == '.') < 3)
                        {
                            valueTextBox.Text += ".";
                        }
                    }
                    else
                    {
                        if (!valueTextBox.Text.Contains("."))
                        {
                            valueTextBox.Text += ".";
                        }
                    }
                    break;
                default:
                    string input = valueTextBox.Text + button.Text;
                    if (ValidateInput(input))
                    {
                        valueTextBox.Text = input;
                    }
                    else
                    {
                        if (currentInputMode == InputMode.IPAddress)
                        {
                            errorLabel.Text = "无效的IP地址输入";
                        }
                        else
                        {
                            errorLabel.Text = $"无效输入，请输入 {minValue} 到 {maxValue} 的数字";
                        }
                    }
                    break;
            }
        }

        private bool ValidateInput(string input)
        {
            if (currentInputMode == InputMode.IPAddress)
            {
                // 添加IP地址格式的验证逻辑
                var parts = input.Split('.');
                if (parts.Length > 4) return false; // IP地址最多有4部分
                foreach (var part in parts)
                {
                    if (!int.TryParse(part, out int numPart) || numPart < 0 || numPart > 255)
                    {
                        return false;
                    }
                }
                return true; // 所有部分都有效
            }
            else
            {
                // 验证点的数量
                if (input.Count(f => f == '.') > 1)
                {
                    return false;
                }

                // 验证是否在空白区域
                if (string.IsNullOrEmpty(input))
                {
                    return false;
                }

                // 尝试解析当前文本为数字，并验证是否在允许的范围内
                if (decimal.TryParse(input, out decimal value))
                {
                    if (value >= minValue && value <= maxValue)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        // 重写WndProc方法以监听Windows消息
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCACTIVATE && m.WParam == IntPtr.Zero)
            {
                // 如果WM_NCACTIVATE消息的wParam参数为0，则表示点击了Form外部
                this.Close(); // 关闭Form
            }
            base.WndProc(ref m);
        }

        //public static decimal EditNumic(decimal value)
        //{
        //    HelloVirtualKeyboardNumber keyboardForm = new HelloVirtualKeyboardNumber();
        //    if (keyboardForm.ShowDialog() == DialogResult.OK)
        //    {
        //        decimal userInput = keyboardForm.UserInputValue;
        //    }
        //}

    }
}
