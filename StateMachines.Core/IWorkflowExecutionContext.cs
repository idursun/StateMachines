namespace StateMachines.Core
{
    public interface IWorkflowExecutionContext
    {
        void Execute(IExecutable node);
        void EvaluateInputs(WorkflowNode node);
    }
}