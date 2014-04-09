using System;
using System.Collections.Generic;

namespace StateMachine.Core
{
    public class ExecutionContext
    {
        public ExecutionContext()
        {
            m_Variables = new Dictionary<string, object>();
        }

        public void Set(Pin pin, object value)
        {
            string key = MakeCacheKey(pin);
            m_Variables[key] = value;
        }

        public object Get(Pin pin)
        {
            if (pin == null) throw new ArgumentNullException("pin");

            string key = MakeCacheKey(pin);
            if (m_Variables.ContainsKey(key))
            {
                return m_Variables[key];
            }
            return null;
        }

        private static string MakeCacheKey(Pin pin)
        {
            if (pin == null)
                throw new ArgumentNullException("pin");
            return pin.Node.GetType() + "" + pin.Node.Guid.ToString() + "::" + pin.Name;
        }

        private readonly Dictionary<string, object> m_Variables;
    }
}