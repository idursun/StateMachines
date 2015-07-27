﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace EventMachine.Core.Utils
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
            where T: WorkflowNode
        {
            Type type1 = node.GetType();
            foreach (var pin in GetPins(type1, pinType))
            {
                pin.Node = node;
                yield return pin;
            }
        }

        public static Pin Pin<T,R>(this T node, Expression<Func<T, R>> expr)
            where T: WorkflowNode
        {

            if (expr.NodeType != ExpressionType.Lambda)
                throw new Exception("Has to be lambda.");

            string propertyName;
            MemberExpression body = expr.Body as MemberExpression;
            if (body != null)
            {
                propertyName = body.Member.Name;
            }
            else
                throw new Exception("has to be a property expression");

            return Pin(node, propertyName);
        }

        public static Pin Pin<T>(this T node, string propertyName)
            where T: WorkflowNode
        {
            var properties = node.GetType().GetProperties();
            var propertyInfo = properties.FirstOrDefault(x => x.Name == propertyName);
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