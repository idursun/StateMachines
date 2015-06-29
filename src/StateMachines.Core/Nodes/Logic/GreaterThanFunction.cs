using System;
using System.Collections;

namespace StateMachines.Core.Nodes.Logic
{
    public class GreaterThanFunction : WorkflowFunction
    {
        [Input]
        public object A { get; set; } 

        [Input]
        public object B { get; set; }

        [Output]
        public bool Result { get; set; }

        public override void Evaluate()
        {
            Result = Comparer.Default.Compare(A, B) > 0;
        }
    }
}