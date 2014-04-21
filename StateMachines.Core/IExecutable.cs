namespace StateMachines.Core
{
    public interface IExecutable
    {
        void Execute(IWorkflowExecutionContext context);
    }
}