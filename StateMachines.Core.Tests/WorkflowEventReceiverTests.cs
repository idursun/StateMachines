using NUnit.Framework;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowEventReceiverTests
    {
        [Test]
        public void Test_Handles_Derived_Types()
        {
            var eventSink = new DataReceivedEventReceiver();
            Assert.That(eventSink.Handles(new DataReceivedEventData()), Is.True);
        }

        [Test]
        public void Test_Handles_Null()
        {
            var eventSink = new DataReceivedEventReceiver();
            Assert.That(eventSink.Handles(null), Is.False);
        }
    }

    public class DataReceivedEventReceiver : WorkflowEventReceiver
    {
        [Output]
        public string Data { get; set; }

        public IExecutable Next { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            context.Execute(Next);
        }

        public override void SetEventData(WorkflowEventData eventData)
        {
            DataReceivedEventData dataReceivedEventSink = eventData as DataReceivedEventData;
            if (dataReceivedEventSink != null)
            {
                Data = dataReceivedEventSink.Data;
            }
        }

        public override System.Type EventDataType
        {
            get { return typeof (DataReceivedEventData); }
        }
    }

    public class DataReceivedEventData : WorkflowEventData
    {
        public string Data { get; set; }
    }
}