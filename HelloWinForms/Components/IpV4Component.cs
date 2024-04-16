using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media;

namespace HelloWinForms.Components
{
    public class IpV4Component
    {
        private TextBox ipTextBox;

        public IpV4Component(TextBox ipTextBox)
        {
            this.ipTextBox = ipTextBox;
            this.ipTextBox.KeyPress += ipTextBox_KeyPress;
            this.ipTextBox.TextChanged += ipTextBox_TextChanged;
        }

        public void Initialize(string ip)
        {
            ipTextBox.Text = ip;
        }

        private void ipTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // 允许控制字符，例如退格
            if (char.IsControl(e.KeyChar)) return;

            // 允许输入点，但要限制点的数量和位置
            if (e.KeyChar == '.')
            {
                if (textBox.Text.Count(f => f == '.') >= 3 || textBox.Text.Length == 0 || textBox.Text.EndsWith("."))
                {
                    e.Handled = true; // 阻止输入更多的点，或在开始时和连续点的情况下输入点
                    return;
                }
            }
            else if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // 阻止非数字字符
                return;
            }

            // 分割当前文本和即将添加的字符，检查每一部分是否为0-255之间的数字
            string futureText = textBox.Text.Insert(textBox.SelectionStart, e.KeyChar.ToString());
            if (!isValidIpInput(futureText))
            {
                e.Handled = true; // 如果部分超过3位或不在0-255范围内，阻止输入
                return;
            }
        }

        private string lastValidText = "";

        private void ipTextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // 检查文本框中的内容是否为有效的IP地址
            if (isValidIpInput(textBox.Text))
            {
                if (!string.IsNullOrEmpty(textBox.Text))
                {
                    lastValidText = textBox.Text; // 更新最后一次有效的文本
                }
            }
            else
            {
                textBox.Text = lastValidText; // 恢复到最后一次有效的文本
                textBox.SelectionStart = textBox.Text.Length; // 将光标移动到文本末尾
            }
        }

        private bool isValidIpInput(string ipAddress)
        {
            // 使用System.Net.IPAddress.TryParse来检查是否为有效的IP地址
            //return System.Net.IPAddress.TryParse(ipAddress, out _);
            string[] parts = ipAddress.Split('.');
            foreach (var part in parts)
            {
                if (part.Length > 3 || (int.TryParse(part, out int num) && (num < 0 || num > 255)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidIpAddress()
        {
            var ipAddress = ipTextBox.Text;
            if (string.IsNullOrEmpty(ipAddress))
                return false;
            string[] parts = ipAddress.Split('.');
            if (parts.Length != 4)
                return false;
            foreach (var part in parts)
            {
                if (int.TryParse(part, out int num) && (num < 0 || num > 255))
                {
                    return false;
                }
            }
            return System.Net.IPAddress.TryParse(ipAddress, out _);
        }
    }
}
