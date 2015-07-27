using System;

namespace EventMachine.Core
{
    public class InvalidWorkflowStateException : Exception
    {
        public InvalidWorkflowStateException(string msg) : base(msg)
        {
            
        }
    }
}