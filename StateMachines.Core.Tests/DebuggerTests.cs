using Moq;
using NUnit.Framework;
using StateMachines.Core.Utils;

namespace StateMachines.Core.Tests
{
    [TestFixture]
    public class DebuggerTests
    {
        private WorkflowBuilder m_workflowBuilder;
        private SimpleNode m_simpleNode1;
        private SimpleNode m_simpleNode2;
        private SimpleNode m_simpleNode3;

        [SetUp]
        public void Setup()
        {
            m_workflowBuilder = new WorkflowBuilder();
            InitializeEventReceiver receiver = new InitializeEventReceiver();
            m_simpleNode1 = new SimpleNode();
            m_simpleNode2 = new SimpleNode();
            m_simpleNode3 = new SimpleNode();
            m_workflowBuilder.Add(receiver);
            m_workflowBuilder.Add(m_simpleNode1);
            m_workflowBuilder.Add(m_simpleNode2);
            m_workflowBuilder.Add(m_simpleNode3);

            m_workflowBuilder.Connect(receiver.Pin(x => x.Fired), m_simpleNode1);
            m_workflowBuilder.Connect(m_simpleNode1.Pin(x => x.Next), m_simpleNode2);
            m_workflowBuilder.Connect(m_simpleNode2.Pin(x => x.Next), m_simpleNode3);
        }

        [Test]
        public void Test_SetBreakpoint_StopsAtTheBreakpoint()
        {
            var executionContext = m_workflowBuilder.Compile();
            Mock<IDebugger> debuggerMock = new Mock<IDebugger>();

            executionContext.Attach(debuggerMock.Object);
            executionContext.SetBreakpoint(m_simpleNode2.Guid);

            executionContext.PublishEvent(new WorkflowEventData());

            debuggerMock.Verify(x => x.Break(It.IsAny<WorkflowStateData>()));
        }

        [Test]
        public void Test_SetBreakpoint_Resumes()
        {
            var executionContext = m_workflowBuilder.Compile();
            Mock<IDebugger> debuggerMock = new Mock<IDebugger>();
            WorkflowStateData stateData = null;
            debuggerMock.Setup(x => x.Break(It.IsAny<WorkflowStateData>())).Callback(delegate(WorkflowStateData sd)
            {
                stateData = sd;
            });

            executionContext.Attach(debuggerMock.Object);
            executionContext.SetBreakpoint(m_simpleNode2.Guid);
            executionContext.PublishEvent(new WorkflowEventData());

            Assert.IsFalse(m_simpleNode3.IsCalled);

            executionContext.Resume(stateData);

            Assert.IsTrue(m_simpleNode3.IsCalled);
        }

        public class SimpleNode : WorkflowExecutionNode
        {
            public IExecutable Next { get; set; }
            public bool IsCalled { get; set; }
            public override void Execute(IWorkflowExecutionContext context)
            {
                IsCalled = true;
                context.Execute(Next);
            }
        }
    }
}