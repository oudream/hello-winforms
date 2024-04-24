using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class IniFileManager
{
    private string _filePath;

    public IniFileManager(string filePath)
    {
        _filePath = filePath;
    }

    public void WriteValue(string section, string key, string value)
    {
        try
        {
            var data = ReadIniFile();
            if (!data.ContainsKey(section))
                data[section] = new Dictionary<string, string>();

            data[section][key] = value;
            WriteIniFile(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing INI file: {ex.Message}");
            // Optionally: throw or handle the error more specifically
        }
    }

    public string ReadValue(string section, string key, string defaultValue = "")
    {
        try
        {
            var data = ReadIniFile();
            if (data.ContainsKey(section) && data[section].ContainsKey(key))
                return data[section][key];

            return defaultValue;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading INI file: {ex.Message}");
            return defaultValue; // Return the default value if there is an error
        }
    }

    private Dictionary<string, Dictionary<string, string>> ReadIniFile()
    {
        var data = new Dictionary<string, Dictionary<string, string>>();
        try
        {
            if (!File.Exists(_filePath))
                return data;

            Dictionary<string, string> currentSection = null;

            foreach (var line in File.ReadAllLines(_filePath))
            {
                var trimmedLine = line.Trim();
                if (trimmedLine.StartsWith(";")) continue; // Skip comments
                if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                {
                    var sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2);
                    if (!data.ContainsKey(sectionName))
                    {
                        currentSection = new Dictionary<string, string>();
                        data[sectionName] = currentSection;
                    }
                }
                else if (currentSection != null)
                {
                    var kvp = trimmedLine.Split(new[] { '=' }, 2);
                    if (kvp.Length == 2)
                        currentSection[kvp[0].Trim()] = kvp[1].Trim();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error parsing INI file: {ex.Message}");
        }
        return data;
    }

    private void WriteIniFile(Dictionary<string, Dictionary<string, string>> data)
    {
        try
        {
            using (var sw = new StreamWriter(_filePath))
            {
                foreach (var section in data)
                {
                    sw.WriteLine($"[{section.Key}]");
                    foreach (var kvp in section.Value)
                    {
                        sw.WriteLine($"{kvp.Key}={kvp.Value}");
                    }
                    sw.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing INI file: {ex.Message}");
        }
    }
}
