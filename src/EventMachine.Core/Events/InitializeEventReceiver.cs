﻿namespace EventMachine.Core.Events
{
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