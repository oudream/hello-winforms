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
    public partial class HelloVirtualKeyboardNumber : Form
    {
        private string[,] keys = {
                {"1", "2", "3", "←"},
                {"4", "5", "6", "R"},
                {"7", "8", "9", "↵"},
                {"0", "", ".", ""}
            };

        private decimal minValue = 0; // 最小值示例
        private decimal maxValue = 100; // 最大值示例

        public decimal UserInputValue { get; private set; }

        public HelloVirtualKeyboardNumber()
        {
            InitializeComponent();

            InitButtons();
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
                    valueTextBox.Text = UserInputValue.ToString();
                    break;
                case "OK":
                    if (ValidateInput(valueTextBox.Text))
                    {
                        // 如果输入验证通过，则设置UserInputValue并关闭窗体
                        UserInputValue = decimal.Parse(valueTextBox.Text);
                        this.DialogResult = DialogResult.OK; // 设置对话框结果为OK
                        this.Close(); // 关闭窗体
                    }
                    else
                    {
                        errorLabel.Text = $"无效输入，请输入 {minValue} 到 {maxValue} 的数字";
                    }
                    break;
                case ".":
                    if (!valueTextBox.Text.Contains("."))
                    {
                        valueTextBox.Text += ".";
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
                        errorLabel.Text = $"无效输入，请输入 {minValue} 到 {maxValue} 的数字";
                    }
                    break;
            }
        }

        private bool ValidateInput(string input)
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
