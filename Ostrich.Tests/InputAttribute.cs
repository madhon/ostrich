namespace Ostrich.Tests
{
    using System;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class InputAttribute : Attribute
    {
        public InputAttribute(params object[] parameters)
        {
            this.Parameters = parameters;
        }

        public object[] Parameters { get; private set; }
    }
}
