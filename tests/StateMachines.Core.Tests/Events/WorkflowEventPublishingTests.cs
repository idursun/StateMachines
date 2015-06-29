using NUnit.Framework;
using StateMachines.Core.Events;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests.Events
{
    [TestFixture]
    public class WorkflowEventPublishingTests
    {
        private readonly WorkflowBuilder m_builder = new WorkflowBuilder();

        [Test]
        public void Test_PublishEvent()
        {
            var initializer = new InitializeEventReceiver();
            var publisher = new EventPublisher();

            m_builder.Add(initializer);
            m_builder.Add(publisher);

            m_builder.Connect(initializer.Pin(x => x.Fired), publisher);

            var context = m_builder.Compile();
            context.PublishEvent(new WorkflowEventData());

            context.Run();
            Assert.That(publisher.ExecutionCount, Is.EqualTo(1));

            context.Run();
            Assert.That(publisher.ExecutionCount, Is.EqualTo(2));

            context.Run();
            Assert.That(publisher.ExecutionCount, Is.EqualTo(2));
        }

        public class EventPublisher : WorkflowExecutionNode
        {
            public int ExecutionCount { get; set; }

            public override void Execute(IWorkflowExecutionContext context)
            {
                ExecutionCount = ExecutionCount + 1;
                if (ExecutionCount < 2)
                    context.PublishEvent(new WorkflowEventData());
            }
        }
    }

   
}