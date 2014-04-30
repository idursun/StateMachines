namespace StateMachines.Core
{
    public interface IWorkflowExecutionContext
    {
        void Execute(IExecutable node);
        void EvaluateInputs(WorkflowNode node);
        void PublishEvent(WorkflowEventData workflowEventData);
        void Attach(IDebugger debugger);
        void Run();
        void Resume(WorkflowStateData stateData);
        ExecutionState State { get; }
    }

    public enum ExecutionState
    {
        Idle,
        Executing,
        Resuming,
        Stopped,
    }
}