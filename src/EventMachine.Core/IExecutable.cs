namespace EventMachine.Core
{
    public interface IExecutable
    {
        void Execute(IWorkflowExecutionContext context);
    }
}