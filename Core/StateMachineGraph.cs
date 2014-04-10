using System;
using System.Collections.Generic;
using System.Linq;

namespace StateMachine.Core
{
    public class StateMachineGraph
    {
        public StateMachineGraph()
        {
            Nodes = new List<MachineNode>();
            Connections = new List<Tuple<Pin, Pin>>();
            FlowConnections = new List<Tuple<Pin, IExecutable>>();
        }

        public List<MachineNode> Nodes { get; private set; }
        public List<Tuple<Pin, Pin>> Connections { get; private set; }
        public List<Tuple<Pin, IExecutable>> FlowConnections { get; private set; }


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

        public void Connect(Pin pin1, IExecutable executable)
        {
            if (pin1 == null) throw new ArgumentNullException("pin1");
            if (executable == null) throw new ArgumentNullException("pin2");
            if (!Nodes.Contains(pin1.Node))
                throw new Exception("node for pin1 is not added");
            if (!Nodes.Contains(executable as MachineNode))
                throw new Exception("node for pin2 is not added");

            FlowConnections.Add(Tuple.Create(pin1, executable));
        }

        public void Compile()
        {
            foreach (var flowConnection in FlowConnections)
            {
                flowConnection.Item1.Set(flowConnection.Item2);
            }
            m_compiled = true;
        }

        public void PublishEvent(StateEvent stateEvent)
        {
            if (!m_compiled)
                throw new Exception("graph is not compiled");

            events.Enqueue(stateEvent);

            Signal();
        }

        public void Signal()
        {
            if (events.Count == 0)
                return;

            StateEvent stateEvent = events.Dequeue();
            var matchingNodes = Nodes.Where(x => x.GetType() == stateEvent.GetType());

            StateExecutionContext context = new StateExecutionContext(this);
            foreach (var matchingNode in matchingNodes)
            {
                context.EvaluateInputs(matchingNode);
                context.Execute(matchingNode as IExecutable);
            }
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

        private Queue<StateEvent>  events = new Queue<StateEvent>();
        private bool m_compiled;
        //private readonly Dictionary<string, object> m_Variables = new Dictionary<string, object>();
    }
}