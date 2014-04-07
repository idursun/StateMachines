using System;
using System.Collections.Generic;

namespace StateMachine.Core
{
    public class StateMachine
    {

        public StateMachine()
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
            while (node != null)
            {
                EvaluateInputs(node);
                node.Execute();
                node = node.Next;
            }
        }

        private void AssignInputs(MachineNode node)
        {
            var pins = node.GetPins(PinType.Input);
            foreach (var pin in pins)
            {
                IEnumerable<Pin> connectedPins = GetConnectedPins(pin);
                foreach (var connectedPin in connectedPins)
                {
                    string key = MakeCacheKey(connectedPin);
                    if (m_valueCache.ContainsKey(key))
                    {
                        pin.Set(m_valueCache[key]);
                    }
                    else
                    {
                        throw new Exception("Value is not evaluated");
                    }
                }
            }
        }

        private void EvaluateInputs(MachineNode node)
        {
            if (node == null) throw new ArgumentNullException("node");

            var nodeInputPins = node.GetPins(PinType.Input);
            foreach (var inputPin in nodeInputPins)
            {
                var connectedPins = this.GetConnectedPins(inputPin);
                foreach (var connectedPin in connectedPins)
                {
                    EvaluateInputs(connectedPin.Node);
                    ((Function)connectedPin.Node).Evaluate();
                    Remember(MakeCacheKey(connectedPin), connectedPin.Get());
                }
            }

            AssignInputs(node);
        }

        private static string MakeCacheKey(Pin pin)
        {
            if (pin == null) throw new ArgumentNullException("pin");
            return pin.Node.GetType() + "" + pin.Node.Guid.ToString() + "::" + pin.Name;
        }

        private void Remember(string key, object value)
        {
            m_valueCache[key] = value;
        }

        private IEnumerable<Pin> GetConnectedPins(Pin input)
        {
            foreach (var connection in Connections)
            {
                if (connection.Item1 == input)
                    yield return connection.Item2;
                if (connection.Item2 == input)
                    yield return connection.Item1;
            }
        }

        private readonly Dictionary<string, object> m_valueCache = new Dictionary<string, object>();

    }
}