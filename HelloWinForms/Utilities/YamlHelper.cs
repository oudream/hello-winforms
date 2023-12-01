using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace CxWorkStation.Utilities
{

    public static class YamlHelper
    {
        public static SerializerBuilder CreateSerializer()
        {
            return new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .WithDefaultScalarStyle(ScalarStyle.DoubleQuoted)
                ;
        }

        public static DeserializerBuilder CreateDeserializer()
        {
            return new DeserializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                ;
        }
    }

}


