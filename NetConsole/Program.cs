using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            List<int> testList = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 };

            int length = testList.Count - 1;
            int? tmp;
            for (int i = 0; (tmp = testList[i]).HasValue;)
            {
                if (i == 0)
                {
                    testList.RemoveAt(7);
                    testList.RemoveAt(6);
                }
                Console.WriteLine(testList[i]);
                i++;
            }
            Console.ReadLine();

        }
    }
}
