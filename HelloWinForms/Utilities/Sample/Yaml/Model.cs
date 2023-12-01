using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using CxWorkStation.Utilities;

namespace HelloWinForms.Utilities.Sample.Yaml
{
    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
    }

    public class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public float HeightInInches { get; set; }
        public Dictionary<string, Address> Addresses { get; set; }

    }

    public enum Color
    {
        Red,
        Green,
        Blue
    }

    public class Section
    {
        public string Property1 { get; set; }
        public string Property2 { get; set; }
    }

    public class YamlThree
    {
        public Person Person { get; set; }
        public Section Section { get; set; }
        public Address Address { get; set; }
        public Color Color { get; set; }
    }

    public class YamlSection
    {
        public Section Section { get; set; }
        // Add other sections or properties as needed
    }

}
