using System;
using System.Collections.Generic;
using EventMachine.Core.Events;

namespace EventMachine.Core
{
    public interface IWorkflowGraph
    {
        IEnumerable<Pin> GetConnectedPins(Pin input);
        IEnumerable<WorkflowEventReceiver> EventSinkNodes();
    }
}