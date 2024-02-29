using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;


namespace HelloWinForms
{
    public partial class HelloSerializableJson : Form
    {
        public HelloSerializableJson()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
        public void SerializeToBinary(List<PersonSJ> persons, string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                formatter.Serialize(stream, persons);
            }
        }

        public List<PersonSJ> DeserializeFromBinary(string filePath)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                return (List<PersonSJ>)formatter.Deserialize(stream);
            }
        }

        public void SerializeToJson(List<PersonSJ> persons, string filePath)
        {
            string jsonString = JsonConvert.SerializeObject(persons);
            File.WriteAllText(filePath, jsonString);
        }

        public List<PersonSJ> DeserializeFromJson(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<List<PersonSJ>>(jsonString);
        }

        private void HelloSerialize1()
        {
            List<PersonSJ> persons = new List<PersonSJ>();
            int numberOfPersons = 10000; // 可以根据需要调整数量

            for (int i = 0; i < numberOfPersons; i++)
            {
                persons.Add(PersonSJ.GenerateRandomPerson());
            }

            Stopwatch stopwatch = new Stopwatch();

            // 二进制序列化
            stopwatch.Start();
            SerializeToBinary(persons, "persons.bin");
            var personsFromBinary = DeserializeFromBinary("persons.bin");
            stopwatch.Stop();
            Console.WriteLine("Binary Serialization: " + stopwatch.ElapsedMilliseconds + " ms");

            // JSON 序列化
            stopwatch.Restart();
            SerializeToJson(persons, "persons.json");
            var personsFromJson = DeserializeFromJson("persons.json");
            stopwatch.Stop();
            Console.WriteLine("JSON Serialization: " + stopwatch.ElapsedMilliseconds + " ms");
        }
    }

    [Serializable]
    public class PersonSJ
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public DateTime BirthDate { get; set; }

        public PersonSJ() { }

        public PersonSJ(string name, int age, string email, DateTime birthDate)
        {
            Name = name;
            Age = age;
            Email = email;
            BirthDate = birthDate;
        }

        // 生成随机的 PersonSJ 对象
        public static PersonSJ GenerateRandomPerson()
        {
            Random random = new Random();
            return new PersonSJ
            {
                Name = "Person_" + random.Next(1000, 9999),
                Age = random.Next(18, 100),
                Email = $"person{random.Next(1000, 9999)}@example.com",
                BirthDate = new DateTime(random.Next(1950, 2010), random.Next(1, 12), random.Next(1, 28))
            };
        }
    }

}
