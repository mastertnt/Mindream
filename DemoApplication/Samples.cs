using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mindream;

namespace DemoApplication
{
    class Samples
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
    }
}
