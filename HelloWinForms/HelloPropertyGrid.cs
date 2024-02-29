using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms
{
    public partial class HelloPropertyGrid : Form
    {
        public HelloPropertyGrid()
        {
            InitializeComponent();
        }

        private PropertyObject propertyObject;
        private void button1_Click(object sender, EventArgs e)
        {
            // 创建并设置一个对象
            propertyObject = new PropertyObject
            {
                Name = "John Doe",
                Age = 30,
                IsMember = true
            };

            // 将对象绑定到 PropertyGrid
            propertyGrid1.SelectedObject = propertyObject;
        }

        private Object device;
        private void button2_Click(object sender, EventArgs e)
        {
            // 动态加载 DLL
            Assembly deviceAssembly = Assembly.LoadFrom("DeviceCxClassLibrary.dll");

            // 创建 DeviceFactory 类型
            Type deviceFactoryType = deviceAssembly.GetType("DeviceCxClassLibrary.DeviceFactory");

            // 调用静态方法 CreateDevice
            MethodInfo createDeviceMethod = deviceFactoryType.GetMethod("CreateDevice");
            device = createDeviceMethod.Invoke(null, null);

            // 将设备对象绑定到 PropertyGrid
            propertyGrid1.SelectedObject = device;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Type type = typeof(Button);
            label1.Text = type.Name;

            Button instance = Activator.CreateInstance(type) as Button;
            instance.Text = "Hello World";
            this.panel1.Controls.Add(instance);
        }
    }

    // [DisplayName(...)]
    // [Description(...)]
    // [Category(...)]
    // [TypeConverter(...)]
    // [ReadOnly(...)]
    // [Browsable(...)]
    // [DefaultValue(...)]
    // [Editor(...)]

    // 定义一个简单的对象，其属性将显示在 PropertyGrid 中
    public class PropertyObject
    {
        private string name;
        private int age;
        private bool isMember;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        public bool IsMember
        {
            get { return isMember; }
            set { isMember = value; }
        }
    }

}
