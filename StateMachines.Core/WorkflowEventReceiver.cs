using System;

namespace StateMachines.Core
{
    public abstract class WorkflowEventReceiver: WorkflowNode, IExecutable
    {
        public string Name { get; set; }
        public abstract void Execute(IWorkflowExecutionContext context);
        public virtual Type EventDataType { get { return typeof (WorkflowEventData); }}

        public abstract void SetEventData(WorkflowEventData eventData);

        public virtual bool Handles(WorkflowEventData eventData)
        {
            if (eventData == null)
                return false;

            return eventData.GetType() == EventDataType;
        }
    }
}