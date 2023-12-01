using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day06
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            const int numberOfCharsToCheck = 14;

            int counter = numberOfCharsToCheck;
            char[] set = new char[numberOfCharsToCheck];
            int retVal = t.Read(set, 0, numberOfCharsToCheck);

            while (retVal != -1)
            {
               HashSet<char> hs = new HashSet<char>();
               bool unique = true;
               for (int i = 0; i < numberOfCharsToCheck; i++)
               {
                  if (hs.Contains(set[i]))
                  {
                     unique = false;
                     break;
                  }
                  else
                  {
                     hs.Add(set[i]);
                  }
               }

               if (unique)
               {
                  Console.WriteLine("Found a starting set at character {0}", counter);
                  break;
               }

               retVal = t.Read();

               if (retVal != -1)
               {
                  for (int i = 1; i < numberOfCharsToCheck; i++)
                  {
                     set[i - 1] = set[i];
                  }
                  set[numberOfCharsToCheck - 1] = (char)retVal;
               }
               counter++;
            }
         }
      }
   }
}
