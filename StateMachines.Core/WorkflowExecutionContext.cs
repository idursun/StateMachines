using System;
using System.Collections.Generic;
using System.Linq;
using StateMachines.Core.Utils;

namespace StateMachines.Core
{
    public class WorkflowExecutionContext : IWorkflowExecutionContext
    {
        public ExecutionState State { get; private set; }

        protected internal WorkflowExecutionContext(Workflow workflow)
        {
            m_workflow = workflow;
            m_variables = new Dictionary<string, object>();
            m_debugger = new NullDebugger();
        }

        public void Execute(IExecutable node)
        {
            if (node == null)
                return;

            WorkflowNode workflowNode = node as WorkflowNode;
            if (workflowNode != null)
            {
                EvaluateInputs(workflowNode);
                if (State == ExecutionState.Executing && IsHit(workflowNode.Guid))
                {
                    m_debugger.Break(new WorkflowStateData()
                    {
                        ExecutingNodeGuid = workflowNode.Guid,
                        Variables = m_variables
                    });
                    return;
                }
            }

            State = ExecutionState.Executing;
            node.Execute(this);
        }

        public void PublishEvent(WorkflowEventData workflowEventData)
        {
            events.Enqueue(workflowEventData);

            Run();
        }

        public void Run()
        {
            if (events.Count == 0)
                return;

            WorkflowEventData workflowEventData = events.Dequeue();

            IEnumerable<WorkflowEventReceiver> matchingEventSinks = m_workflow.EventSinksNodes().Where(x => x.Handles(workflowEventData)).ToList();

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

            WorkflowNode workflowNode = m_workflow.Nodes.FirstOrDefault(x => x.Guid == stateData.ExecutingNodeGuid);
            if (workflowNode == null)
                throw new InvalidWorkflowStateException(string.Format("Node with guid {0} does not exist", stateData.ExecutingNodeGuid));

            m_variables.Clear();
            m_variables = new Dictionary<string, object>(stateData.Variables ?? new Dictionary<string, object>());

            EvaluateInputs(workflowNode);

            IExecutable node = workflowNode as IExecutable;
            if (node == null)
                throw new InvalidWorkflowStateException("Node type is not invalid");

            State = ExecutionState.Resuming;
            Execute(node);
        }

        public void Attach(IDebugger debugger)
        {
            m_debugger = debugger;
        }

        public void SetBreakpoint(Guid nodeGuid)
        {
            m_breakpoints.Add(nodeGuid);
        }

        public void RemoveBreakpoint(Guid nodeGuid)
        {
            m_breakpoints.Remove(nodeGuid);
        }

        private Dictionary<string, object> m_variables;


        private readonly HashSet<Guid> m_breakpoints = new HashSet<Guid>();

        private void EvaluateInputs(WorkflowNode node)
        {
            if (node == null)
                return;

            var nodeInputPins = node.GetPins(PinType.Input);
            foreach (var inputPin in nodeInputPins)
            {
                IEnumerable<Pin> connectedPins = m_workflow.GetConnectedPins(inputPin);
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

        protected void Set(Pin pin, object value)
        {
            string key = MakeCacheKey(pin);
            m_variables[key] = value;
        }

        protected object Get(Pin pin)
        {
            if (pin == null) throw new ArgumentNullException("pin");

            string key = MakeCacheKey(pin);
            if (m_variables.ContainsKey(key))
            {
                return m_variables[key];
            }
            return null;
        }

        private void AssignInputs(WorkflowNode node)
        {
            var pins = node.GetPins(PinType.Input);
            foreach (var pin in pins)
            {
                IEnumerable<Pin> connectedPins = m_workflow.GetConnectedPins(pin);
                foreach (var connectedPin in connectedPins)
                {
                    object o = this.Get(connectedPin);
                    pin.Set(o);
                }
            }
        }

        private bool IsHit(Guid guid)
        {
            return m_breakpoints.Contains(guid);
        }

        private static string MakeCacheKey(Pin pin)
        {
            if (pin == null)
                throw new ArgumentNullException("pin");
            return pin.Node.GetType() + "" + pin.Node.Guid.ToString() + "::" + pin.Name;
        }

        private readonly Queue<WorkflowEventData> events = new Queue<WorkflowEventData>();
        private readonly Workflow m_workflow;
        private IDebugger m_debugger;
    }

    public class NullDebugger : IDebugger
    {
        public void Step()
        {
        }

        public void Resume()
        {
        }

        public void Break(WorkflowStateData workflowStateData)
        {
            
        }
    }
}