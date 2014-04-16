using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Graph;
using Graph.Compatibility;
using Graph.Items;
using StateMachines.Core;
using StateMachines.Core.Utils;

namespace StateMachines.Designer
{
    public partial class FrmMain : Form
    {
        private Point m_clickedAt;
        private bool dragging;

        public FrmMain()
        {
            InitializeComponent();

            graphControl1.CompatibilityStrategy = new TypeCompatibility();
            graphControl1.ConnectionAdded += GraphControl1OnConnectionAdded;
            graphControl1.HighlightCompatible = true;
            CreateTypeNodes();

            LoadSampleGraph();
            //graphControl1.AddNode()
        }

        private void LoadSampleGraph()
        {
            var initEventSinkNode = CreateNodeFromType(typeof (InitEventSink));
            graphControl1.AddNode(initEventSinkNode);

            var showMessageBoxNode = CreateNodeFromType(typeof (ShowMessageBox));
            graphControl1.AddNode(showMessageBoxNode);

            showMessageBoxNode.Location = new PointF(100, 100);

            var nodeLabelItem = initEventSinkNode.Items.Cast<NodeLabelItem>().First(x => x.Text == "Next");
            var nodeLabelItem2 = showMessageBoxNode.Items.Cast<NodeLabelItem>().First(x => x.Text == "Exec");
            graphControl1.Connect(nodeLabelItem , nodeLabelItem2 );

        }

        private void GraphControl1OnConnectionAdded(object sender, AcceptNodeConnectionEventArgs e)
        {
            e.Connection.Name = " None ";
        }

        private void CreateTypeNodes()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            var nodeType = types.Where(x => typeof(MachineNode).IsAssignableFrom(x)).ToList();
            foreach (var type in nodeType)
            {
                listBox1.Items.Add(new GraphNodeType()
                {
                    Name = type.FullName,
                    NodeType = type
                });

                //var node = CreateNodeFromType(type);
                //graphControl1.AddNode(node);
            }
        }

        private static Node CreateNodeFromType(Type type)
        {
            Node node = new Node(type.FullName);
            node.Tag = type;
            if (typeof (IExecutable).IsAssignableFrom(type))
            {
                NodeLabelItem item = new NodeLabelItem("Exec", true, false);
                item.Input.ConnectorType = NodeConnectorType.Exec;
                node.AddItem(item);
                node.BackColor = Color.Silver;
            }

            if (typeof (StateFunction).IsAssignableFrom(type))
            {
                node.BackColor = Color.LightGreen;
            }
            
            if (typeof (StateEventSink).IsAssignableFrom(type))
            {
                node.BackColor = Color.Salmon;
            }

            //execute pins
            foreach (var pin in type.GetPins(PinType.Execute))
            {
                NodeLabelItem item = new NodeLabelItem(pin.Name, false, true);
                item.Output.ConnectorType = NodeConnectorType.Exec;
                node.AddItem(item);
            }

            foreach (var pin in type.GetPins(PinType.Input))
            {
                NodeLabelItem item = new NodeLabelItem(pin.Name, true, false);
                node.AddItem(item);
            }

            foreach (var pin in type.GetPins(PinType.Output))
            {
                NodeLabelItem nodeLabelItem = new NodeLabelItem(pin.Name, false, true);
                nodeLabelItem.Tag = pin.GetPropertyType();
                node.AddItem(nodeLabelItem);
            }
            return node;
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            m_clickedAt = e.Location;
            dragging = true;
        }

        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            var dragSize = SystemInformation.DragSize;
            if (dragging && Math.Abs(e.X - m_clickedAt.X) > dragSize.Width ||  Math.Abs(e.Y - m_clickedAt.Y) > dragSize.Height)
            {
                GraphNodeType graphNodeType = listBox1.SelectedItem as GraphNodeType;
                if (graphNodeType == null)
                    return;
                var node = CreateNodeFromType(graphNodeType.NodeType);
                m_clickedAt = Point.Empty;
                dragging = false;
                this.DoDragDrop(node, DragDropEffects.Copy);
            }
        }

        private void m_btnCompile_Click(object sender, EventArgs e)
        {
            StateMachineGraph sm = new StateMachineGraph();
            Dictionary<Node, Guid> nodeToGuid = new Dictionary<Node, Guid>();
            foreach (Node node in graphControl1.Nodes)
            {
                Type type = (Type) node.Tag;
                var nodeGuid = Guid.NewGuid();
                nodeToGuid[node] = nodeGuid;
                sm.Nodes.Add(Tuple.Create(nodeGuid, type.AssemblyQualifiedName));
            }

            foreach (NodeConnection connection in graphControl1.Nodes.SelectMany(x => x.Connections))
            {
                var tuple = Tuple.Create(
                    nodeToGuid[connection.From.Node],
                    (connection.From.Item as NodeLabelItem).Text,
                    nodeToGuid[connection.To.Node],
                    (connection.To.Item as NodeLabelItem).Text);

                if (!sm.Connections.Contains(tuple))
                    sm.Connections.Add(tuple);
            }

            var stateMachineGraph = sm.BuildGraph();

            try
            {
                stateMachineGraph.PublishEvent(StateEventData.Empty);
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error:" + exception.Message);
            }
        }
    }

    public class TypeCompatibility : ICompatibilityStrategy
    {
        public bool CanConnect(NodeConnector @from, NodeConnector to)
        {
            if (@from.ConnectorType != to.ConnectorType)
                return false;
            return true;
        }
    }

    internal class GraphNodeType
    {
        public string Name { get; set; }
        public Type NodeType { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
