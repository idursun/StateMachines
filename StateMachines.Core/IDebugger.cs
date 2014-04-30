using System;

namespace StateMachines.Core
{
    public interface IDebugger
    {
        void Step();
        void Resume();
        void Break(WorkflowStateData workflowStateData);
    }
}