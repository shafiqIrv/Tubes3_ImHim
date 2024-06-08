using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes3_ImHim
{
    class LCS
    {
        public static float Calculate(string text1, string text2)
        {
            int m = text1.Length;
            int n = text2.Length;
            int[,] dp = new int[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (text1[i - 1] == text2[j - 1])
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            // return float
            int lcsLength = dp[m, n];
            int maxLength = Math.Max(m, n);
            return maxLength > 0 ? (float)lcsLength / maxLength : 0;
        }
    }
}
