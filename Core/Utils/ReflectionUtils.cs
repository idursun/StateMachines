using System;
using System.Collections.Generic;
using System.Reflection;

namespace StateMachine.Core.Utils
{
    public static class ReflectionUtils
    {
        public static IEnumerable<Pin> GetPins(this Type type, PinType pinType)
        {
            Type attributeType;
            switch (pinType)
            {
                case PinType.Input:
                    attributeType = typeof(InputAttribute);
                    break;
                case PinType.Output:
                    attributeType = typeof(OutputAttribute);
                    break;
                default:
                    throw new Exception("Pin type has to be either Input or Output");
            }

            PropertyInfo[] propertyInfos = type.GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var customAttributes = propertyInfo.GetCustomAttributes(attributeType, true);
                if (customAttributes.Length > 0)
                    yield return new Pin(propertyInfo)
                    {
                        Name = propertyInfo.Name
                    };
            }
        }
    }
}