using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloEncoding : Form
    {
        public HelloEncoding()
        {
            InitializeComponent();
        }

        private void OutInfo(string message)
        {
            this.richTextBox1.AppendText(message);
            this.richTextBox1.AppendText("\n\n");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var s1 = "hello国田回复😊🤖";
            OutInfo($"sl Length: {s1.Length}");
            var cs = s1.ToCharArray();
            OutInfo($"cs Length: {cs.Length}");
            // 将 string 转换为 UTF-8 字节序列
            byte[] utf8Bytes = Encoding.UTF8.GetBytes(cs);
            var utf8String = Encoding.UTF8.GetString(utf8Bytes);
            var csUtf8 = utf8String.ToCharArray();
            OutInfo($"utf8String Length: {utf8String.Length}, char Length: {csUtf8.Length}");
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            var s1 = "hello国田回复😊🤖";
            var cs = s1.ToCharArray();

            byte[] utf32Bytes = Encoding.UTF32.GetBytes(cs);
            var utf32String = Encoding.UTF32.GetString(utf32Bytes);
            var csUtf32 = utf32String.ToCharArray();
            OutInfo($"utf32String Length: {utf32String.Length}, char Length: {csUtf32.Length}");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string text = "Hello, 👋 你好，🌍!";
            // 使用 StringInfo 类来处理可能包含代理对的字符串
            var textEnum = StringInfo.GetTextElementEnumerator(text);
            while (textEnum.MoveNext())
            {
                // 获取当前的文本元素（可能是一个字符或代理对）
                string textElement = textEnum.GetTextElement();
                // 输出当前的文本元素
                OutInfo(textElement);
            }
        }
    }
}
