using CommonInterfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloIniFile : Form
    {
        public HelloIniFile()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HelloIniFileManager();
        }

        private void HelloIniFileManager()
        {
            // 计算耗时

            Stopwatch sw = Stopwatch.StartNew();

            var filePath = "settings.ini";
            var iniAdapter = new IniFileAdapter(filePath);

            // 测试写入多个键值对
            var valuesToWrite = new Dictionary<string, string>
        {
            { "Key1", "Value1" },
            { "Key2", "Value2" }
        };

            iniAdapter.WriteValues("ExampleSection1", valuesToWrite);
            iniAdapter.WriteValues("ExampleSection2", valuesToWrite);

            sw.Stop();
            var costWrite = sw.ElapsedMilliseconds;

            // 读取并验证
            var value1 = iniAdapter.ReadValue("ExampleSection1", "Key1");
            var value2 = iniAdapter.ReadValue("ExampleSection2", "Key2");

            OutInfo($"Key1: {value1}");  // 应输出: Value1
            OutInfo($"Key2: {value2}");  // 应输出: Value2

            sw.Restart();

            var values = iniAdapter.ReadIniFile();

            OutInfo($"Key1: {IniFileAdapter.GetValue(values, "ExampleSection1", "Key1")}");  // 应输出: Value1
            OutInfo($"Key2: {IniFileAdapter.GetValue(values, "ExampleSection2", "Key2")}");  // 应输出: Value2

            IniFileAdapter.SetValue(values, "ExampleSection3", "Key3", "Value3");
            IniFileAdapter.SetValue(values, "ExampleSection3", "Key2", "Value2");
            iniAdapter.WriteIniFile(values);

            sw.Stop();
            var costRead = sw.ElapsedMilliseconds;

            IniFileAdapter.PrintData(values, OutInfo);

            OutInfo($"写入耗时：{costWrite}ms, 读取耗时：{costRead}ms");
        }

        private void OutInfo(string message)
        {
            this.richTextBox1.AppendText(message);
            this.richTextBox1.AppendText("\n\n");
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            
        }
    }
}
