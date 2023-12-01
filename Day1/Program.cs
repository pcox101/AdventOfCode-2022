using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Day1
{
   class Program
   {
      static void Main(string[] args)
      {

         SortedSet<Elf> elves = GetAndParseInput();

         int totalCalories = 0;
         int[] top3Elves = new int[3];
         
         int i = 0;
         IEnumerator<Elf> en = elves.GetEnumerator();
         en.MoveNext();
         while (i < 3)
         {
            Elf thisElf = en.Current;
            top3Elves[i] = thisElf.Index;
            totalCalories += thisElf.TotalCalories;

            i++;
            en.MoveNext();
         }

         Elf elfWithTheMostest = elves.Last<Elf>();

         Console.WriteLine(string.Format("Elf number {0} is carrying {1} calories",
                                          elfWithTheMostest.Index,
                                          elfWithTheMostest.TotalCalories));

         Console.WriteLine(string.Format("Elf numbers {0}, {1} and {2} is in total carrying {3} calories",
                                 top3Elves[0], top3Elves[1], top3Elves[2],
                                 totalCalories));

      }

      static SortedSet<Elf> GetAndParseInput()
      {
         using (FileStream fs = new FileStream("C:\\Users\\coxpet02\\source\\repos\\AdventOfCode\\Day1\\input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            SortedSet<Elf> result = new SortedSet<Elf>();
            int elfNumber = 1;
            Elf thisElf = new Elf(elfNumber);

            string field = t.ReadLine();
            while (field != null)
            {
               if (string.IsNullOrEmpty(field))
               {
                  result.Add(thisElf);
                  elfNumber++;
                  thisElf = new Elf(elfNumber);
               }
               else
               {
                  thisElf.Calories.Add(Convert.ToInt32(field));
               }
               field = t.ReadLine();
            }
            // Add the last elf
            result.Add(thisElf);

            return result;

         }

      }
   }

   class Elf : IComparable<Elf>
   {
      public Elf(int index)
      {
         Index = index;
      }
      public int Index
      {
         get;
         private set;
      }

      public List<int> Calories = new List<int>();
      public int TotalCalories
      {
         get
         {
            return Calories.Sum();
         }
      }

      public int CompareTo(Elf other)
      {
         return other.TotalCalories - this.TotalCalories;
      }
   }
}
