namespace EventMachine.Core
{
    public interface IWorkflowBuilder
    {
        IWorkflowBuilder Connect(Pin pin1, Pin pin2);
        IWorkflowBuilder Connect(Pin pin1, IExecutable executable);
        IWorkflowExecutionContext Compile();
    }
}