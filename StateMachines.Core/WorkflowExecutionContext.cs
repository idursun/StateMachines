using System;
using System.Collections.Generic;
using StateMachines.Core.Utils;

namespace StateMachines.Core
{
    public class WorkflowExecutionContext : IWorkflowExecutionContext
    {
        public WorkflowExecutionContext(Workflow machine)
        {
            m_machine = machine;
            m_Variables = new Dictionary<string, object>();
        }

        public void Execute(IExecutable node)
        {
            if (node == null)
                return;

            EvaluateInputs(node as WorkflowNode);
            node.Execute(this);
        }

        protected void Set(Pin pin, object value)
        {
            string key = MakeCacheKey(pin);
            m_Variables[key] = value;
        }

        protected object Get(Pin pin)
        {
            if (pin == null) throw new ArgumentNullException("pin");

            string key = MakeCacheKey(pin);
            if (m_Variables.ContainsKey(key))
            {
                return m_Variables[key];
            }
            return null;
        }

        public void EvaluateInputs(WorkflowNode node)
        {
            if (node == null)
                return;

            var nodeInputPins = node.GetPins(PinType.Input);
            foreach (var inputPin in nodeInputPins)
            {
                IEnumerable<Pin> connectedPins = m_machine.GetConnectedPins(inputPin);
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
                IEnumerable<Pin> connectedPins = m_machine.GetConnectedPins(pin);
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

        private readonly Dictionary<string, object> m_Variables;

        private readonly Workflow m_machine;
    }
}