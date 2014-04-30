using System;

namespace StateMachines.Core
{
    public interface IDebugger
    {
        bool IsHit(Guid nodeGuid);
        void Step();
        void Resume();
        void Break(WorkflowStateData workflowStateData);
    }
}