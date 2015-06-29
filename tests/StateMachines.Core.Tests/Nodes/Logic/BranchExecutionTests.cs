using Moq;
using NUnit.Framework;
using StateMachines.Core.Nodes.Logic;

namespace StateMachines.Core.Tests.Nodes.Logic
{
    [TestFixture]
    public class BranchExecutionTests
    {
        [Test]
        public void Test_ExecutesTrueBranch()
        {
            var contextMock = new Mock<IWorkflowExecutionContext>();
            var node = new BranchExecutionNode();
            node.Condition = true;
            node.Execute(contextMock.Object);

            contextMock.Verify(x => x.Execute(node.True));
        }
        
        [Test]
        public void Test_ExecutesFalseBranch()
        {
            var contextMock = new Mock<IWorkflowExecutionContext>();
            var node = new BranchExecutionNode();
            node.Condition = false;
            node.Execute(contextMock.Object);

            contextMock.Verify(x => x.Execute(node.False));
        }
    }
}