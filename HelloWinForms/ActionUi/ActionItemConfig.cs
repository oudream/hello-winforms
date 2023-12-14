using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloWinForms.ActionUi
{
    public enum ConfigurationType
    {
        None,
        HorizontalPanel,
        VerticalPanel,
        DialogPanel
    }

    public class ActionUiItemConifig
    {
        public string Catalog { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public bool AtToolBar { get; set; }

        public bool AtMenu { get; set; }

        public ConfigurationType ConfigType { get; set; }

    }
}
