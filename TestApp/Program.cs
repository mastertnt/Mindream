using System;
using System.Linq;
using System.Reflection;
using Mindream;

namespace TestApp
{
    public class Program
    {
        [StaticMethodComponent]
        public static int Add(int pFirst, int pSecond)
        {
            return pFirst + pSecond;
        }

        [StaticMethodComponent]
        public static void Add1(int pFirst, int pSecond, out int pResult)
        {
            pResult = pFirst + pSecond;
        }

        [StaticMethodComponent]
        public static int Add2(int pFirst, out int pResult, ref int pSecond)
        {
            pResult = pFirst + pSecond;
            return pResult;
        }

        [StaticMethodComponent]
        public static void Add3(int pFirst, ref int pSecond, int pResult)
        {
            pSecond = pFirst + pResult + pSecond;
        }

        [StaticMethodComponent]
        public static bool Branch(bool condition)
        {
            return condition;
        }

        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="pArgs">The arguments.</param>
        static void Main(string[] pArgs)
        {
            ComponentDescriptorRegistry lComponentDescriptorRegistry = new ComponentDescriptorRegistry();
            lComponentDescriptorRegistry.FindAllDescriptors(Assembly.GetExecutingAssembly());

            foreach (var lDescriptor in lComponentDescriptorRegistry.Descriptors)
            {
                Console.WriteLine(lDescriptor.ToString());
            }

            IComponentDescriptor lRetrievedDescriptor = lComponentDescriptorRegistry.Descriptors.FirstOrDefault(pDescriptor => pDescriptor.Name == "Add2");
            IComponent lComponent = lRetrievedDescriptor.Create();
            lComponent["pFirst"] = 42;
            lComponent["pSecond"] = 42;
            lComponent["pResult"] = 10;
            lComponent.Start();
        }
    }
}
