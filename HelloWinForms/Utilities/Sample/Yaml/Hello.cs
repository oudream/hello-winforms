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

namespace HelloWinForms.Utilities.Sample.Yaml
{
 
    public static class Hello
    {
        // DeserializerBuilder Option:
        // IgnoreUnmatchedProperties()
        // NamingConvention: CamelCaseNamingConvention
        public static void IgnoreUnmatched()
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

            Console.WriteLine($"Value of 'Property1': {yamlObject.Section.Property1}");
            Console.WriteLine($"Value of 'Property2': {yamlObject.Section.Property2}");
        }

        public static void SaveFull()
        {
            // 读取 YAML 文件内容
            string yamlPath = @"E:\tmp\yaml1.yaml";
            string yamlContent = File.ReadAllText(yamlPath);

            // 创建序列化器和反序列化器
            var deserializer = YamlHelper.CreateDeserializer().Build();

            var serializer = YamlHelper.CreateSerializer().Build();

            // 反序列化 YAML 数据
            var yamlObject = deserializer.Deserialize<YamlThree>(yamlContent);

            if (yamlObject == null)
            {
                yamlObject = new YamlThree();
            }
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

        public static void SavePart()
        {
            // 读取整个 YAML 文件内容
            string yamlPath = @"E:\tmp\yaml1.yaml";
            string yamlContent = File.ReadAllText(yamlPath);

            // 将 YAML 文件内容加载到字典
            var deserializer = YamlHelper.CreateDeserializer().Build();
            var yamlDict = deserializer.Deserialize<Dictionary<string, object>>(yamlContent);

            // 找到对应的键值对并修改值
            string keyToModify = "person"; // 修改对应的键
            var newPersonValue = new Person { Name = "新的名字", Age = 30 }; // 新的 Person 对象

            if (yamlDict.ContainsKey(keyToModify))
            {
                yamlDict[keyToModify] = newPersonValue;
            }

            // 将字典保存回文件
            var serializer = YamlHelper.CreateSerializer().Build();
            string updatedYamlContent = serializer.Serialize(yamlDict);
            File.WriteAllText(yamlPath, updatedYamlContent);

            Console.WriteLine($"文件 {yamlPath} 已更新");
        }



    }

    public class EnumToIntegerNamingConvention : INamingConvention
    {
        public string Apply(string value)
        {
            return value;
        }

        public string Apply(System.Reflection.MemberInfo member)
        {
            if (member is System.Reflection.FieldInfo fieldInfo && fieldInfo.FieldType.IsEnum)
            {
                var enumValue = (int)Enum.Parse(fieldInfo.FieldType, fieldInfo.Name);
                return enumValue.ToString();
            }

            return member.Name;
        }
    }

}
