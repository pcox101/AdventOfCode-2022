using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day04
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string line = t.ReadLine();
            int fullOverlaps = 0;
            int partialOverlaps = 0;

            while (line != null)
            {
               string[] cleaningSet = line.Split(',');

               int start1, end1, start2, end2;

               GetValues(cleaningSet[0], out start1, out end1);
               GetValues(cleaningSet[1], out start2, out end2);

               if ((start1 <= start2) && (end1 >= end2) ||
                   (start2 <= start1) && (end2 >= end1))
               {
                  Console.WriteLine(string.Format("{0} and {1} entirely overlap", cleaningSet[0], cleaningSet[1]));
                  fullOverlaps++;
               }
               else if ((start2 <= end1) && (start1 <= end2) ||
                        (end1 <= start2) && (end2 <= start1))
               {
                  Console.WriteLine(string.Format("{0} and {1} partially overlap", cleaningSet[0], cleaningSet[1]));
                  partialOverlaps++;
               }
               else
               {
                  Console.WriteLine(string.Format("{0} and {1} do not overlap", cleaningSet[0], cleaningSet[1]));
               }

               line = t.ReadLine();
            }

            Console.WriteLine(string.Format("There are {0} sets that entirely exist within the other.", fullOverlaps));
            Console.WriteLine(string.Format("There are {0} sets that partially overlap.", partialOverlaps + fullOverlaps));
         }
      }

      private static void GetValues(string set, out int start, out int end)
      {
         string[] split = set.Split('-');

         start = Convert.ToInt32(split[0]);
         end = Convert.ToInt32(split[1]);
      }
   }
}
