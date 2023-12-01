using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.ActionUi
{
    public enum ConfigurationType
    {
        None,
        HorizontalPanel,
        VerticalPanel,
        DialogPanel
    }
    
    public delegate string ConfigCheckHandler(Control configControl);

    public delegate string ConfigApplyHandler(Control configControl);
    
    public class ActionUiItem
    {
        public string Catalog { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool AtToolBar { get; set; }

        public bool AtMenu { get; set; }

        public ConfigurationType ConfigType { get; set; }

        public ConfigCheckHandler ConfigCheck { get; set; }
        
        public ConfigApplyHandler ConfigApply { get; set; }

        public Panel ConfigPanel { get; set; }
    }
}
