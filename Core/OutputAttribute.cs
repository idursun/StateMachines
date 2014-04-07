using System;

namespace StateMachine.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class OutputAttribute: Attribute
    {
         
    }
}