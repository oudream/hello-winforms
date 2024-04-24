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

        private void button3_Click(object sender, EventArgs e)
        {
            PlcFeedback feedback = new PlcFeedback();
            feedback.BatchNumber = 1;

            var numberValue = DateTime.Now.Second % 2 == 1 ? (ushort)1 : (ushort)2;

            feedback.Number1 = feedback.Number1 == 2 ? (ushort)2 : numberValue;
        }
    }

}
