using Moq;
using NUnit.Framework;
using StateMachines.Core.Debugging;
using StateMachines.Core.Events;
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
        private Mock<IDebugger> m_debuggerMock;

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

            m_debuggerMock = new Mock<IDebugger>();
        }

        [Test]
        public void Test_SetBreakpoint_StopsAtTheBreakpoint()
        {
            var executionContext = m_workflowBuilder.Compile();

            executionContext.Attach(m_debuggerMock.Object);
            executionContext.SetBreakpoint(m_simpleNode2);

            executionContext.PublishEvent(new WorkflowEventData());

            m_debuggerMock.Verify(x => x.Break(It.IsAny<WorkflowStateData>()));
        }

        [Test]
        public void Test_SetBreakpoint_Resumes()
        {
            var executionContext = m_workflowBuilder.Compile();
            WorkflowStateData stateData = null;
            m_debuggerMock.Setup(x => x.Break(It.IsAny<WorkflowStateData>())).Callback(delegate(WorkflowStateData sd)
            {
                stateData = sd;
            });

            executionContext.Attach(m_debuggerMock.Object);
            executionContext.SetBreakpoint(m_simpleNode2);
            executionContext.PublishEvent(new WorkflowEventData());

            Assert.IsFalse(m_simpleNode3.IsCalled);

            executionContext.Resume(stateData);

            Assert.IsTrue(m_simpleNode3.IsCalled);
        } 
        
        [Test]
        public void Test_RemoveBreakpoint_Removes()
        {
            var executionContext = m_workflowBuilder.Compile();

            executionContext.Attach(m_debuggerMock.Object);
            executionContext.SetBreakpoint(m_simpleNode2);
            executionContext.RemoveBreakpoint(m_simpleNode2);

            executionContext.PublishEvent(new WorkflowEventData());

            m_debuggerMock.Verify(x => x.Break(It.IsAny<WorkflowStateData>()), Times.Never);
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