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

        private readonly Random rnd = new Random();

        public RandomIntegerFunction()
        {
            Min = 0;
            Max = 20;
        }

        public override void Evaluate()
        {
            Result = rnd.Next(Min, Max);
        }
    }

    public class RandomStringGenerator: StateFunction
    {

        [Input]
        public int Length { get; set; }

        [Output]
        public string Generated { get; set; }

        public RandomStringGenerator()
        {
            Length = 10;
        }

        private Random m_random = new Random();

        public override void Evaluate()
        {
            string name = "";
            for (int i = 0; i < Length; i++)
            {
                name += (char)(65 + m_random.Next(26));
            }
            Generated = name;
        }
    }

    public class ConcatFunction : StateFunction
    {
        [Input]
        public string First { get; set; }
        [Input]
        public string Second { get; set; }

        [Output]
        public string Output { get; set; }

        public override void Evaluate()
        {
            Output = First + " " + Second;
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
            context.Execute(Next);
        }
    }

    public class InitEventSink : StateEventSink
    {
        public IExecutable Next { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            context.Execute(Next);    
        }
    }
}