namespace StateMachine.Core
{
    public abstract class ExecutionNode : MachineNode
    {
        public abstract void Execute();
    }
}