using System;

namespace StateMachines.Core.Events
{
    public abstract class WorkflowEventReceiver: WorkflowNode, IExecutable
    {
        public string Name { get; set; }
        public virtual Type EventDataType { get { return typeof (WorkflowEventData); }}

        public abstract void Execute(IWorkflowExecutionContext context);
        public abstract void SetEventData(WorkflowEventData eventData);

        public virtual bool Handles(WorkflowEventData eventData)
        {
            if (eventData == null)
                return false;

            return eventData.GetType() == EventDataType;
        }
    }
}