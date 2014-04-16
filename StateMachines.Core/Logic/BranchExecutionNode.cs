namespace StateMachines.Core.Logic
{
    public class BranchExecutionNode : ExecutionNode
    {
        [Input]
        public bool Condition { get; set; }

        public IExecutable True { get; set; }
        public IExecutable False { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            if (Condition)
            {
                context.Execute(True);
            }
            else
            {
                context.Execute(False);
            } 
        }
    }
}