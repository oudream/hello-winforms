using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace HelloWinForms
{
    public partial class HelloVariable : Form
    {
        public HelloVariable()
        {
            InitializeComponent();
        }

        private Dictionary<int, VariableObject> sharedDictionary = new Dictionary<int, VariableObject>();

        private List<VariableObject> sharedList = new List<VariableObject>();
        private readonly int DurationInSeconds = 60; // 总运行时间：60秒
        private object _lockSharedList = new object();

        private void button1_Click(object sender, EventArgs e)
        {
            // 初始化列表
            for (int i = 0; i < 10; i++)
            {
                sharedList.Add(new VariableObject(i, $"Object {i}"));
                sharedDictionary.Add(i, new VariableObject(i, $"Object {i}"));
            }

            // 创建并启动线程
            Thread[] threads = new Thread[10];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(ReadList);
                threads[i].Start();
            }

            // 等待所有线程完成
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine("所有线程已完成读取操作。");
        }

        private void ReadList()
        {
            DateTime endTime = DateTime.Now.AddSeconds(DurationInSeconds);
            DateTime lastReportTime = DateTime.Now;
            int readCount = 0;

            while (DateTime.Now < endTime)
            {
                var count = 0;
                var ss = "";
                //lock (sharedList)
                //{
                //foreach (var item in sharedList)
                //{
                //    // 模拟读取操作
                //    count += item.Id;
                //    ss += item.Name;
                //    if (ss.Length > 100)
                //    {
                //        ss = item.Name;
                //    }
                //}
                //}
                foreach (var kvp in sharedDictionary)
                {
                    // 模拟读取操作
                    count += kvp.Value.Id;
                    ss += kvp.Value.Name;
                    if (ss.Length > 100)
                    {
                        ss = kvp.Value.Name;
                    }
                }

                readCount++;

                // 当前时间与上次报告时间相差超过1秒时输出
                if ((DateTime.Now - lastReportTime).TotalSeconds >= 1)
                {
                    Console.WriteLine($"线程 {Thread.CurrentThread.ManagedThreadId} 在过去的一秒内读取了 {readCount} 次。");
                    readCount = 0; // 重置计数器
                    lastReportTime = DateTime.Now; // 更新上次报告时间
                }
            }
        }
    }

    public class VariableObject
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public VariableObject(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

}
