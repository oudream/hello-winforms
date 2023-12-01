using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelloWinForms.ActionUi
{
    public interface IConfigurable
    {
        // 实现配置检查逻辑
        bool CheckConfiguration(Control configControl);

        // 实现配置应用逻辑
        void ApplyConfiguration(Control configControl);

        // 返回对话框方式的配置界面
        Control GetConfigurationControl();
    }
}
