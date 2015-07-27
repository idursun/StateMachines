namespace EventMachine.Core.Debugging
{
    public interface IDebugger
    {
        void Step();
        void Resume();
        void Break(WorkflowStateData workflowStateData);
    }
}