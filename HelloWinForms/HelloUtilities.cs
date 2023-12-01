using CxWorkStation.Utilities;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HelloWinForms
{
    public partial class HelloUtilities : Form
    {
        public HelloUtilities()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Utilities.Sample.Yaml.Hello.IgnoreUnmatched();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Utilities.Sample.Yaml.Hello.SaveFull();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Utilities.Sample.Yaml.Hello.SavePart();
        }

        private void button4_Click(object sender, EventArgs e)
        {
         
        }

    }




   
}
