using CxWorkStation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using YamlDotNet.Core;
using System.IO;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;
using System.Reflection;

namespace HelloWinForms.Utilities.Sample.Yaml
{
 
    public static class Hello
    {
        // DeserializerBuilder Option:
        // IgnoreUnmatchedProperties()
        // NamingConvention: CamelCaseNamingConvention
        public static void LoadIgnoreUnmatched()
        {
            var yaml = @"
section:
  property1: value1
  property2: value2
object2:
  property3: value1
  property4: value2
object3:
  property3: value1
  property4: value2
";

            var deserializer = YamlHelper.CreateDeserializer().Build();
            var yamlObject = deserializer.Deserialize<YamlSection>(yaml);
            string msg = PrintObjectInfo(yamlObject);
            Console.WriteLine($"yamlObject': {msg}");
        }

        // 先加载出对象模型（模型可以是多层嵌套）
        // 加载时，可以选择自己感觉兴趣部分来加载
        // 再保存为修改的对象（覆盖形式）
        public static void SaveFull()
        {
            // 读取 YAML 文件内容
            string yamlPath = @"E:\tmp\yaml1.yaml";
            string yamlContent = File.ReadAllText(yamlPath);

            // 创建序列化器和反序列化器
            var deserializer = YamlHelper.CreateDeserializer().Build();

            var serializer = YamlHelper.CreateSerializer().Build();

            // 反序列化 YAML 数据
            var yamlObject = deserializer.Deserialize<YamlThree>(yamlContent) ?? new YamlThree();

            if (yamlObject.Person == null)
            {
                yamlObject.Person = new Person();
            }
            if (yamlObject.Section == null)
            {
                yamlObject.Section = new Section();
            }
            if (yamlObject.Address == null)
            {
                yamlObject.Address = new Address();
            }
            DateTime currentDate = DateTime.Now;
            yamlObject.Person.Name = $"Now Is {currentDate:yyyy-MM-dd HH:mm:ss.fff}";
            yamlObject.Person.Age = 31;
            yamlObject.Section.Property1 = "p1";
            yamlObject.Section.Property2 = "p2";
            yamlObject.Address.State = "state1";
            yamlObject.Address.Street = "street1";
            yamlObject.Color = Color.Blue;

            // 将更新后的对象序列化为 YAML
            string updatedYamlContent = serializer.Serialize(yamlObject);

            // 保存更新后的 YAML 数据到文件
            File.WriteAllText(yamlPath, updatedYamlContent);

            Console.WriteLine("YAML file updated successfully.");
        }

        // 先加载出对象模型（模型可以是多层嵌套）
        // 加载时，可以选择自己感觉兴趣部分来加载
        // 再保存需要修改的部分对象
        public static void SavePart()
        {
            string yamlPath = @"E:\tmp\yaml1.yaml";
            var yaml = LoadYamlFromFile(yamlPath);
            if (yaml != null)
            {
                // 获取 YAML 中的第一个文档
                var doc = yaml.Documents[0];
                var root = (YamlMappingNode)doc.RootNode;
                // 找到 person 节点
                var personNode = root.Children[new YamlScalarNode("person")] as YamlMappingNode;
                if (personNode != null)
                {
                    // 修改 person 对象的属性
                    personNode.Children[new YamlScalarNode("name")] = new YamlScalarNode("New Name");
                    var age = new YamlScalarNode("22");
                    age.Style = ScalarStyle.Plain;
                    var heightInInches = new YamlScalarNode("175");
                    heightInInches.Style = ScalarStyle.Plain;
                    personNode.Children[new YamlScalarNode("age")] = age; // 修改为新的年龄
                    personNode.Children[new YamlScalarNode("heightInInches")] = heightInInches; // 修改为新的身高
                    // 将修改后的 YAML 保存到文件
                    SaveYamlToFile(yaml, yamlPath);                                                                                    
                    Console.WriteLine("Person object modified and saved to 'output.yaml'.");
                }
                else
                {
                    Console.WriteLine("Person node not found in the YAML.");
                }
            }
        }

