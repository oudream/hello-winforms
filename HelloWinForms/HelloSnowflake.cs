using HelloWinForms.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloSnowflake : Form
    {
        public HelloSnowflake()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestSnowflakeManager();
        }

        private void TestSnowflakeManager()
        {
            var idsByType = new Dictionary<DataType, List<long>>();
            var types = Enum.GetValues(typeof(DataType));
            const int idsPerType = 1000;
            var sb = new StringBuilder();
            // 初始化字典
            foreach (DataType type in types)
            {
                idsByType[type] = new List<long>(idsPerType);
            }

            // 开始计时
            Stopwatch stopwatch = Stopwatch.StartNew();

            // 生成ID
            foreach (DataType type in types)
            {
                for (int i = 0; i < idsPerType; i++)
                {
                    long id = SnowflakeManager.GenerateId(type);
                    idsByType[type].Add(id);
                }
            }

            // 停止计时
            stopwatch.Stop();
            sb.AppendLine($"Total time to generate {idsPerType * types.Length} IDs: {stopwatch.ElapsedMilliseconds} ms");

            // 对每种类型随机抽取10个ID并记录
            Random random = new Random();
            foreach (DataType type in types)
            {
                var sampledIds = idsByType[type].OrderBy(x => random.Next()).Take(10).ToList();
                sb.AppendLine($"Sampled IDs for {type}:");

                foreach (var id in sampledIds)
                {
                    var extractedType = SnowflakeManager.GetDataTypeFromId(id);
                    sb.AppendLine($"ID: {id}, Extracted DataType: {extractedType}");
                }
                sb.AppendLine();
            }

            // 显示结果
            MessageBox.Show(sb.ToString(), "Snowflake ID Generation Test");
        }
    }
}
