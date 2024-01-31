using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HelloRoslyn
{
    public static class UtilityClass
    {
        public static int Add(int a, int b)
        {
            // 示例操作：简单的循环累加
            int sum = 0;
            for (int i = 0; i < 1000000; i++)
            {
                sum += a + b;
            }
            return sum;
        }
    }
}
