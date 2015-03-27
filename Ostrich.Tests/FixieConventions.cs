namespace Ostrich.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Fixie;

    public class FixieConventions : Convention
    {
        public FixieConventions()
        {
            Classes.NameEndsWith("Tests");
            Methods.Where(method => method.IsVoid() & method.IsPublic);

            ClassExecution.CreateInstancePerClass();

            Parameters.Add(this.FromInlineDataAttributes);
        }

        private IEnumerable<object[]> FromInlineDataAttributes(MethodInfo method)
        {
            var inputAttributes = method.GetCustomAttributes<InputAttribute>(true).ToArray();
            if (!inputAttributes.Any())
            {
                yield return null;
            }
            else 
            {
                foreach (var input in inputAttributes)
                {
                    yield return input.Parameters;
                }
            }
        }

        private object Default(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