        public static void FindField()
        {
            // 读取整个 YAML 文件内容
            string yamlPath = @"E:\tmp\yaml1.yaml";
            string yamlContent = File.ReadAllText(yamlPath);

            // 将 YAML 文件内容加载到字典
            var deserializer = YamlHelper.CreateDeserializer().Build();
            var yamlDict = deserializer.Deserialize<Dictionary<object, object>>(yamlContent);

            // 找到对应的键值对并修改值
            string keyToModify = "person"; // 修改对应的键
            var newPersonValue = new Person { Name = "新的名字", Age = 30 }; // 新的 Person 对象

            if (yamlDict.ContainsKey(keyToModify))
            {
                yamlDict[keyToModify] = newPersonValue;
            }

            // 将字典保存回文件
            var serializer = YamlHelper.CreateSerializer()
                .EnsureRoundtrip()
                .Build();
            string updatedYamlContent = serializer.Serialize(yamlDict);
            File.WriteAllText(yamlPath, updatedYamlContent);

            Console.WriteLine($"文件 {yamlPath} 已更新");
        }

        // 保存时使用类型转换，枚举默认保存为字符串的，以下方面把枚举转为整形来保存
        public static void SaveEnumToInteger()
        {
            // 读取 YAML 文件内容
            string yamlPath = @"E:\tmp\yaml1.yaml";
            string yamlContent = File.ReadAllText(yamlPath);

            // 创建序列化器和反序列化器
            var deserializer = YamlHelper.CreateDeserializer().Build();

            var serializer = YamlHelper.CreateSerializer()
                .WithTypeConverter(new EnumTypeConverter())
                .Build();

            // 反序列化 YAML 数据
            var yamlObject = deserializer.Deserialize<YamlThree>(yamlContent) ?? new YamlThree();
            if (yamlObject.Person == null)
            {
                yamlObject.Person = new Person();
            }
            if (yamlObject.Section == null)
            {
                yamlObject.Section = new Section();
            }
            if (yamlObject.Address == null)
            {
                yamlObject.Address = new Address();
            }

            DateTime currentDate = DateTime.Now;
            yamlObject.Person.Name = $"Now Is {currentDate:yyyy-MM-dd HH:mm:ss.fff}";
            yamlObject.Person.Age = 31;
            yamlObject.Section.Property1 = "p1";
            yamlObject.Section.Property2 = "p2";
            yamlObject.Address.State = "state1";
            yamlObject.Address.Street = "street1";
            yamlObject.Color = Color.Green;

            // 将更新后的对象序列化为 YAML
            string updatedYamlContent = serializer.Serialize(yamlObject);

            // 保存更新后的 YAML 数据到文件
            File.WriteAllText(yamlPath, updatedYamlContent);

            Console.WriteLine("YAML file updated successfully.");
        }

        private static YamlStream LoadYamlFromFile(string filePath)
        {
            using (var input = new StreamReader(File.OpenRead(filePath)))
            {
                var yaml = new YamlStream();
                yaml.Load(input);
                return yaml;
            }
        }

        private static void SaveYamlToFile(YamlStream yaml, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                yaml.Save(writer);
            }
        }

        public static string PrintObjectInfo(object obj)
        {
            StringBuilder stringBuilder = new StringBuilder();
            PrintObjectInfo(obj, stringBuilder, 0);
            return stringBuilder.ToString();
        }

        private static void PrintObjectInfo(object obj, StringBuilder stringBuilder, int indentationLevel)
        {
            if (obj == null)
            {
                stringBuilder.AppendLine("Object is null.");
                return;
            }

            Type type = obj.GetType();
            string indentation = new string(' ', indentationLevel * 2);

            stringBuilder.AppendLine($"{indentation}Object Type: {type.FullName}");

            foreach (PropertyInfo property in type.GetProperties())
            {
                object value = property.GetValue(obj);

                if (value != null)
                {
                    stringBuilder.AppendLine($"{indentation}{property.Name}: {value}");

                    // 如果属性是一个嵌套对象，则递归调用 PrintObjectInfo 处理嵌套对象
                    if (!property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                    {
                        PrintObjectInfo(value, stringBuilder, indentationLevel + 1);
                    }
                }
            }
        }
    }

    public class EnumTypeConverter : IYamlTypeConverter
    {
        public bool Accepts(Type type)
        {
            return type.IsEnum;
        }

        public object ReadYaml(IParser parser, Type type)
        {
            var value = ((Scalar)parser.Current)?.Value;
            return value != null ? Enum.Parse(type, value) : null;
        }

        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            if (value != null && value is Enum)
            {
                var enumValue = (int)value;
                emitter.Emit(new Scalar(enumValue.ToString()));
            }
            else
            {
                emitter.Emit(new Scalar(string.Empty));
            }
        }
    }

}
