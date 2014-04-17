using System;
using NUnit.Framework;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        private StateMachine m_stateMachine;

        [SetUp]
        public void Setup()
        {
            m_stateMachine = new StateMachine();
        }

        [Test]
        public void Test_Connect_Throws_If_Node_Is_Not_Added()
        {
            MakeMessageNode node = new MakeMessageNode();
            Assert.That(delegate
            {
                m_stateMachine.Connect(node.Pin(x => x.Input), node.Pin(x => x.Input));
            }, Throws.Exception);
        }

        [Test]
        public void Test_Add_Sets_Guid_If_Empty()
        {
            MakeMessageNode makeMessageNode = new MakeMessageNode();
            m_stateMachine.Add(makeMessageNode);

            Assert.That(makeMessageNode.Guid, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void Test_Add_ChecksGuidForDuplicate()
        {
            MakeMessageNode makeMessageNode1 = new MakeMessageNode();
            MakeMessageNode makeMessageNode2 = new MakeMessageNode();
            var newGuid = Guid.NewGuid();
            makeMessageNode1.Guid = newGuid;
            makeMessageNode2.Guid = newGuid;

            m_stateMachine.Add(makeMessageNode1);
            Assert.That(() => m_stateMachine.Add(makeMessageNode2), Throws.Exception); 
        }

        [Test]
        public void Test_Signal_ProvidesInputValues()
        {
            var initEvent = new InitializeEventReceiver();
            var executionNode = new MakeMessageNode();
            var concatFunction = new ConcatFunction();
            var getMessage1Function = new GetMessageFunction("Hello");
            var getMessage2Function = new GetMessageFunction("World");

            m_stateMachine.Add(initEvent);
            m_stateMachine.Add(concatFunction);
            m_stateMachine.Add(getMessage1Function);
            m_stateMachine.Add(getMessage2Function);
            m_stateMachine.Add(executionNode);

            m_stateMachine.Connect(concatFunction.Pin(x => x.First), getMessage1Function.Pin(x => x.Message));
            m_stateMachine.Connect(concatFunction.Pin(x => x.Second), getMessage2Function.Pin(x => x.Message));
            m_stateMachine.Connect(executionNode.Pin(x => x.Input), concatFunction.Pin(x => x.Output));
            m_stateMachine.Connect(initEvent.Pin(x => x.Fired), executionNode);

            m_stateMachine.Compile();
            m_stateMachine.PublishEvent(StateEventData.Empty);

            Assert.That(executionNode.Output, Is.EqualTo("Hello World !!!"), "Message is not correct");
        }
    }

    public class GetMessageFunction: StateFunction
    {
        private readonly string m_initialMessage;

        public GetMessageFunction(string initialMessage)
        {
            m_initialMessage = initialMessage;
        }

        [Output]
        public string Message { get; set; }

        public override void Evaluate()
        {
            Message = m_initialMessage;
        }
    }

    public class MakeMessageNode : ExecutionNode
    {
        [Input]
        public string Input { get; set; }

        [Output]
        public string Output { get; set; }

        public override void Execute(IStateExecutionContext context)
        {
            if (Input == null)
                throw new Exception("Input value was not set");

            Output = Input + " !!!";
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
}