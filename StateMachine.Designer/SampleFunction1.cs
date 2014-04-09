using System;
using System.Windows.Forms;
using StateMachine.Core;

namespace StateMachine.Designer
{
    public class SampleFunction1 : StateFunction
    {
        [Input]
        public string First { get; set; }
        [Input]
        public string Second { get; set; }
        [Output]
        public string Result { get; set; }

        public override void Evaluate()
        {
            Result = First + " " + Second;
        }
    }  
    
    public class RandomIntegerFunction : StateFunction
    {
        [Input]
        public int Min { get; set; }
        [Input]
        public int Max { get; set; }

        [Output]
        public int Result { get; set; }

        readonly Random rnd = new Random();

        public override void Evaluate()
        {
            Result = rnd.Next(Min, Max);
        }
    }

    public class ShowMessageBox : ExecutionNode
    {
        [Input]
        public string Message { get; set; }

        public IExecutable Next { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            MessageBox.Show(Message);
        }
    }
}