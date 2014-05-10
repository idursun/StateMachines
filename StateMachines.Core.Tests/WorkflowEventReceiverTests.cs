using Moq;
using NUnit.Framework;
using StateMachines.Core.Events;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowEventReceiverTests
    {
        [Test]
        public void Test_HandlesDerivedTypes()
        {
            var eventSink = new DataReceivedEventReceiver();
            Assert.That(eventSink.Handles(new DataReceivedEventData()), Is.True);
        }

        [Test]
        public void Test_HandlesNull()
        {
            var eventSink = new DataReceivedEventReceiver();
            Assert.That(eventSink.Handles(null), Is.False);
        }

        [Test]
        public void Test_SetEventDataIsCalled()
        {
            WorkflowBuilder builder = new WorkflowBuilder();
            DataReceivedEventReceiver eventReceiver = new DataReceivedEventReceiver();

            builder.Add(eventReceiver);

            var context = builder.Compile();
            
            context.PublishEvent(new DataReceivedEventData() { Data = "data"});
            context.Run();

            Assert.That(eventReceiver.Data, Is.EqualTo("data"));
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