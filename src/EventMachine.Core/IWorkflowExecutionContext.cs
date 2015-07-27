using System;
using EventMachine.Core.Debugging;
using EventMachine.Core.Events;

namespace EventMachine.Core
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