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
                case PinType.Execute:
                    IEnumerable<PropertyInfo> infos = type.GetProperties().Where(x => x.PropertyType == typeof (IExecutable));
                    foreach (var propertyInfo in infos)
                    {
                        yield return new Pin(propertyInfo) { Name = propertyInfo.Name};
                    }
                    yield break;
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
            Type type1 = node.GetType();
            foreach (var pin in GetPins(type1, pinType))
            {
                pin.Node = node;
                yield return pin;
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

        //public static Pin Flow<T>(this T node, Expression<Func<T, IExecutable>> expr)
        //    where T: IExecutable
        //{
        //    if (expr.NodeType != ExpressionType.Lambda)
        //        throw new Exception("Has to be lambda.");

        //    MemberExpression member = (expr.Body as MemberExpression);
        //    if (member == null)
        //        throw new Exception("has to be a property expression");

        //    var propertyName = member.Member.Name;
        //    var propertyInfo = typeof(T).GetProperties().FirstOrDefault(x => x.Name == propertyName);
        //    if (propertyInfo == null)
        //        throw new Exception(string.Format("Property '{0}' is not found", propertyName));

        //    return new Pin(propertyInfo)
        //    {
        //        Node = node as MachineNode,
        //        Name = propertyInfo.Name
        //    };
        //}
    }
}