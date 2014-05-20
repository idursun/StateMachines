using System;
using StateMachines.Core.Debugging;
using StateMachines.Core.Events;

namespace StateMachines.Core
{
    public interface IWorkflowExecutionContext
    {
        ExecutionState State { get; }

        void Execute(IExecutable node);
        void Run();
        void Resume(WorkflowStateData stateData);
        void PublishEvent<TEvent>(TEvent workflowEventData) where TEvent : WorkflowEventData;

        void Attach(IDebugger debugger);
        void SetBreakpoint(IExecutable executable);
        void RemoveBreakpoint(IExecutable executable);
    }

    public enum ExecutionState
    {
        Idle,
        Executing,
        Resuming,
        Debugging,
    }
}