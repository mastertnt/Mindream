using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using Mindream;

namespace TestApp
{
    class Program
    {
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
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
            ComponentMethod lComponentMethod = new ComponentMethod(GetMethodInfo<Program>(x => x.Test()));
        }
    }
}
