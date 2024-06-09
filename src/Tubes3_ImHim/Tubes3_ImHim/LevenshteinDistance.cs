using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes3_ImHim
{
    class LevenshteinDistance
    {   

        public static int CalculateLevenshteinDistance(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[] prev = new int[m + 1];
            int[] current = new int[m + 1];

            if (n == 0) return m;
            if (m == 0) return n;

            for (int j = 0; j <= m; j++)
                prev[j] = j;

            for (int i = 1; i <= n; i++)
            {
                current[0] = i;
                for (int j = 1; j <= m; j++)
                {
                    int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;
                    current[j] = Math.Min(Math.Min(current[j - 1] + 1, prev[j] + 1), prev[j - 1] + cost);
                }
                prev = (int[])current.Clone(); 
            }

            return current[m];
        }

        public static float Calculate(string s, string t)
        {
            int maxLength = Math.Max(s.Length, t.Length);
            if (maxLength == 0)
                return 1.0f; 

            int distance = CalculateLevenshteinDistance(s, t);
            return (1.0f - (float)distance / maxLength) * 100;
        }


    }
}
