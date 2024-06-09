using System;
using System.Text.RegularExpressions;

namespace Tubes3_ImHim 
{
    public class RegexClass
    {
        // static void Main()
        // {
        //     string Name = "Alay";
        //     string target = "4l4y";
        //     bool result = IsMatch(Name, target);
        //     Console.WriteLine(result);
        // }

        public static bool IsMatch(string Name, string target)
        {
            // Membersihkan input alayName dan target
            Name = CleanString(Name);
            target = CleanString(target);

            // Membuat regex berdasarkan input nama alay
            string regexPattern = GenerateRegex(Name);

            // Mencocokkan target string dengan regex
            Regex regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            return regex.IsMatch(target);
        }

        static string CleanString(string input)
        {
            // Menghilangkan spasi dan mengubah ke huruf kecil
            return input.Replace(" ", "").ToLower();
        }

        static string RemoveVokal(string input)
        {
            // Menghilangkan huruf vokal dari input
            return Regex.Replace(input, "[aiueo]", "");
        }

        static string GenerateRegex(string Name)
        {
            // konversi ke regex
            string pattern = Name.Replace("a", "[a4]?")
                                    .Replace("b", "[b8]")
                                    .Replace("e", "[e3]?")
                                    .Replace("g", "[g6]")
                                    .Replace("i", "[i1]?")
                                    .Replace("o", "[o0]?")
                                    .Replace("s", "[s5]")
                                    .Replace("t", "[t7]")
                                    .Replace("z", "[z2]")
                                    .Replace("u", "[u]?");
            return pattern;
        }
    }
}
