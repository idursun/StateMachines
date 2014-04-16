using Moq;
using NUnit.Framework;
using StateMachines.Core.Logic;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class BranchExecutionTests
    {
        [Test]
        public void Test_Executes_True_Branch()
        {
            var contextMock = new Mock<IStateExecutionContext>();
            var node = new BranchExecutionNode();
            node.Condition = true;
            node.Execute(contextMock.Object);

            contextMock.Verify(x => x.Execute(node.True));
        }
        
        [Test]
        public void Test_Executes_False_Branch()
        {
            var contextMock = new Mock<IStateExecutionContext>();
            var node = new BranchExecutionNode();
            node.Condition = false;
            node.Execute(contextMock.Object);

            contextMock.Verify(x => x.Execute(node.False));
        }
    }
}