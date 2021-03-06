﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Graph;
using Graph.Compatibility;
using Graph.Items;
using EventMachine.Core;
using EventMachine.Core.Events;
using EventMachine.Core.Utils;

namespace EventMachine.Designer
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
            var initEventSinkNode = CreateNodeFromType(typeof (InitEventReceiver));
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
            List<Type> types = new List<Type>();
            types.AddRange(Assembly.GetExecutingAssembly().GetTypes());
            types.AddRange(typeof (WorkflowNode).Assembly.GetTypes().Where(x => x.Namespace.Contains("Nodes."))); // sadece nodes altindakileri alsin simdilik.

            var nodeType = types.Where(x => typeof(WorkflowNode).IsAssignableFrom(x)).ToList();
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

            if (typeof (WorkflowFunction).IsAssignableFrom(type))
            {
                node.BackColor = Color.LightGreen;
            }
            
            if (typeof (WorkflowEventReceiver).IsAssignableFrom(type))
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
            try
            {
                var executionContext = graphControl1.Compile();
                executionContext.PublishEvent(WorkflowEventData.Start);
                executionContext.Run();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Error:" + exception.Message);
            }
        }

        private void graphControl1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                IEnumerable<Node> nodes = graphControl1.Nodes.Where(x => x.ElementType == ElementType.NodeSelection);
                graphControl1.RemoveNodes(nodes);
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
