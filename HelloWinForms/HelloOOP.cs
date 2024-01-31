using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloOOP : Form
    {
        public HelloOOP()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var manager = new ObjectManager();
            manager.AddObject(new BaseClass());
            manager.AddObject(new DerivedClass());

            var obj1 = manager.GetObject(0);
            obj1.Show(); // 应该输出 "BaseClass method called."

            var obj2 = manager.GetObject(1);
            obj2.Show(); // 应该输出 "DerivedClass method called."

            // 可以使用类型检查来确定对象的具体类型
            if (obj2 is DerivedClass)
            {
                Console.WriteLine("对象是 DerivedClass 类型");
            }
        }
    }

    // 基类
    public class BaseClass
    {
        public virtual void Show()
        {
            Console.WriteLine("BaseClass method called.");
        }
    }

    // 派生类
    public class DerivedClass : BaseClass
    {
        public override void Show()
        {
            Console.WriteLine("DerivedClass method called.");
        }
    }

    // 管理类
    public class ObjectManager
    {
        private List<BaseClass> objects = new List<BaseClass>();

        public void AddObject(BaseClass obj)
        {
            objects.Add(obj);
        }

        public BaseClass GetObject(int index)
        {
            return objects[index];
        }
    }

}
