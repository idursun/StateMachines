using System;
using System.Collections.Generic;
using System.Linq;
using StateMachines.Core.Utils;

namespace StateMachines.Core
{
    public class WorkflowExecutionContext : IWorkflowExecutionContext
    {
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
                if (State == ExecutionState.Executing && m_debugger.IsHit(workflowNode.Guid))
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

            m_variables.Clear();
            m_variables = new Dictionary<string, object>(stateData.Variables);
            WorkflowNode workflowNode = m_workflow.Nodes.FirstOrDefault(x => x.Guid == stateData.ExecutingNodeGuid);
            if (workflowNode == null)
            {
                throw new Exception("Invalid workflow state");
            }
            
            EvaluateInputs(workflowNode);

            IExecutable node = workflowNode as IExecutable;
            if (node == null)
            {
                throw new Exception("Invalid workflow state");
            }
            State = ExecutionState.Resuming;
            Execute(node);
        }

        public ExecutionState State { get; private set; }

        public void EvaluateInputs(WorkflowNode node)
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

        private static string MakeCacheKey(Pin pin)
        {
            if (pin == null)
                throw new ArgumentNullException("pin");
            return pin.Node.GetType() + "" + pin.Node.Guid.ToString() + "::" + pin.Name;
        }

        private readonly Queue<WorkflowEventData> events = new Queue<WorkflowEventData>();
        private readonly Workflow m_workflow;
        private IDebugger m_debugger;
        private Dictionary<string, object> m_variables;


        public void Attach(IDebugger debugger)
        {
            m_debugger = debugger;
        }
    }

    public class NullDebugger : IDebugger
    {
        public bool IsHit(Guid nodeGuid)
        {
            return false;
        }

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