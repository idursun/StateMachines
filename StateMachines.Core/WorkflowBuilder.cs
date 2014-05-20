using System;
using System.Collections.Generic;
using System.Linq;
using StateMachines.Core.Events;

namespace StateMachines.Core
{
    public class WorkflowBuilder
    {
        public WorkflowBuilder()
        {
            Nodes = new List<WorkflowNode>();
            Connections = new List<Tuple<Pin, Pin>>();
            FlowConnections = new List<Tuple<Pin, IExecutable>>();
        }

        public List<WorkflowNode> Nodes { get; private set; }
        public List<Tuple<Pin, Pin>> Connections { get; private set; }
        public List<Tuple<Pin, IExecutable>> FlowConnections { get; private set; }

        public void Add(WorkflowNode node)
        {
            if (node.Guid == Guid.Empty)
                node.Guid = Guid.NewGuid();

            if (Nodes.Any(x => x.Guid == node.Guid))
                throw new Exception("a node with this guid already exists");

            Nodes.Add(node);
        }

        public void Connect(Pin pin1, Pin pin2)
        {
            if (pin1 == null) throw new ArgumentNullException("pin1");
            if (pin2 == null) throw new ArgumentNullException("pin2");
            if (!Nodes.Contains(pin1.Node))
                throw new Exception("node for pin1 is not added");
            if (!Nodes.Contains(pin2.Node))
                throw new Exception("node for pin2 is not added");

            Connections.Add(Tuple.Create(pin1, pin2));
        }

        public void Connect(Pin pin1, IExecutable executable)
        {
            if (pin1 == null) throw new ArgumentNullException("pin1");
            if (executable == null) throw new ArgumentNullException("executable");
            if (!Nodes.Contains(pin1.Node))
                throw new Exception("node for pin1 is not added");
            if (!Nodes.Contains(executable as WorkflowNode))
                throw new Exception("node for pin2 is not added");

            FlowConnections.Add(Tuple.Create(pin1, executable));
        }

        public IWorkflowExecutionContext Compile()
        {
            foreach (var flowConnection in FlowConnections)
            {
                flowConnection.Item1.Set(flowConnection.Item2);
            }
            return new WorkflowExecutionContext(new WorkflowGraphMemo(Nodes.ToArray(),Connections.ToArray()));
        }

        /// <summary>
        /// Preserves the state of the graph for context to work for further modifications
        /// </summary>
        class WorkflowGraphMemo : IWorkflowGraph
        {
            private readonly WorkflowNode[] m_workflowNodes;
            private readonly Tuple<Pin, Pin>[] m_connections;

            protected internal WorkflowGraphMemo(WorkflowNode[] workflowNodes, Tuple<Pin, Pin>[] connections)
            {
                m_workflowNodes = workflowNodes;
                m_connections = connections;
            }

            public IEnumerable<Pin> GetConnectedPins(Pin input)
            {
                foreach (var connection in m_connections)
                {
                    if (connection.Item1 == input)
                        yield return connection.Item2;
                    if (connection.Item2 == input)
                        yield return connection.Item1;
                }
            }

            public IEnumerable<WorkflowEventReceiver> EventSinkNodes()
            {
                return m_workflowNodes.Where(x => x is WorkflowEventReceiver).Cast<WorkflowEventReceiver>().ToList();
            }
        }
    }
}