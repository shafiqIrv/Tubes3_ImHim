using System;
using System.Collections.Generic;
using System.Drawing;
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
            using (Bitmap bitmap = new Bitmap(bmpFilePath))
            {
                StringBuilder binaryString = new StringBuilder();

                // Loop through each pixel in the bitmap
                for (int y = 0; y < bitmap.Height; y++)
                {
                    for (int x = 0; x < bitmap.Width; x++)
                    {
                        Color pixel = bitmap.GetPixel(x, y);

                        // Convert ke grayscale
                        int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));

                        // Nilai kritis = 128
                        int binaryValue = grayScale > 128 ? 1 : 0;

                        binaryString.Append(binaryValue);
                    }
                }

                return binaryString.ToString();
            }
        }

        public static int binaryToDecimal(string n)
        {
            string num = n;

            // simpan value desimal
            int dec_value = 0;

            // Inisialisasi base jadi 1
            int base1 = 1;

            int len = num.Length;
            for (int i = len - 1; i >= 0; i--)
            {
                if (num[i] == '1')
                    dec_value += base1;
                base1 = base1 * 2;
            }

            return dec_value;
        }

        public static string setStringtoASCII(string s)
        {

            // Simpan ukuran string
            int N = (s.Length);

            // Normalisasi jika tidak valid
            if (N % 8 != 0)
            {
                while (N % 8 != 0)
                {
                    s = s + "0";
                }
            }

            string res = "";

            // Iterasi
            for (int i = 0; i < N; i += 8)
            {
                int decimal_value = binaryToDecimal((s.Substring(i, 8)));

                res += (char)(decimal_value);
            }

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
