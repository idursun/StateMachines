using System;

namespace StateMachines.Core
{
    public class WorkflowNode
    {
        public Guid Guid { get; set; }
    }

    public class InitializeEventReceiver : WorkflowEventReceiver
    {
        public IExecutable Fired { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            context.Execute(Fired);  
        }

        public override void SetEventData(WorkflowEventData eventData)
        {
            
        }
    }
}