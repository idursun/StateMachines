using NUnit.Framework;
using StateMachine.Core;
using StateMachine.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class EventSinkTests
    {

        private StateMachine.Core.StateMachine m_stateMachine;

        [SetUp]
        public void Setup()
        {
            m_stateMachine = new StateMachine.Core.StateMachine();
        }

        [Test]
        public void Test_HandlesEventFromADerivedType()
        {
            var eventSink1 = new DataReceivedEventSink();
            var eventSink2 = new DataReceivedEventSink();
            var executionNode = new MakeMessageNode();
            var concatFunction = new ConcatFunction();

            m_stateMachine.Add(eventSink1);
            m_stateMachine.Add(eventSink2);
            m_stateMachine.Add(concatFunction);
            m_stateMachine.Add(executionNode);

            m_stateMachine.Connect(eventSink1.Pin(x => x.Data), concatFunction.Pin(x => x.First));
            m_stateMachine.Connect(eventSink2.Pin(x => x.Data), concatFunction.Pin(x => x.Second));
            m_stateMachine.Connect(concatFunction.Pin(x => x.Output), executionNode.Pin(x => x.Input));
            m_stateMachine.Connect(eventSink1.Pin(x => x.Next), executionNode);

            m_stateMachine.Compile();
            m_stateMachine.PublishEvent(new DataReceivedStateEventData()
            {
                Data = "DATA"
            });

            Assert.That(executionNode.Output, Is.EqualTo("DATA DATA !!!"), "Message was not correct");
        }
    }

    public class DataReceivedEventSink : StateEventSink
    {
        [Output]
        public string Data { get; set; }

        public IExecutable Next { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            context.Execute(Next);
        }

        public override void SetEventData(StateEventData eventData)
        {
            DataReceivedStateEventData dataReceivedEventSink = eventData as DataReceivedStateEventData;
            if (dataReceivedEventSink != null)
            {
                Data = dataReceivedEventSink.Data;
            }
        }

        public override System.Type EventDataType
        {
            get { return typeof (DataReceivedStateEventData); }
        }
    }

    public class DataReceivedStateEventData : StateEventData
    {
        public string Data { get; set; }
    }
}