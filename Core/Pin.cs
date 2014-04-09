using System;
using System.Diagnostics;
using System.Reflection;

namespace StateMachine.Core
{
    [DebuggerDisplay("{Node}::{Name}")]
    public class Pin 
    {
        public Pin(PropertyInfo propertyInfo)
        {
            m_propertyInfo = propertyInfo;
        }

        public MachineNode Node { get; set; }
        public string Name { get; set; }

        protected bool Equals(Pin other)
        {
            return Equals(Node, other.Node) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Pin) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Node != null ? Node.GetHashCode() : 0)*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Pin left, Pin right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Pin left, Pin right)
        {
            return !Equals(left, right);
        }

        public object GetValue()
        {
            object result = m_propertyInfo.GetValue(Node, null);
            return result;
        }

        public void Set(object value)
        {
            m_propertyInfo.SetValue(Node, value, null);
        }

        public Type GetPropertyType()
        {
            return m_propertyInfo.PropertyType;
        }

        private readonly PropertyInfo m_propertyInfo;
    }

    public enum PinType
    {
        None,
        Input,
        Output
    }
}