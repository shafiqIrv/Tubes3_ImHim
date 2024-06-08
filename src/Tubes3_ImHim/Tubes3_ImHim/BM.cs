using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes3_ImHim
{
    public class BM
    {
        private static int[] BuildBadCharShift(string pattern)
        {
            // Size of the alphabet (ASCII)
            const int alphabetSize = 256; 
            int[] badCharShift = new int[alphabetSize];

            for (int i = 0; i < alphabetSize; i++)
            {
                badCharShift[i] = -1;
            }

            for (int i = 0; i < pattern.Length; i++)
            {
                badCharShift[(int)pattern[i]] = i;
            }

            return badCharShift;
        }

        public static bool Search(string pattern,string text)
        {
            int[] badCharShift = BuildBadCharShift(pattern);
            int m = pattern.Length;
            int n = text.Length;
            int skip;

            for (int i = 0; i <= n - m; i += skip)
            {
                skip = 0;
                for (int j = m - 1; j >= 0; j--)
                {
                    if (pattern[j] != text[i + j])
                    {
                        skip = Math.Max(1, j - badCharShift[(int)text[i + j]]);
                        break;
                    }
                }
                if (skip == 0)
                {
                    return true; // Pattern found
                }
            }

            return false; // Pattern not found
        }
    }
}
