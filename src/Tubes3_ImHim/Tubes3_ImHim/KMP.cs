using System;
using System.Collections.Generic;

namespace Tubes3_ImHim
{
    public class KMP
    {
        public static bool Search(string pattern, string text)
        {
            int p = pattern.Length;
            int t = text.Length;

            // Create the border function which will store the length of the longest prefix suffix
            int[] border_function = new int[p];
            int j = 0; // Index for pattern[]

            // Calculate the border_function[] array
            CreateBorderFunction(pattern, p, border_function);

            int i = 0; // Index for text[]
            while (i < t)
            {
                if (pattern[j] == text[i])
                {
                    j++;
                    i++;
                }

                if (j == p)
                {
                    return true; // Pattern found, return true
                }
                // Mismatch after j matches
                else if (i < t && pattern[j] != text[i])
                {
                    // Check if text[i] character is in pattern
                    if (!pattern.Contains(text[i]))
                    {
                        i += j + 1; // Continue from the position after the mismatch character
                        j = 0; // Reset j
                    }
                    else
                    {
                        if (j != 0)
                            j = border_function[j - 1];
                        else
                            i = i + 1;
                    }
                }
            }

            return false; // Pattern not found
        }

        // Function to calculate the border_function[] array
        private static void CreateBorderFunction(string pattern, int p, int[] border_function)
        {
            int len = 0;
            int i = 1;
            border_function[0] = 0; // border_function[0] is always 0

            while (i < p)
            {
                if (pattern[i] == pattern[len])
                {
                    len++;
                    border_function[i] = len;
                    i++;
                }
                else
                {
                    if (len != 0)
                    {
                        len = border_function[len - 1];
                    }
                    else
                    {
                        border_function[i] = 0;
                        i++;
                    }
                }
            }
        }
    }
}
