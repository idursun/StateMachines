namespace StateMachines.Core
{
    public interface IExecutable
    {
        void Execute(IStateExecutionContext context);
    }

    public abstract class ExecutionNode : MachineNode, IExecutable
    {
        public abstract void Execute(IStateExecutionContext context);

    }
}