namespace EventMachine.Core.Nodes.Consts
{
    public class ConstValueFunction<T> : WorkflowFunction
    {
        private readonly T m_value;

        public ConstValueFunction(T value)
        {
            m_value = value;
        }

        [Output]
        public T Value { get; set; }

        public override void Evaluate()
        {
            Value = m_value;
        }
    }
}