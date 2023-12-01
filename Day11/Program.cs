using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day11
{
   class Program
   {
      static void Main(string[] args)
      {
         List< Monkey> monkeys = new List<Monkey>();

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            while (true)
            {
               Monkey newMonkey = ParseMonkey(t); 

               if (newMonkey == null)
               {
                  break;
               }
               monkeys.Add(newMonkey);
            }
         }

         ulong lowestFactorial = 1;
         for (int i = 0; i < monkeys.Count; i++)
         {
            lowestFactorial *= monkeys[i].DivisibleTest;
         }

         // Now play the game
         for (int round = 1; round <= 10000; round++)
         {
            // Loop through each monkey
            foreach (Monkey monkey in monkeys)
            {
               // Loop through each item in the monkey's item list
               for (int itemIndex = 0; itemIndex < monkey.Items.Count; itemIndex++)
               {
                  monkey.InspectedItems++;

                  ulong originalWorryLevel = monkey.Items[itemIndex];

                  ulong newWorryLevel = monkey.Operation(originalWorryLevel);
                  //newWorryLevel = newWorryLevel / 3;

                  
                  if (originalWorryLevel > newWorryLevel)
                  {
                     Console.WriteLine("Ooops!");
                  }

                  newWorryLevel = newWorryLevel % lowestFactorial;
               
                  ulong remainder = newWorryLevel % monkey.DivisibleTest;

                  int indexToThrowTo = 0;
                  if (remainder == 0)
                  {
                     indexToThrowTo = monkey.TrueMonkeyIndex;
                  }
                  else
                     indexToThrowTo = monkey.FalseMonkeyIndex;

                  monkeys[indexToThrowTo].Items.Add(newWorryLevel);
               }
               monkey.Items.Clear();
            }

            // At the end of round output
#if false
            Console.WriteLine(string.Format("After round {0}, the monkeys are holding items with these worry levels:", round));
            foreach (Monkey monkey in monkeys)
            {
               Console.Write(string.Format("Monkey {0}: ", monkey.Index));
               foreach (ulong item in monkey.Items)
               {
                  Console.Write(item.ToString() + ",");
               }
               Console.WriteLine();
            }
#endif
            if ((round == 1) || (round == 20) || (round % 1000 == 0))
            {
               Console.WriteLine(string.Format("After round {0}, worry levels are:", round));
               foreach (Monkey monkey in monkeys)
               {
                  Console.WriteLine(string.Format("Monkey {0} inspected items {1} times ", monkey.Index, monkey.InspectedItems));
               }
            }
         }

         // Find the 2 most active monkeys
         monkeys.Sort((i, j) => { return j.InspectedItems.CompareTo(i.InspectedItems); });

         ulong monkeyBusiness = monkeys[0].InspectedItems * monkeys[1].InspectedItems;

         Console.WriteLine(string.Format("Monkey Business is {0}", monkeyBusiness));

      }

      public static Monkey ParseMonkey(TextReader t)
      {
         string s = t.ReadLine();
         if (s == null)
         {
            return null;
         }
         if (s == "")
         {
            s = t.ReadLine();
         }
         
         // Monkey 1:
         int index = Convert.ToInt32(s.Substring(7, s.Length - 8));

         Monkey newMonkey = new Monkey(index);

         // Starting items: x, y, z
         s = t.ReadLine();
         uint currentItem = 0;
         for (int offset = 18; offset < s.Length; offset++)
         {
            if (s[offset] == ',')
            {
               newMonkey.Items.Add(currentItem);
               currentItem = 0;
            }
            else if (s[offset] == ' ')
            {

            }
            else
            {
               currentItem *= 10;
               currentItem += Convert.ToUInt32(s.Substring(offset, 1));
            }
         }
         newMonkey.Items.Add(currentItem);

         // Operation:   Operation: new = old + 6
         s = t.ReadLine();
         newMonkey.Operation = ParseOperation(s);

         // Test: divisible by 19
         s = t.ReadLine();
         newMonkey.DivisibleTest = Convert.ToUInt32(s.Substring(21));

         // If true: throw to monkey 2
         // If false: throw to monkey 0
         s = t.ReadLine();
         newMonkey.TrueMonkeyIndex = Convert.ToInt32(s.Substring(28));
         s = t.ReadLine();
         newMonkey.FalseMonkeyIndex = Convert.ToInt32(s.Substring(29));

         return newMonkey;
      }

      static Func<ulong, ulong> ParseOperation(string s)
      {
         // Operation: old = old + x
         // Operation: old = old * x
         // Operation: old = old * old
         if (s.Contains("old * old"))
         {
            return i =>  i * i;
         }
         ulong value = Convert.ToUInt64(s.Substring(25));
         if (s.Contains("*"))
            return i => i * value;
         else
            return i => i + value;
      }
   }
   
   class Monkey
   {
      public Monkey(int index)
      {
         Items = new List<ulong>();
         Index = index;
      }

      public int Index { get; private set; }
      public List<ulong> Items { get; private set; }
      public ulong DivisibleTest { get; set; }

      public int TrueMonkeyIndex { get; set; }
      public int FalseMonkeyIndex { get; set; }

      public Func<ulong, ulong> Operation { get; set; }

      public ulong InspectedItems { get; set; }
   }
}
