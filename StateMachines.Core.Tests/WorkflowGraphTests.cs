using System;
using NUnit.Framework;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class WorkflowGraphTests
    {
        [Test]
        public void Test_BuildWorkflow_Converts_all_connections_and_nodes()
        {
            WorkflowGraph workflowGraph = new WorkflowGraph();
            Guid node1 = Guid.NewGuid();
            Guid node2 = Guid.NewGuid();

            workflowGraph.AddNode(node1, typeof(SimpleNode).AssemblyQualifiedName);
            workflowGraph.AddNode(node2, typeof(SimpleNode).AssemblyQualifiedName);
            workflowGraph.AddConnection(node1, "Next", node2, "Exec");

            WorkflowBuilder workflowBuilder = workflowGraph.BuildWorkflow();

            Assert.That(workflowBuilder.Nodes.Count, Is.EqualTo(2));
            Assert.That(workflowBuilder.Connections.Count, Is.EqualTo(0));
            Assert.That(workflowBuilder.FlowConnections.Count, Is.EqualTo(1));
        } 
        
        [Test]
        public void Test_BuildWorkflow_Throws_if_type_is_wrong()
        {
            WorkflowGraph workflowGraph = new WorkflowGraph();
            workflowGraph.AddNode(Guid.NewGuid(), typeof (SimpleNode).ToString());

            Assert.That(() => workflowGraph.BuildWorkflow(), Throws.InstanceOf<TypeLoadException>());
        }

        public class SimpleNode :  WorkflowExecutionNode
        {
            public IExecutable Next { get; set; }
            public override void Execute(IWorkflowExecutionContext context)
            {
                context.Execute(Next);
            }
        }
    }
}