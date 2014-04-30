using System;

namespace StateMachines.Core
{
    public class InvalidWorkflowStateException : Exception
    {
        public InvalidWorkflowStateException(string msg) : base(msg)
        {
            
        }
    }
}