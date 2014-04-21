using System;
using System.Collections.Generic;
using System.Linq;
using Graph;
using Graph.Items;
using StateMachines.Core;

namespace StateMachines.Designer
{
    public static class GraphControlExtensions
    {
        public static WorkflowGraph ConvertToWorkflowGraph(this GraphControl graphControl)
        {
            WorkflowGraph workflowGraph = new WorkflowGraph();
            var nodeToGuid = new Dictionary<Node, Guid>();
            foreach (Node node in graphControl.Nodes)
            {
                Type type = (Type) node.Tag;
                var nodeGuid = Guid.NewGuid();
                nodeToGuid[node] = nodeGuid;
                workflowGraph.Nodes.Add(Tuple.Create(nodeGuid, type.AssemblyQualifiedName));
            }

            foreach (NodeConnection connection in graphControl.Nodes.SelectMany(x => x.Connections))
            {
                NodeLabelItem fromItem = (connection.From.Item as NodeLabelItem);
                NodeLabelItem toItem = (connection.To.Item as NodeLabelItem);
                if (fromItem == null || toItem == null)
                    continue;

                var tuple = Tuple.Create(
                    nodeToGuid[connection.From.Node], fromItem.Text,
                    nodeToGuid[connection.To.Node], toItem.Text);

                if (!workflowGraph.Connections.Contains(tuple))
                    workflowGraph.Connections.Add(tuple);
            }
            return workflowGraph;
        }
    }
}