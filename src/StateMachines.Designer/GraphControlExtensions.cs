using System;
using System.Collections.Generic;
using System.Linq;
using Graph;
using Graph.Items;
using StateMachines.Core;
using StateMachines.Core.Utils;

namespace StateMachines.Designer
{
    public static class GraphControlExtensions
    {
        public static IWorkflowExecutionContext Compile(this GraphControl graphControl)
        {
            WorkflowBuilder builder = new WorkflowBuilder();
            var nodeToGuid = new Dictionary<Node, Guid>();
            var nodes = new Dictionary<Guid, WorkflowNode>();
            foreach (Node graphNode in graphControl.Nodes)
            {
                Type type = (Type)graphNode.Tag;
                var nodeGuid = Guid.NewGuid();
                nodeToGuid[graphNode] = nodeGuid;
                WorkflowNode workflowNode = Activator.CreateInstance(type) as WorkflowNode;
                if (workflowNode != null)
                {
                    workflowNode.Guid = nodeGuid;
                    builder.Add(workflowNode);
                    nodes[nodeGuid] = workflowNode;
                }
            }

            foreach (NodeConnection connection in graphControl.Nodes.SelectMany(x => x.Connections))
            {
                NodeLabelItem fromItem = (connection.From.Item as NodeLabelItem);
                NodeLabelItem toItem = (connection.To.Item as NodeLabelItem);
                if (fromItem == null || toItem == null)
                    continue;

                Guid fromNode = nodeToGuid[connection.From.Node];
                Guid toNode = nodeToGuid[connection.To.Node];
                if (toItem.Text == "Exec")
                {
                    builder.Connect(nodes[fromNode].Pin(fromItem.Text), nodes[toNode] as IExecutable);
                }
                else
                {
                    builder.Connect(nodes[fromNode].Pin(fromItem.Text), nodes[toNode].Pin(toItem.Text));
                }
            }
            return builder.Compile();
        }
    }
}