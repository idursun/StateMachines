using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StateMachine.Core
{
    public class MachineNode
    {
        public Guid Guid { get; set; }
    }

    public abstract class StateEvent : MachineNode, IExecutable
    {
        public string Name { get; set; }

        public abstract void Execute(IStateExecutionContext context);
    }

    public class InitializeEvent : StateEvent
    {
        public IExecutable Fired { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            context.Execute(Fired);    
        }
    }
}