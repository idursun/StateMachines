using System;
using System.Collections.Generic;
using System.Linq;
using StateMachines.Core.Utils;

namespace StateMachines.Core
{
    public class WorkflowGraph
    {
        public WorkflowGraph()
        {
            Nodes = new List<Tuple<Guid, string>>();
            Connections = new List<Tuple<Guid, string, Guid, string>>();
        }

        public List<Tuple<Guid, string>> Nodes { get; set; }
        public List<Tuple<Guid, string, Guid, string>> Connections { get; set; }

        public Workflow BuildWorkflow()
        {
            Workflow workflowGraph = new Workflow();
            foreach (var tuple in this.Nodes)
            {
                Guid guid = tuple.Item1;
                Type type = Type.GetType(tuple.Item2, true);

                WorkflowNode workflowNode = Activator.CreateInstance(type) as WorkflowNode;
                if (workflowNode == null) 
                    throw new Exception("type cannot be casted to WorkflowNode");

                workflowNode.Guid = guid;
                workflowGraph.Add(workflowNode);
            }

            foreach (var tuple in this.Connections)
            {
                var node1 = workflowGraph.Nodes.FirstOrDefault(x => x.Guid == tuple.Item1);
                var node2 = workflowGraph.Nodes.FirstOrDefault(x => x.Guid == tuple.Item3);
                if (tuple.Item4 == "Exec")
                {
                    workflowGraph.Connect(node1.Pin(tuple.Item2), node2 as IExecutable);
                }
                else
                {
                    workflowGraph.Connect(node1.Pin(tuple.Item2), node2.Pin(tuple.Item4));
                }
            }
            return workflowGraph;
        }
    }
}