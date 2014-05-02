using System;
using System.Collections.Generic;

namespace StateMachines.Core
{
    public interface IWorkflowGraph
    {
        IEnumerable<Pin> GetConnectedPins(Pin input);
        IEnumerable<WorkflowEventReceiver> EventSinksNodes();
        WorkflowNode FindNodeByGUID(Guid nodeGuid);
    }
}