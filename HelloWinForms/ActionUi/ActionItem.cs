using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.ActionUi
{
    public class ActionUiItem : ActionUiItemConifig 
    {
        public EventHandler OnClick { get; set; }

        public Control ConfigControl { get; set; }

    }
}
