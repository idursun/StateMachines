using System;
using System.Collections.Generic;

namespace StateMachines.Core
{
    public class WorkflowStateData
    {
        public Dictionary<ContextVariable, object> Variables { get; set; }
        public IExecutable ExecutingNode { get; set; }
    }
}