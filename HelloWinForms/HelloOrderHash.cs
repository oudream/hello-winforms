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
    public partial class HelloOrderHash : Form
    {
        public HelloOrderHash()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HelloOrder();
        }

        static void HelloOrder()
        {
            // 定义哈希表（Dictionary）
            Dictionary<string, object> hashTable = new Dictionary<string, object>
        {
            { "MeshShift", 1 },
            { "OverflowL", 2 },
            { "Particle", 3 },
            { "NozzleCrack", 4 },
            { "OverflowS", 5 },
            { "MeshCrease", 6 },
            { "WhiteDot", 7 },
            { "CoverShift", 8 }
        };

            // 定义英文到中文的翻译
            Dictionary<string, string> translations = new Dictionary<string, string>
        {
            { "OverflowL", "溢流L" },
            { "OverflowS", "溢流S" },
            { "Particle", "颗粒" },
            { "MeshShift", "网格偏移" },
            { "CoverShift", "盖子偏移" },
            { "NozzleCrack", "喷嘴裂纹" },
            { "WhiteDot", "白点" },
            { "MeshCrease", "网格折痕" }
        };

            // 定义排序顺序
            List<string> sortOrder = new List<string>
        {
            "OverflowL", "OverflowS", "Particle", "MeshShift",
            "CoverShift", "NozzleCrack", "WhiteDot", "MeshCrease"
        };

            // 将哈希表转换为 List 并按自定义顺序排序
            var sortedList = hashTable
                .OrderBy(kvp => sortOrder.IndexOf(kvp.Key)) // 根据sortOrder中的顺序排序
                .Select(kvp => new KeyValuePair<string, object>(translations[kvp.Key], kvp.Value)) // 将键转换为中文
                .ToList();

            string result = string.Join(",", sortedList.Select(item => $"{item.Key}"));
            Console.WriteLine(result);

            // 输出排序并翻译后的结果
            foreach (var item in sortedList)
            {
                Console.WriteLine($"{item.Key}: {item.Value}");
            }
        }
    }
}
