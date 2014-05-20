using System;

namespace StateMachines.Core
{
    public struct ContextVariableKey
    {
        public Guid NodeGuid;
        public string PinName;

        public bool Equals(ContextVariableKey other)
        {
            return NodeGuid.Equals(other.NodeGuid) && string.Equals(PinName, other.PinName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ContextVariableKey && Equals((ContextVariableKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (NodeGuid.GetHashCode()*397) ^ (PinName != null ? PinName.GetHashCode() : 0);
            }
        }
    }
}