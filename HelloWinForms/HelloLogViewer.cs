using CommonInterfaces;
using HelloWinForms.Utilities;
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
    public partial class HelloLogViewer : Form
    {
        private LogViewer logViewer;

        public HelloLogViewer()
        {
            InitializeComponent();

            LogHelper.Run("HelloLogViewer");


            logViewer = new LogViewer(logDataGridView, clearButton, levelWithinCheckBox, filterComboBox,
   tagCheckBox, tagComboBox, addTagButton, clearTagButton, tagLabel);
        }
    }
}
