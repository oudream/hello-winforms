using NPinyin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloPinyin : Form
    {
        public HelloPinyin()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = PinyinHelper.ConvertToTitleCasePinyin("你好");
        }


    }

    public static class PinyinHelper
    {
        public static string ConvertToTitleCasePinyin(string chineseText)
        {
            string pinyin = Pinyin.GetPinyin(chineseText);
            string[] words = pinyin.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Substring(0, 1).ToUpper() + words[i].Substring(1).ToLower();
            }

            return string.Join("", words);
        }
    }
}
