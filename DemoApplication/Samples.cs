using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream;
using Mindream.Attributes;

namespace DemoApplication
{
    class Samples
    {
        [StaticMethodComponent]
        public static int Add(int first, int second)
        {
            return first + second;
        }

        [StaticMethodComponent]
        public static void Add1(int first, int second, out int result)
        {
            result = first + second;
        }

        [StaticMethodComponent]
        public static int Add2(int first, out int result2, ref int pSecond)
        {
            result2 = first + pSecond;
            return result2;
        }

        [StaticMethodComponent]
        public static void Add3(int first, ref int second, int result)
        {
            second = first + result + second;
        }
    }
}
