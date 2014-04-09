using System;
using System.Collections.Generic;
using StateMachine.Core.Utils;

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
            ExecutionContext context = new ExecutionContext();
            while (node != null)
            {
                EvaluateInputs(node, context);
                node.Execute();
                //node = node.Next;
            }
        }

        private void EvaluateInputs(MachineNode node, ExecutionContext context)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            var nodeInputPins = node.GetPins(PinType.Input);
            foreach (var inputPin in nodeInputPins)
            {
                var connectedPins = this.GetConnectedPins(inputPin);
                foreach (var connectedPin in connectedPins)
                {
                    EvaluateInputs(connectedPin.Node, context);
                    ((Function)connectedPin.Node).Evaluate();
                    context.Set(connectedPin, connectedPin.GetValue());
                }
            }

            AssignInputs(node, context);
        }

        private void AssignInputs(MachineNode node, ExecutionContext context)
        {
            var pins = node.GetPins(PinType.Input);
            foreach (var pin in pins)
            {
                IEnumerable<Pin> connectedPins = GetConnectedPins(pin);
                foreach (var connectedPin in connectedPins)
                {
                    object o = context.Get(connectedPin);
                    pin.Set(o);
                }
            }
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

        //private readonly Dictionary<string, object> m_Variables = new Dictionary<string, object>();
    }
}