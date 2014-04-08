using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Graph;
using Graph.Compatibility;
using Graph.Items;
using StateMachine.Core;
using StateMachine.Core.Utils;

namespace StateMachine.Designer
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            graphControl1.CompatibilityStrategy = new AlwaysCompatible();
            graphControl1.ConnectionAdding += GraphControl1OnConnectionAdding;
            graphControl1.ConnectionAdded += GraphControl1OnConnectionAdded;
            CreateTypeNodes();
        }

        private void GraphControl1OnConnectionAdded(object sender, AcceptNodeConnectionEventArgs e)
        {
            e.Connection.Name = " None ";
        }

        private void GraphControl1OnConnectionAdding(object sender, AcceptNodeConnectionEventArgs acceptNodeConnectionEventArgs)
        {
            acceptNodeConnectionEventArgs.Cancel = false;
        }

        private void CreateTypeNodes()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            var enumerable = types.Where(x => typeof(MachineNode).IsAssignableFrom(x)).ToList();
            foreach (var type in enumerable)
            {
                Node node = new Node(type.FullName);
                foreach (var pin in type.GetPins(PinType.Input))
                {
                    Type propertyType = pin.GetPropertyType();
                    NodeItem item;
                    if (propertyType == typeof (String))
                    {
                        NodeTextBoxItem nodeLabelItem = new NodeTextBoxItem(pin.Name, true, false);
                        item = nodeLabelItem;
                    }
                    else
                    {
                        NodeLabelItem nodeLabelItem = new NodeLabelItem(pin.Name, true, false);
                        item = nodeLabelItem;
                    }
                    item.Tag = propertyType;
                    node.AddItem(item);
                }

                foreach (var pin in type.GetPins(PinType.Output))
                {
                    NodeLabelItem nodeLabelItem = new NodeLabelItem(pin.Name, false, true);
                    nodeLabelItem.Tag = pin.GetPropertyType();
                    node.AddItem(nodeLabelItem);
                }

                graphControl1.AddNode(node);
            }
        }
    }
}
