using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day25
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();
            long val = 0;

            while (s != null)
            {
               val += ConvertFromSnafu(s);

               s = t.ReadLine();
            }

            string totalSnafu = ConvertToSnafu(val);
            Console.WriteLine(string.Format("Total is {0} which in Snafu is {1}.", val, totalSnafu));
            Console.ReadLine();
         }
      }

      static long ConvertFromSnafu(string snafu)
      {
         long value = 0;

         StringBuilder newBase5 = new StringBuilder();
         StringBuilder allTheTwos = new StringBuilder();

         for (int i = 0; i < snafu.Length; i++)
         {
            switch(snafu[i])
            {
               case '2':
                  newBase5.Append('4');
                  break;
               case '1':
                  newBase5.Append('3');
                  break;
               case '0':
                  newBase5.Append('2');
                  break;
               case '-':
                  newBase5.Append('1');
                  break;
               case '=':
                  newBase5.Append('0');
                  break;
            }
            allTheTwos.Append('2');
         }

         value = ArbitrarySystemToDecimal(newBase5.ToString(), 5);
         value -= ArbitrarySystemToDecimal(allTheTwos.ToString(), 5);

         return value;
      }

      static string ConvertToSnafu(long value)
      {
         StringBuilder newBase5 = new StringBuilder();
         StringBuilder allTheTwos = new StringBuilder();

         newBase5.Append(DecimalToArbitrarySystem(value, 5));
         for (int i = 0; i < newBase5.Length; i++)
         {
            allTheTwos.Append("2");
         }

         long newBase5value = ArbitrarySystemToDecimal(newBase5.ToString(), 5);
         newBase5value += ArbitrarySystemToDecimal(allTheTwos.ToString(), 5);

         // Convert back
         string resultInBase5 = DecimalToArbitrarySystem(newBase5value, 5);
         StringBuilder resultInSnafu = new StringBuilder();

         // map to Snafu
         for (int i = 0; i < resultInBase5.Length; i++)
         {
            switch (resultInBase5[i])
            {
               case '4':
                  resultInSnafu.Append('2');
                  break;
               case '3':
                  resultInSnafu.Append('1');
                  break;
               case '2':
                  resultInSnafu.Append('0');
                  break;
               case '1':
                  resultInSnafu.Append('-');
                  break;
               case '0':
                  resultInSnafu.Append('=');
                  break;
            }
         }

         return resultInSnafu.ToString(); ;
      }

      /// <param name="decimalNumber">The number to convert.</param>
      /// <param name="radix">The radix of the destination numeral system (in the range [2, 36]).</param>
      /// <returns></returns>
      static string DecimalToArbitrarySystem(long decimalNumber, int radix)
      {
         const int BitsInLong = 64;
         const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

         if (radix < 2 || radix > Digits.Length)
            throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

         if (decimalNumber == 0)
            return "0";

         int index = BitsInLong - 1;
         long currentNumber = Math.Abs(decimalNumber);
         char[] charArray = new char[BitsInLong];

         while (currentNumber != 0)
         {
            int remainder = (int)(currentNumber % radix);
            charArray[index--] = Digits[remainder];
            currentNumber = currentNumber / radix;
         }

         string result = new String(charArray, index + 1, BitsInLong - index - 1);
         if (decimalNumber < 0)
         {
            result = "-" + result;
         }

         return result;
      }

      static long ArbitrarySystemToDecimal(string arbitraryNumber, int radix)
      {
         // only works for radix <= 10! :-)
         long v = 0;
         
         for (int i = 0; i < arbitraryNumber.Length; i++)
         {
            v = v * radix + Convert.ToInt64(arbitraryNumber[i].ToString());
         }

         return v;
      }
   }
}
