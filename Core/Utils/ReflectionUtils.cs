using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public static IEnumerable<Pin> GetPins<T>(this T node, PinType pinType)
            where T: MachineNode
        {
            Type type;
            switch (pinType)
            {
                case PinType.Input:
                    type = typeof(InputAttribute);
                    break;
                case PinType.Output:
                    type = typeof(OutputAttribute);
                    break;
                default:
                    throw new Exception("Pin type has to be either Input or Output");
            }

            PropertyInfo[] propertyInfos = typeof(T).GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var customAttributes = propertyInfo.GetCustomAttributes(type, true);
                if (customAttributes.Length > 0)
                    yield return new Pin(propertyInfo)
                    {
                        Name = propertyInfo.Name,
                        Node = node
                    };
            }
        }

        public static Pin Pin<T>(this T node, Expression<Func<T, object>> expr)
            where T: MachineNode
        {
            if (expr.NodeType != ExpressionType.Lambda)
                throw new Exception("Has to be lambda.");

            MemberExpression member = (expr.Body as MemberExpression);
            if (member == null)
                throw new Exception("has to be a property expression");

            var propertyName = member.Member.Name;
            var propertyInfo = typeof(T).GetProperties().FirstOrDefault(x => x.Name == propertyName);
            if (propertyInfo == null)
                throw new Exception(string.Format("Property '{0}' is not found", propertyName));

            return new Pin(propertyInfo)
            {
                Node = node,
                Name = propertyName
            };
        }


    }
}