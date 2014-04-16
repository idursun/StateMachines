using System;
using System.Collections.Generic;
using System.Linq;
using StateMachines.Core.Utils;

namespace StateMachines.Core
{
    public class StateMachineGraph
    {
        public StateMachineGraph()
        {
            Nodes = new List<Tuple<Guid, string>>();
            Connections = new List<Tuple<Guid, string, Guid, string>>();
        }
        public List<Tuple<Guid, string>> Nodes { get; set; }
        public List<Tuple<Guid, string, Guid, string>> Connections { get; set; }

        public StateMachine BuildGraph()
        {
            StateMachine stateMachineGraph = new StateMachine();
            foreach (var tuple in this.Nodes)
            {
                Guid guid = tuple.Item1;
                Type type = Type.GetType(tuple.Item2, true);

                MachineNode machineNode = Activator.CreateInstance(type) as MachineNode;
                if (machineNode == null) 
                    throw new Exception("type cannot be casted to MachineNode");

                machineNode.Guid = guid;
                stateMachineGraph.Add(machineNode);
            }

            foreach (var tuple in this.Connections)
            {
                var node1 = stateMachineGraph.Nodes.FirstOrDefault(x => x.Guid == tuple.Item1);
                var node2 = stateMachineGraph.Nodes.FirstOrDefault(x => x.Guid == tuple.Item3);
                if (tuple.Item4 == "Exec")
                {
                    stateMachineGraph.Connect(node1.Pin(tuple.Item2), node2 as IExecutable);
                }
                else
                {
                    stateMachineGraph.Connect(node1.Pin(tuple.Item2), node2.Pin(tuple.Item4));
                }
            }

            stateMachineGraph.Compile();
            return stateMachineGraph;
        }
    }
}