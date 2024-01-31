using HelloRoslyn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using System.Diagnostics;
using System.Reflection;

namespace HelloWinForms
{
    public partial class HelloRoslyn : Form
    {
        public HelloRoslyn()
        {
            InitializeComponent();
        }

        public static void CompileScript()
        {
            string scriptCode = File.ReadAllText("Script/ScriptFileA.csx");
            var syntaxTree = CSharpSyntaxTree.ParseText(scriptCode);

            var references = new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(UtilityClass).Assembly.Location)
            };

            var compilation = CSharpCompilation.Create("ScriptAssembly.dll",
                new[] { syntaxTree },
                references,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);
                if (result.Success)
                {
                    File.WriteAllBytes("ScriptAssembly.dll", ms.ToArray());
                }
            }
        }

        private static void LoadAndExecute()
        {
            var assembly = Assembly.LoadFile("E:\\image\\hello-winforms\\HelloWinForms\\bin\\Debug\\ScriptAssembly.dll");
            var type = assembly.GetType("ScriptMain");
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethod("ExecuteScript");
            var result = method.Invoke(instance, null);
            Console.WriteLine($"Script executed. Result: {result}");

            // 测试动态执行
            var stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < 10000; i++)
            {
                result = method.Invoke(instance, null); // 假设是无参方法
            }
            stopwatch.Stop();
            Console.WriteLine($"Dynamic Execution Time: {stopwatch.ElapsedMilliseconds} ms, result: {result}");

            // 测试本地执行
            stopwatch.Restart();
            for (int i = 0; i < 10000; i++)
            {
                result = UtilityClass.Add(5, 10);
            }
            stopwatch.Stop();
            Console.WriteLine($"Local Execution Time: {stopwatch.ElapsedMilliseconds} ms, result: {result}");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CompileScript();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            LoadAndExecute();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string code1 = @"
public class ScriptedClass
{
    public string HelloWorld { get; set; }
    public ScriptedClass()
    {
        HelloWorld = ""Hello Roslyn!"";
    }
}";

            var script = CSharpScript.RunAsync(code1).Result;

            var result = script.ContinueWithAsync<string>("new ScriptedClass().HelloWorld").Result;

            MessageBox.Show(result.ReturnValue);
        }

        private void button4_Click(object sender, EventArgs e)
        {
        }
    }

    public class MyClass
    {
        public int Add(int a, int b)
        {
            return a + b;
        }
    }
}
