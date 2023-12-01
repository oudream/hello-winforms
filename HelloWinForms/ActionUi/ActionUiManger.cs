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
    public class ActionUiManger
    {
        private List<ActionUiItem> _registeredActions;
        private Control _actionPanel;
        private Panel _horizontalPanel;
        private Control _verticalControl;

        public ActionUiManger(Control actionPanel, Panel horizontalPanel, Control verticalControl)
        {
            _registeredActions = new List<ActionUiItem>();
            _actionPanel = actionPanel;
            _horizontalPanel = horizontalPanel;
            _verticalControl = verticalControl;
        }

        private bool ActionExists(string catalog, string actionName)
        {
            return _registeredActions.Any(a => a.Catalog == catalog && a.Name == actionName);
        }
        
        public void RegisterActionWithoutConfig(string catalog, string name, string title, string description, ConfigApplyHandler applyCallback)
        {
            if (!ActionExists(catalog, name))
            {
                var action = new ActionUiItem
                {
                    Catalog = catalog,
                    Name = name,
                    Title = title,
                    Description = description,
                    ConfigType = ConfigurationType.None,
                    ConfigApply = applyCallback,
                };
                // configPanelCallback?.Invoke(action.ConfigPanel);
                _registeredActions.Add(action);
            }
            else
            {
                Console.WriteLine($"Action with catalog '{catalog}' and name '{name}' already exists. Skipping registration.");
            }
        }

        public void RegisterActionWithConfig(string catalog, string name, string title, string description, ConfigurationType configType, Panel configPanel, ConfigApplyHandler applyCallback)
        {
            if (!ActionExists(catalog, name))
            {
                var action = new ActionUiItem
                {
                    Catalog = catalog,
                    Name = name,
                    Title = title,
                    Description = description,
                    ConfigType = configType,
                    ConfigPanel= configPanel,
                    ConfigApply = applyCallback,
                };
                // configPanelCallback?.Invoke(action.ConfigPanel);
                _registeredActions.Add(action);
            }
            else
            {
                Console.WriteLine($"Action with catalog '{catalog}' and name '{name}' already exists. Skipping registration.");
            }
        }

        public void RegisterActionWithFullConfig(string catalog, string name, string title, string description, ConfigurationType configType, Panel configPanel, ConfigCheckHandler checkHandler, ConfigApplyHandler applyCallback)
        {
            if (!ActionExists(catalog, name))
            {
                var action = new ActionUiItem
                {
                    Catalog = catalog,
                    Name = name,
                    Title = title,
                    Description = description,
                    ConfigType = configType,
                    ConfigPanel = configPanel,
                    ConfigCheck = checkHandler,
                    ConfigApply = applyCallback,
                };
                // configPanelCallback?.Invoke(action.ConfigPanel);
                _registeredActions.Add(action);
            }
            else
            {
                Console.WriteLine($"Action with catalog '{catalog}' and name '{name}' already exists. Skipping registration.");
            }
        }

        public void ExecuteAction(string actionName)
        {
            var action = _registeredActions.Find(a => a.Name == actionName);

            if (action != null)
            {

            }
            else
            {
                Console.WriteLine($"Action '{actionName}' not found.");
            }
        }

        private void ShowConfigurationPanel(ActionUiItem action)
        {
            // Similar to the previous implementation
            // (omitting for brevity)
        }

        private void ApplyConfiguration(ActionUiItem action)
        {
            // Similar to the previous implementation
            // (omitting for brevity)
        }
        

        private void ActionButton_Click(object sender, EventArgs e)
        {
            // 处理按钮点击事件
            Button btn = (Button)sender;
            ActionUiItem item = (ActionUiItem)btn.Tag;

        }

        private void ShowConfiguration(ActionUiItem item)
        {
            // 根据配置类型显示界面
            switch (item.ConfigType)
            {
                case ConfigurationType.HorizontalPanel:
                    ShowConfigPanel(item, DockStyle.Top);
                    break;
                case ConfigurationType.VerticalPanel:
                    ShowConfigPanel(item, DockStyle.Right);
                    break;
                case ConfigurationType.DialogPanel:
                    ShowConfigDialog(item);
                    break;
            }
        }

        private void ShowConfigPanel(ActionUiItem item, DockStyle dockStyle)
        {
            // 显示配置界面的Panel
            //Control configControl = item.ConfigInterface.GetConfigurationControl();
            //configControl.Dock = dockStyle;
            //panelConfig.Controls.Add(configControl);

            //// 显示应用按钮
            //Button applyButton = new Button
            //{
            //    Text = "Apply Configuration",
            //    Dock = DockStyle.Top
            //};
            //applyButton.Click += (sender, e) => ApplyConfiguration(item, configControl);
            //panelConfig.Controls.Add(applyButton);
        }

        private void ShowConfigDialog(ActionUiItem item)
        {
            // 显示配置对话框
            //Control configControl = item.ConfigInterface.GetConfigurationControl();
            //Form configDialog = new Form
            //{
            //    Text = "Configuration Dialog",
            //    Size = configControl.Size,
            //    FormBorderStyle = FormBorderStyle.FixedDialog,
            //    MaximizeBox = false,
            //    MinimizeBox = false
            //};

            //configControl.Dock = DockStyle.Fill;
            //configDialog.Controls.Add(configControl);

            //// 显示应用按钮
            //Button applyButton = new Button
            //{
            //    Text = "Apply Configuration",
            //    Dock = DockStyle.Bottom
            //};
            //applyButton.Click += (sender, e) =>
            //{
            //    ApplyConfiguration(item, configControl);
            //    configDialog.Close();
            //};
            //configDialog.Controls.Add(applyButton);

            //configDialog.ShowDialog();
        }

        private void ApplyConfiguration(ActionUiItem item, Control configControl)
        {
            // 应用配置
            //if (item.ConfigInterface.CheckConfiguration(configControl))
            //{
            //    item.ApplyConfig(configControl);
            //    MessageBox.Show("Configuration applied successfully!");
            //}
            //else
            //{
            //    MessageBox.Show("Configuration check failed. Please review your settings.");
            //}
        }
    }
}
