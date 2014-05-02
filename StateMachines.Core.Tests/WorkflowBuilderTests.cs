using System;
using NUnit.Framework;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowBuilderTests
    {
        private WorkflowBuilder m_workflowBuilder;

        [SetUp]
        public void Setup()
        {
            m_workflowBuilder = new WorkflowBuilder();
        }

        [Test]
        public void Test_Connect_Throws_If_Node_Is_Not_Added()
        {
            MakeMessageNode node = new MakeMessageNode();
            Assert.That(delegate
            {
                m_workflowBuilder.Connect(node.Pin(x => x.Input), node.Pin(x => x.Input));
            }, Throws.Exception);
        }

        [Test]
        public void Test_Add_Sets_Guid_If_Empty()
        {
            MakeMessageNode makeMessageNode = new MakeMessageNode();
            m_workflowBuilder.Add(makeMessageNode);

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

            m_workflowBuilder.Add(makeMessageNode1);
            Assert.That(() => m_workflowBuilder.Add(makeMessageNode2), Throws.Exception); 
        }

    }

    public class MakeMessageNode : WorkflowExecutionNode
    {
        [Input]
        public string Input { get; set; }

        [Output]
        public string Output { get; set; }

        public override void Execute(IWorkflowExecutionContext context)
        {
            if (Input == null)
                throw new Exception("Input value was not set");

            Output = Input + " !!!";
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
}