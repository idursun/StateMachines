namespace StateMachines.Core
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