﻿using System;
using System.Collections.Generic;

namespace StateMachines.Core
{
    public class WorkflowStateData
    {
        public Dictionary<string, object> Variables { get; set; }
        public Guid ExecutingNodeGuid { get; set; }
    }
}