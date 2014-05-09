using System;

namespace StateMachines.Core
{
    public struct ContextVariable
    {
        public Guid NodeGuid;
        public string PinName;

        public bool Equals(ContextVariable other)
        {
            return NodeGuid.Equals(other.NodeGuid) && string.Equals(PinName, other.PinName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is ContextVariable && Equals((ContextVariable) obj);
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