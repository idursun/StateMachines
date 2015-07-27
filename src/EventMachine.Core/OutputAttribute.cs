using System;

namespace EventMachine.Core
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class OutputAttribute: Attribute
    {
         
    }
}