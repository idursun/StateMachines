using System;

namespace StateMachines.Core
{
    public class MachineNode
    {
        public Guid Guid { get; set; }
    }

    public class StateEventData
    {
        public static readonly StateEventData Empty = new StateEventData();
    }

    public abstract class StateEventReceiver: MachineNode, IExecutable
    {
        public string Name { get; set; }
        public abstract void Execute(IStateExecutionContext context);
        public virtual Type EventDataType { get { return typeof (StateEventData); }}

        public virtual void SetEventData(StateEventData eventData)
        {
            
        }

        public virtual bool Handles(StateEventData eventData)
        {
            if (eventData == null)
                return false;

            return eventData.GetType() == EventDataType;
        }
    }

    public class InitializeEventReceiver : StateEventReceiver
    {
        public IExecutable Fired { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            context.Execute(Fired);  
        }
    }
}