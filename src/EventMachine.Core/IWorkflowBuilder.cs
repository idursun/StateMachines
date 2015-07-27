using System;

namespace EventMachine.Core
{
    public interface IWorkflowBuilder
    {
        IWorkflowBuilder Connect<T, R>(Func<WorkflowNode, T> pin1, Func<WorkflowNode, R> pin2);
        IWorkflowBuilder Connect(Func<WorkflowNode, IExecutable> pin1, WorkflowExecutionNode node);
        IWorkflowExecutionContext Compile();
    }
}