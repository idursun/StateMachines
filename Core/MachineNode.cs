using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace StateMachine.Core
{
    public class MachineNode
    {
        public Guid Guid { get; set; }

        public Pin Pin<T>(Expression<Func<T, object>> expr)
        {
            if (expr.NodeType != ExpressionType.Lambda)
                throw new Exception("Has to be lambda.");

            MemberExpression member = (expr.Body as MemberExpression);
            if (member == null)
                throw new Exception("has to be a property expression");

            var propertyName = member.Member.Name;
            var propertyInfo = typeof (T).GetProperties().FirstOrDefault(x => x.Name == propertyName);
            if (propertyInfo == null)
                throw new Exception(string.Format("Property '{0}' is not found", propertyName));

            return new Pin(propertyInfo)
            {
                Node = this,
                Name= propertyName
            };
        }

        public IEnumerable<Pin> GetPins(PinType pinType)
        {
            Type type;
            switch (pinType)
            {
                case PinType.Input:
                    type = typeof (InputAttribute);
                    break;
                case PinType.Output:
                    type = typeof (OutputAttribute);
                    break;
                default:
                    throw new Exception("Pin type has to be either Input or Output");
            }

            PropertyInfo[] propertyInfos = this.GetType().GetProperties();
            foreach (var propertyInfo in propertyInfos)
            {
                var customAttributes =  propertyInfo.GetCustomAttributes(type, true);
                if (customAttributes.Length > 0)
                    yield return new Pin(propertyInfo)
                    {
                        Name = propertyInfo.Name,
                        Node = this
                    };
            }
        }
    }
}