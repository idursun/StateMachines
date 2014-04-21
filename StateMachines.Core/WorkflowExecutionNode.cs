namespace StateMachines.Core
{
    public abstract class WorkflowExecutionNode : WorkflowNode, IExecutable
    {
        public abstract void Execute(IWorkflowExecutionContext context);

    }
}