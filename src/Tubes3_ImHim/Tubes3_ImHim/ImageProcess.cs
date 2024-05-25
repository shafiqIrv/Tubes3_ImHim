using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tubes3_ImHim
{
    class ImageProcesser
    {
        public static string BitmapToAscii(string bitmapPath)
        {
            return setStringtoASCII(BitmapToBinary(bitmapPath));

        }

        public static string BitmapToBinary(string bmpFilePath)
        {
            using (FileStream fileStream = new FileStream(bmpFilePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader binaryReader = new BinaryReader(fileStream))
                {
                    byte[] binaryData = binaryReader.ReadBytes((int)fileStream.Length);
                    StringBuilder binaryString = new StringBuilder();

                    foreach (byte b in binaryData)
                    {
                        // Mengonversi setiap byte menjadi string biner 8-bit dan menambahkannya ke StringBuilder
                        binaryString.Append(Convert.ToString(b, 2).PadLeft(8, '0'));
                    }

                    return binaryString.ToString();
                }
            }
        }

        public static int binaryToDecimal(string n)
        {
            string num = n;

            // Stores the decimal value
            int dec_value = 0;

            // Initializing base value to 1
            int base1 = 1;

            int len = num.Length;
            for (int i = len - 1; i >= 0; i--)
            {

                // If the current bit is 1
                if (num[i] == '1')
                    dec_value += base1;
                base1 = base1 * 2;
            }

            // Return answer
            return dec_value;
        }

        public static string setStringtoASCII(string str)
        {

            // To store size of s
            int N = (str.Length);

            // Kalo ga valid, normalisasi, isi 0 dibelakang
            if (N % 8 != 0)
            {
                while (N % 8 != 0)
                {
                    str = str + "0";
                }
            }

            // To store final answer
            string res = "";

            // Loop to iterate through String
            for (int i = 0; i < N; i += 8)
            {
                int decimal_value = binaryToDecimal((str.Substring(i, 8)));

                // Apprend the ASCII character
                // equivalent to current value
                res += (char)(decimal_value);
            }

            // Return Answer
            return res;
        }


        public static string get30MiddleCharacter(string source )
        {
            int desiredLength = 30;

            if (source.Length < desiredLength)
            {
                return "null";
            }
            else
            {
                int startIndex = (source.Length / 2) - (desiredLength / 2);
                string result = source.Substring(startIndex, desiredLength);
                return result;
            }
        }
    }
}
