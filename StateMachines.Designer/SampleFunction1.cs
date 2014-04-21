using System;
using System.Windows.Forms;
using StateMachines.Core;

namespace StateMachines.Designer
{
    public class SampleFunction1 : WorkflowFunction
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

    public class RandomIntegerFunction : WorkflowFunction
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

    public class RandomStringGenerator: WorkflowFunction
    {
        [Input]
        public int Length { get; set; }

        [Output]
        public string Generated { get; set; }

        public RandomStringGenerator()
        {
            Length = 10;
        }

        private readonly Random m_random = new Random();

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

    public class ConcatFunction : WorkflowFunction
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

    public class ShowMessageBox : WorkflowExecutionNode
    {
        [Input]
        public string Message { get; set; }

        public IExecutable Next { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            MessageBox.Show(Message);
            context.Execute(Next);
        }
    }

    public class InitEventReceiver : WorkflowEventReceiver
    {
        public IExecutable Next { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            context.Execute(Next);    
        }

        public override void SetEventData(WorkflowEventData eventData)
        {
            
        }
    }
}