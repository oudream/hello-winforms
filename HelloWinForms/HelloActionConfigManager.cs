using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/**
 * 
Action界面及执行的管理器，实现注册方式，注册项的基础信息
有：按钮图标、Title、Description、是否需要配置、配置类型、配置界面、配置检查接口、配置应用接口。
管理器需要根据是否需要配置，如果不需要直接调用配置应用接口，如果需要配置，要根据配置类型来显示界面。
配置类型分三种。
第一种是横放方式，Get配置界面的Panel显示在Top；
第二种是坚放方式，Get配置的Panel显示在Right；
第三种是对话框，Get配置对话框，使用模态方式显示；
配置显示后，如果配置检查通过，调用配置应用接口（传入配置Control，由具体注册项自己判断是否自己的界面，获参数后，应用）。 
 */

namespace HelloWinForms
{
    public partial class HelloActionConfigManager : Form
    {
        public HelloActionConfigManager()
        {
            InitializeComponent();
        }
    }
}
