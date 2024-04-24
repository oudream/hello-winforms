using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.Actions
{
    public partial class PRight : UserControl
    {
        public PRight()
        {
            InitializeComponent();
        }

        public Panel GetPanel()
        {
            return this.panel;
        }
    }
}
