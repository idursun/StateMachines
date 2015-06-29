namespace StateMachines.Core.Debugging
{
    public class NullDebugger : IDebugger
    {
        public void Step()
        {
        }

        public void Resume()
        {
        }

        public void Break(WorkflowStateData workflowStateData)
        {
            
        }
    }
}