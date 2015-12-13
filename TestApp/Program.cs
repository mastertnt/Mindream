using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Mindream;

namespace TestApp
{
    class Program
    {
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            var member = expression.Body as MethodCallExpression;

            if (member != null)
                return member.Method;

            throw new ArgumentException("Expression is not a method", "expression");
        }

        public void Test()
        {
            
        }

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="pArgs">The arguments.</param>
        static void Main(string[] pArgs)
        {
            MethodInfo lMethod = GetMethodInfo(() => Math.Sin(0));

            StaticMethodComponentDescriptor lDescriptor = new StaticMethodComponentDescriptor(lMethod);
            Console.WriteLine(lDescriptor.ToString());

            Mindream.IComponent lComponent = lDescriptor.Create();
            lComponent["a"] = 0.5;
            lComponent.Start();
        }
    }
}
