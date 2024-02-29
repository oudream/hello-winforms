using CxSystemConfiguration.Utilities;
using CxWorkStation.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

/*
Action界面及执行的管理器，实现注册方式，注册项的基础信息
有：按钮图标、Title、Description、是否需要配置、配置类型、配置界面、配置检查接口、配置应用接口。
管理器需要根据是否需要配置，如果不需要直接调用配置应用接口，如果需要配置，要根据配置类型来显示界面。
配置类型分三种[横放、坚放、对话框]。
第一种是横放方式，Get配置界面的Panel显示在Top；
第二种是坚放方式，Get配置的Panel显示在Right；
第三种是对话框，Get配置对话框，使用模态方式显示；
配置应用有两种方式
第一种是实时应用，有两个场景：1、通过鼠标直接在图像上；2、通过点击配置，通过监听配置变化后作处理；这两种场景平台不作处理，需要开发者自己处理。
第二种是配置显示后，如果配置检查通过，调用配置应用接口（传入配置Control，由具体注册项自己判断是否自己的界面，获参数后，应用）。 
 */

namespace HelloWinForms.ActionUi
{
    public static class ActionUiManger
    {
        private static List<ActionUiItemConifig> _actionUiItemConifigs;
        private static List<ActionUiItem> _actionUiItems;

        // 加载 ActionUiItemConifig
        public static void LoadItems()
        {

        }

        private static bool ActionExists(string actionName)
        {
            return _actionUiItems.Any(a => a.Name == actionName);
        }
        
        public static void RegisterActionWithoutConfig(string name, EventHandler onClick)
        {
            ActionUiItem item = _actionUiItems.FirstOrDefault(a => a.Name == name);
            if (item == null)
            {
                ActionUiItemConifig conf = _actionUiItemConifigs.FirstOrDefault(a => a.Name == name);
                if (conf != null)
                {
                     ActionUiItem actionItem = new ActionUiItem
                     {
                         Catalog = conf.Catalog,
                         Name = conf.Name,
                         Title = conf.Title,
                         Description = conf.Description,
                         AtToolBar = conf.AtToolBar,
                         AtMenu = conf.AtMenu,
                         ConfigType = conf.ConfigType,
                         OnClick = onClick,
                         ConfigControl = null,
                     };
                    _actionUiItems.Add(actionItem);
                }
                else
                {
                    LogHelper.Warning($"Action with name '{name}' can not found at ActionUi.config.yml. Skipping registration.");
                }
            }
            else
            {
                LogHelper.Warning($"Action with name '{name}' already exists. Skipping registration.");
            }
        }

        public static void RegisterActionWithConfig(string name, EventHandler onClick, Control configControl)
        {
            ActionUiItem item = _actionUiItems.FirstOrDefault(a => a.Name == name);
            if (item == null)
            {
                ActionUiItemConifig conf = _actionUiItemConifigs.FirstOrDefault(a => a.Name == name);
                if (conf != null)
                {
                    ActionUiItem actionItem = new ActionUiItem
                    {
                        Catalog = conf.Catalog,
                        Name = conf.Name,
                        Title = conf.Title,
                        Description = conf.Description,
                        AtToolBar = conf.AtToolBar,
                        AtMenu = conf.AtMenu,
                        ConfigType = conf.ConfigType,
                        OnClick = onClick,
                        ConfigControl = null,
                    };
                    _actionUiItems.Add(actionItem);
                }
                else
                {
                    LogHelper.Warning($"Action with name '{name}' can not found at ActionUi.config.yml. Skipping registration.");
                }
            }
            else
            {
                LogHelper.Warning($"Action with name '{name}' already exists. Skipping registration.");
            }
        }


        private static void ActionButton_Click(object sender, EventArgs e)
        {
            // 处理按钮点击事件
            Button btn = (Button)sender;
            if ( btn != null ) 
            {
                ActionUiItem item = (ActionUiItem)btn.Tag;
                if ( item != null ) 
                {
                    switch (item.ConfigType)
                    {
                        case ConfigurationType.HorizontalPanel:
                            break;
                        case ConfigurationType.VerticalPanel:
                            break;
                        case ConfigurationType.DialogPanel:
                            break;
                    }
                }
            }

        }

    }
}
