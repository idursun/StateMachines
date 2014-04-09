using System;
using System.Collections.Generic;

namespace StateMachine.Core
{
    public class StateMachineGraph
    {
        public StateMachineGraph()
        {
            Nodes = new List<MachineNode>();
            Connections = new List<Tuple<Pin, Pin>>();
        }

        public List<MachineNode> Nodes { get; private set; }
        public List<Tuple<Pin, Pin>> Connections { get; private set; }
        public ExecutionNode RootNode { get; set; }

        public void Add(MachineNode node)
        {
            if (node.Guid == Guid.Empty)
                node.Guid = Guid.NewGuid();

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


        public void Signal()
        {
            if (RootNode == null)
                throw new Exception("No root node.");

            ExecutionNode node = RootNode;
            StateExecutionContext context = new StateExecutionContext(this);
            context.EvaluateInputs(node);
            context.Execute(node);
        }


        public IEnumerable<Pin> GetConnectedPins(Pin input)
        {
            foreach (var connection in Connections)
            {
                if (connection.Item1 == input)
                    yield return connection.Item2;
                if (connection.Item2 == input)
                    yield return connection.Item1;
            }
        }

        //private readonly Dictionary<string, object> m_Variables = new Dictionary<string, object>();
    }
}