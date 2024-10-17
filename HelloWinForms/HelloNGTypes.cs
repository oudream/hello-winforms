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
    /// <summary>
    /// 界面有三个产品，要表示NG与OK，OK使用radioButton，NG有NG类型，目录有三个NG类型（气泡，断胶，溢胶）。
    /// 界面要求，输入三个产品的状态（NG、OK），要求界面相应勾选，如果如果自己勾选，输出三个产品的最终状态。
    /// NG类型（使用tableLayoutPanel，2行2列，3个CheckBox）。NG与OK是互斥的。
    /// </summary>
    public partial class HelloNGTypes : Form
    {
        private CheckBox[] checkBoxes1;
        private CheckBox[] checkBoxes2;
        private string[] fixedNGTypes = { "断胶=BB", "溢胶=BO" };
        private List<KeyValuePair<string, string>> dynamicNGTypes;

        public HelloNGTypes()
        {
            InitializeComponent();

            InitializeDynamicTypes("错位=MIS,渣质=SLAG,胶路不满=BPF,卷边=ROLL");

            InitializeUI();
        }

        private void InitializeDynamicTypes(string dynamicNGTypesString)
        {
            // 动态NG类型初始化（这里可以从配置文件或其他方式获取）
            dynamicNGTypes = dynamicNGTypesString.Split(',')
                .Select(part => part.Split('='))
                .Where(parts => parts.Length == 2)
                .Select(parts => new KeyValuePair<string, string>(parts[0], parts[1]))
                .ToList();
        }

        private void InitializeUI()
        {
            // 设置RadioButton属性
            radioButtonOK1.Text = "OK";
            radioButtonOK1.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            radioButtonOK2.Text = "OK";
            radioButtonOK2.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);

            // 初始化TableLayoutPanel
            InitializeTableLayoutPanel(tableLayoutPanel3, out checkBoxes1);
            InitializeTableLayoutPanel(tableLayoutPanel4, out checkBoxes2);
        }

        private void InitializeTableLayoutPanel(TableLayoutPanel tableLayoutPanel, out CheckBox[] checkBoxes)
        {
            tableLayoutPanel.ColumnCount = 3;
            tableLayoutPanel.RowCount = 3;
            tableLayoutPanel.Size = new System.Drawing.Size(200, 200);

            checkBoxes = new CheckBox[9];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var checkBox = new CheckBox();
                    int index = i * 3 + j;

                    if (index < fixedNGTypes.Length)
                    {
                        var parts = fixedNGTypes[index].Split('=');
                        checkBox.Text = parts[0];
                        checkBox.Tag = parts[1];
                    }
                    else if (index < fixedNGTypes.Length + dynamicNGTypes.Count)
                    {
                        var dynamicNGType = dynamicNGTypes[index - fixedNGTypes.Length];
                        checkBox.Text = dynamicNGType.Key;
                        checkBox.Tag = dynamicNGType.Value;
                    }
                    else
                    {
                        checkBox.Visible = false;
                    }

                    checkBox.CheckedChanged += CheckBox_CheckedChanged;
                    checkBox.Dock = DockStyle.Fill;
                    tableLayoutPanel.Controls.Add(checkBox, j, i);
                    checkBoxes[index] = checkBox;
                }
            }
        }

        private void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = sender as RadioButton;

            if (radioButton == radioButtonOK1 && radioButton.Checked)
            {
                UncheckCheckBoxes(checkBoxes1);
            }
            else if (radioButton == radioButtonOK2 && radioButton.Checked)
            {
                UncheckCheckBoxes(checkBoxes2);
            }
        }

        private void CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;
            if (checkBox.Checked)
            {
                if (Array.Exists(checkBoxes1, cb => cb == checkBox))
                {
                    radioButtonOK1.CheckedChanged -= RadioButton_CheckedChanged;
                    radioButtonOK1.Checked = false;
                    radioButtonOK1.CheckedChanged += RadioButton_CheckedChanged;
                }
                else if (Array.Exists(checkBoxes2, cb => cb == checkBox))
                {
                    radioButtonOK2.CheckedChanged -= RadioButton_CheckedChanged;
                    radioButtonOK2.Checked = false;
                    radioButtonOK2.CheckedChanged += RadioButton_CheckedChanged;
                }
            }
        }

        private void UncheckCheckBoxes(CheckBox[] checkBoxes)
        {
            foreach (var checkBox in checkBoxes)
            {
                if (checkBox != null && checkBox.Checked)
                {
                    checkBox.CheckedChanged -= CheckBox_CheckedChanged;
                    checkBox.Checked = false;
                    checkBox.CheckedChanged += CheckBox_CheckedChanged;
                }
            }
        }

        public string GetCurrentProductStatus(bool isFirstProduct)
        {
            if (isFirstProduct)
            {
                return radioButtonOK1.Checked ? "OK" : GetNGTypes(checkBoxes1);
            }
            else
            {
                return radioButtonOK2.Checked ? "OK" : GetNGTypes(checkBoxes2);
            }
        }

        private string GetNGTypes(CheckBox[] checkBoxes)
        {
            List<string> selectedNGTypes = new List<string>();
            foreach (var checkBox in checkBoxes)
            {
                if (checkBox != null && checkBox.Checked)
                {
                    selectedNGTypes.Add(checkBox.Tag.ToString());
                }
            }
            return string.Join(",", selectedNGTypes);
        }

        public void InitializeProductStatus(bool isFirstProduct, string status)
        {
            if (status == "OK")
            {
                if (isFirstProduct)
                {
                    radioButtonOK1.Checked = true;
                    UncheckCheckBoxes(checkBoxes1);
                }
                else
                {
                    radioButtonOK2.Checked = true;
                    UncheckCheckBoxes(checkBoxes2);
                }
            }
            else
            {
                var ngTypes = status.Split(',');

                if (isFirstProduct)
                {
                    radioButtonOK1.Checked = false;
                    SetNGTypes(checkBoxes1, ngTypes);
                }
                else
                {
                    radioButtonOK2.Checked = false;
                    SetNGTypes(checkBoxes2, ngTypes);
                }
            }
        }

        private void SetNGTypes(CheckBox[] checkBoxes, string[] ngTypes)
        {
            foreach (var checkBox in checkBoxes)
            {
                if (checkBox != null)
                {
                    checkBox.Checked = ngTypes.Contains(checkBox.Tag.ToString());
                }
            }
        }

        private void completeButton_Click(object sender, EventArgs e)
        {

        }

    }


}
