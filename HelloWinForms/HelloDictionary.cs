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
    public partial class HelloDictionary : Form
    {
        public HelloDictionary()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> context = new Dictionary<string, string>();
            context["a"] = "b";
            context["c"] = "d";
            context["d"] = "e";
            context["e"] = "f";
            Console.WriteLine(context["f"]);
        }
    }
}
