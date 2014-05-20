using System;
using System.Collections.Generic;
using StateMachines.Core.Events;

namespace StateMachines.Core
{
    public interface IWorkflowGraph
    {
        IEnumerable<Pin> GetConnectedPins(Pin input);
        IEnumerable<WorkflowEventReceiver> EventSinkNodes();
    }
}