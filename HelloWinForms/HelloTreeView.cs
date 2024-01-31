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
    public partial class HelloTreeView : Form
    {
        Dictionary<ObjectType, string> objectTypeToChinese = new Dictionary<ObjectType, string>
        {
            { ObjectType.Project, "项目" },
            { ObjectType.Coroutine, "协程" },
            { ObjectType.Process, "工序" },
            { ObjectType.StateMachine, "状态机" },
            { ObjectType.DecisionOperator, "判定算子" },
            { ObjectType.Equipment, "设备" },
            { ObjectType.Variable, "变量" },
            { ObjectType.DriverModule, "驱动模块" }
        };


        Dictionary<ObjectType, List<ObjectType>> childNodeMap = new Dictionary<ObjectType, List<ObjectType>>
        {
            { ObjectType.Project, new List<ObjectType> { ObjectType.Coroutine } },
            { ObjectType.Coroutine, new List<ObjectType> { ObjectType.Process } },
            { ObjectType.Process, new List<ObjectType> { ObjectType.StateMachine } },
            // 第一层的特殊情况
            { ObjectType.Equipment, new List<ObjectType> { ObjectType.Equipment } },
            { ObjectType.Variable, new List<ObjectType> { ObjectType.Variable } },
            { ObjectType.DriverModule, new List<ObjectType> { ObjectType.DriverModule } },
        };

        public HelloTreeView()
        {
            InitializeComponent();

            InitializeTreeView();

            InitializeContextMenu();
        }

        private void InitializeTreeView()
        {
            treeView1.Nodes.Add(CreateTreeNode(ObjectType.Project));
            treeView1.Nodes.Add(CreateTreeNode(ObjectType.Equipment));
            treeView1.Nodes.Add(CreateTreeNode(ObjectType.Variable));
            treeView1.Nodes.Add(CreateTreeNode(ObjectType.DriverModule));
        }

        private TreeNode CreateTreeNode(ObjectType type)
        {
            TreeNode node = new TreeNode(objectTypeToChinese[type]);
            node.Tag = type; // 将枚举值存储在Tag属性中，方便后续使用
            return node;
        }


        // 示例: 获取子节点的方法（需要根据实际情况来实现）
        private List<TreeNodeObject> GetChildren(ObjectType type)
        {
            // 这里应该是一些逻辑来加载每种类型的具体实例。
            // 返回 TreeNodeObject 的列表
            return new List<TreeNodeObject>();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var selectedNode = e.Node.Tag as TreeNodeObject;
            if (selectedNode != null)
            {
                // 根据 selectedNode 显示信息或执行操作
            }
        }



        private void InitializeContextMenu()
        {
            foreach (ObjectType type in Enum.GetValues(typeof(ObjectType)))
            {
                ToolStripItem menuItem = contextMenuStrip1.Items.Add("添加 " + type.ToString());
                menuItem.Tag = type;
                menuItem.Click += new EventHandler(OnAddNodeClick);
            }

            treeView1.ContextMenuStrip = contextMenuStrip1;
        }

        private void OnAddNodeClick(object sender, EventArgs e)
        {
            if (sender is ToolStripItem menuItem && menuItem.Tag is ObjectType)
            {
                ObjectType typeToAdd = (ObjectType)menuItem.Tag;
                AddNode(typeToAdd);
            }
        }

        private void AddNode(ObjectType typeToAdd)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                TreeNode newNode = new TreeNode("新 " + typeToAdd.ToString());
                newNode.Tag = new TreeNodeObject(typeToAdd, "新节点名");
                selectedNode.Nodes.Add(newNode);
                selectedNode.Expand(); // 展开父节点以显示新节点
            }
        }

        private void treeView1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point clickPoint = new System.Drawing.Point(e.X, e.Y);
                TreeNode clickedNode = treeView1.GetNodeAt(clickPoint);
                if (clickedNode != null)
                {
                    treeView1.SelectedNode = clickedNode;
                    contextMenuStrip1.Items.Clear(); // 清除旧菜单项

                    // 根据节点的层级和类型调整菜单
                    AddContextMenuItemsBasedOnNode(clickedNode);
                }
            }
        }

        private void AddContextMenuItemsBasedOnNode(TreeNode node)
        {
            List<ObjectType> childTypes = new List<ObjectType>();

            // 检查节点层级和类型，确定可以添加的子节点类型
            if (node.Level == 0) // 第一层节点
            {
                ObjectType nodeType = (ObjectType)node.Tag;
                switch (nodeType)
                {
                    case ObjectType.Project:
                        childTypes.Add(ObjectType.Coroutine);
                        break;
                    case ObjectType.Equipment:
                        childTypes.Add(ObjectType.Equipment);
                        break;
                    case ObjectType.Variable:
                        childTypes.Add(ObjectType.Variable);
                        break;
                    case ObjectType.DriverModule:
                        childTypes.Add(ObjectType.DriverModule);
                        break;
                }
            }
            else if (node.Tag is TreeNodeObject nodeObject) // 非第一层节点
            {
                switch (nodeObject.Type)
                {
                    case ObjectType.Coroutine:
                        childTypes.Add(ObjectType.Process);
                        break;
                    case ObjectType.Process:
                        childTypes.Add(ObjectType.StateMachine);
                        break;
                        // 其他类型不添加子节点
                }
            }

            // 根据确定的子节点类型添加菜单项
            foreach (ObjectType childType in childTypes)
            {
                ToolStripItem menuItem = contextMenuStrip1.Items.Add("添加 " + objectTypeToChinese[childType]);
                menuItem.Tag = childType;
                menuItem.Click += new EventHandler(OnAddNodeClick);
            }
        }


        private void AddChildNodeTypes(ObjectType nodeType)
        {
            List<ObjectType> childTypes = new List<ObjectType>();
            if (nodeType == ObjectType.Project)
            {
                childTypes.Add(ObjectType.Coroutine);
            }
            else if (nodeType == ObjectType.Coroutine)
            {
                childTypes.Add(ObjectType.Process);
            }
            else if (nodeType == ObjectType.Process)
            {
                childTypes.Add(ObjectType.StateMachine);
            }
            else if (nodeType == ObjectType.Equipment || nodeType == ObjectType.Variable || nodeType == ObjectType.DriverModule)
            {
                childTypes.Add(nodeType);
            }

            foreach (ObjectType childType in childTypes)
            {
                ToolStripItem menuItem = contextMenuStrip1.Items.Add("添加 " + objectTypeToChinese[childType]);
                menuItem.Tag = childType;
                menuItem.Click += new EventHandler(OnAddNodeClick);
            }
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

    }
    public class TreeNodeObject
    {
        public ObjectType Type { get; set; }
        public string Name { get; set; }
        public List<TreeNodeObject> Children { get; set; }

        public TreeNodeObject(ObjectType type, string name)
        {
            Type = type;
            Name = name;
            Children = new List<TreeNodeObject>();
        }

        // 添加更多的逻辑，比如如何加载子项等。
    }


    public enum ObjectType
    {
        // 项目、协程、工序、状态机、判定算子、设备、变量、驱动模块
        Project, Coroutine, Process, StateMachine, DecisionOperator, Equipment, Variable, DriverModule
    }

    // 第一层的 Equipment, Variable, DriverModule 都能创建自己类型，Project 能创建 Coroutine；
    // 第二层就只有就以下关系 Coroutine 能创建 Process , Process 能创建 StateMachine 
}
