namespace StateMachines.Core.Logic
{
    public class BranchExecutionNode : WorkflowExecutionNode
    {
        [Input]
        public bool Condition { get; set; }

        public IExecutable True { get; set; }
        public IExecutable False { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            if (Condition)
            {
                context.Execute(True);
            }
            else
            {
                context.Execute(False);
            } 
        }
    }
}