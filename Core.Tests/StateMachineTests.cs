using System;
using Moq;
using NUnit.Framework;
using StateMachine.Core;
using StateMachine.Core.Utils;

namespace StateMachine.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        private Core.StateMachine m_stateMachine;
        [SetUp]
        public void Setup()
        {
            m_stateMachine = new Core.StateMachine();
        }

        [Test]
        public void Test_Throws_WhenNoRootNodeIsSet()
        {
            Assert.That(m_stateMachine.Signal, Throws.Exception);
        }

        [Test]
        public void Test_Connect_Throws_IfPinIsNull()
        {
            MakeMessageNode node = new MakeMessageNode();
            Assert.That(delegate
            {
                m_stateMachine.Connect(node.Pin(x => x.Input), null);
            }, Throws.InstanceOf<ArgumentNullException>());
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
        public void Test_Add_Sets_Guid()
        {
            MakeMessageNode makeMessageNode = new MakeMessageNode();
            m_stateMachine.Add(makeMessageNode);

            Assert.That(makeMessageNode.Guid, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public void Test_Signal_SignalsRootNode()
        {
            var executionNode = new Mock<ExecutionNode>();

            m_stateMachine.Add(executionNode.Object);
            m_stateMachine.RootNode = executionNode.Object;

            m_stateMachine.Signal();

            executionNode.Verify(x => x.Execute());
        }

        [Test]
        public void Test_Signal_ProvidesInputValues()
        {
            var executionNode = new MakeMessageNode();
            var concatFunction = new ConcatFunction();
            var getMessage1Function = new GetMessageFunction("Hello");
            var getMessage2Function = new GetMessageFunction("World");
            m_stateMachine.Add(concatFunction);
            m_stateMachine.Add(getMessage1Function);
            m_stateMachine.Add(getMessage2Function);

            m_stateMachine.Add(executionNode);
            m_stateMachine.RootNode = executionNode;
            m_stateMachine.Connect(concatFunction.Pin(x => x.First), getMessage1Function.Pin(x => x.Message));
            m_stateMachine.Connect(concatFunction.Pin(x => x.Second), getMessage2Function.Pin(x => x.Message));
            m_stateMachine.Connect(executionNode.Pin(x => x.Input), concatFunction.Pin(x => x.Output));

            m_stateMachine.Signal();
            Assert.That(executionNode.Output, Is.EqualTo("Hello World !!!"));
        }

    }

    public class GetMessageFunction: Function
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

        public override void Execute()
        {
            if (Input == null)
                throw new Exception("Input value was not set");

            Output = Input + " !!!";
        }
    }

    public class ConcatFunction : Function
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