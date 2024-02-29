using HelloWinForms.Utilities.Sample.Yaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace HelloWinForms
{
    public partial class HelloYAML : Form
    {
        public HelloYAML()
        {
            InitializeComponent();
        }

        private void HelloSave()
        {
            var person = new Person
            {
                Name = "John Doe",
                Age = 30,
                Secret = "This is a secret",
                FullName = "John Doe Can",
                Old = 100,
            };
            // 序列化
            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) // 使用驼峰命名规则（camelCase）
                .Build();

            var yaml = serializer.Serialize(person);
            // 将序列化的YAML保存到文件
            File.WriteAllText("person.yaml", yaml);
        }

        private void HelloLoad()
        {
            // 从文件读取YAML
            var yaml = File.ReadAllText("person.yaml");

            // 反序列化
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance) // 使用驼峰命名规则（camelCase）
                .IgnoreUnmatchedProperties()
                .Build();
                            
            var person = deserializer.Deserialize<Person>(yaml);
            richTextBox1.AppendText($"FullName: {person.FullName}, Age: {person.Age}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HelloSave();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HelloLoad();
        }
    }

    public class Person
    {
        [YamlMember(Alias = "simple_name", ApplyNamingConventions = false)]
        public string Name { get; set; }

        [YamlMember(Order = 1)]
        public int Age { get; set; }

        [YamlIgnore]
        public string Secret { get; set; }

        public string FullName;

        internal int Old;

    }
}
