using System;
using System.Collections.Generic;
using System.Linq;
using StateMachines.Core.Debugging;
using StateMachines.Core.Events;
using StateMachines.Core.Utils;

namespace StateMachines.Core
{
    public class WorkflowExecutionContext : IWorkflowExecutionContext
    {
        private readonly Queue<WorkflowEventData> events = new Queue<WorkflowEventData>();
        private readonly HashSet<IExecutable> m_breakpoints = new HashSet<IExecutable>();
        private readonly IWorkflowGraph m_workflowGraph;
        private Dictionary<ContextVariableKey, object> m_variables;
        private IDebugger m_debugger;

        protected internal WorkflowExecutionContext(IWorkflowGraph graph)
        {
            m_workflowGraph = graph;
            m_variables = new Dictionary<ContextVariableKey, object>();
            m_debugger = new NullDebugger();
        }

        public ExecutionState State { get; private set; }

        public void Execute(IExecutable node)
        {
            if (node == null)
                return;

            WorkflowNode workflowNode = node as WorkflowNode;
            if (workflowNode != null)
            {
                EvaluateInputs(workflowNode);
            }

            if (State == ExecutionState.Executing && IsBreakpointHit(node))
            {
                State = ExecutionState.Debugging;
                m_debugger.Break(new WorkflowStateData()
                {
                    ExecutingNode = node,
                    Variables = m_variables
                });
                return;
            }

            State = ExecutionState.Executing;
            node.Execute(this);
        }

        public void PublishEvent<TEvent>(TEvent workflowEventData) 
            where TEvent: WorkflowEventData
        {
            events.Enqueue(workflowEventData);
        }

        public void Run()
        {
            if (events.Count == 0)
                return;

            WorkflowEventData workflowEventData = events.Dequeue();

            IEnumerable<WorkflowEventReceiver> matchingEventSinks = m_workflowGraph.EventSinkNodes().Where(x => x.Handles(workflowEventData)).ToList();

            State = ExecutionState.Executing;
            //first set data for every sink
            foreach (var matchingNode in matchingEventSinks)
            {
                matchingNode.SetEventData(workflowEventData);
            }

            // and then execute each node
            foreach (var matchingNode in matchingEventSinks)
            {
                EvaluateInputs(matchingNode);
                Execute(matchingNode);
            }
        }

        public void Resume(WorkflowStateData stateData)
        {
            if (stateData == null) 
                throw new ArgumentNullException("stateData");

            if (stateData.ExecutingNode == null)
                throw new InvalidWorkflowStateException("Executing Node is null");
                
            m_variables.Clear();
            m_variables = new Dictionary<ContextVariableKey, object>(stateData.Variables ?? new Dictionary<ContextVariableKey, object>());

            State = ExecutionState.Resuming;
            Execute(stateData.ExecutingNode);
        }

        public void Attach(IDebugger debugger)
        {
            m_debugger = debugger;
        }

        public void SetBreakpoint(IExecutable executable)
        {
            m_breakpoints.Add(executable);
        }

        public void RemoveBreakpoint(IExecutable executable)
        {
            m_breakpoints.Remove(executable);
        }

        protected void Set(Pin pin, object value)
        {
            ContextVariableKey key = MakeContextVariableKey(pin);
            m_variables[key] = value;
        }

        protected object Get(Pin pin)
        {
            if (pin == null) throw new ArgumentNullException("pin");

            ContextVariableKey key = MakeContextVariableKey(pin);
            if (m_variables.ContainsKey(key))
            {
                return m_variables[key];
            }
            return null;
        }

        private void EvaluateInputs(WorkflowNode node)
        {
            if (node == null)
                return;

            var nodeInputPins = node.GetPins(PinType.Input);
            foreach (var inputPin in nodeInputPins)
            {
                IEnumerable<Pin> connectedPins = m_workflowGraph.GetConnectedPins(inputPin);
                foreach (Pin connectedPin in connectedPins)
                {
                    EvaluateInputs(connectedPin.Node);
                    WorkflowFunction function = connectedPin.Node as WorkflowFunction;
                    if (function != null)
                        function.Evaluate();

                    this.Set(connectedPin, connectedPin.GetValue());
                }
            }

            AssignInputs(node);
        }

        private void AssignInputs(WorkflowNode node)
        {
            var pins = node.GetPins(PinType.Input);
            foreach (var pin in pins)
            {
                IEnumerable<Pin> connectedPins = m_workflowGraph.GetConnectedPins(pin);
                foreach (var connectedPin in connectedPins)
                {
                    object o = this.Get(connectedPin);
                    pin.Set(o);
                }
            }
        }

        private bool IsBreakpointHit(IExecutable executable)
        {
            return m_breakpoints.Contains(executable);
        }

        private static ContextVariableKey MakeContextVariableKey(Pin pin)
        {
            if (pin == null)
                throw new ArgumentNullException("pin");

            return new ContextVariableKey()
            {
                NodeGuid = pin.Node.Guid,
                PinName = pin.Name
            };
        }
    }
}