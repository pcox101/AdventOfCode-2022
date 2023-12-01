using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day21
{
   class Program
   {
      static void Main(string[] args)
      {
         Dictionary<string, Monkey> monkeys = new Dictionary<string, Monkey>();

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            while (s != null)
            {
               Monkey newMonkey = new Monkey(s);
               monkeys.Add(newMonkey.Name, newMonkey);

               s = t.ReadLine();
            }
         }

         // Part 1
         long result = monkeys["root"].Calculate(monkeys);

         Console.WriteLine(string.Format("Monkey 'root' has result {0}", result));



         // Part 2
         // need to identify a value for humn that makes the two root variables the same
         // This is basically solving a massive equation...

         Monkey monkeyRoot1 = monkeys[monkeys["root"].Monkey1];
         Monkey monkeyRoot2 = monkeys[monkeys["root"].Monkey2];

         Monkey monkeyToWorkOn = null;
         long valueToWorkOn = 0;

         if (!monkeyRoot1.ContainsHUMN(monkeys))
         {
            monkeyToWorkOn = monkeyRoot2;
            valueToWorkOn = monkeyRoot1.Calculate(monkeys);
         }
         else if (!monkeyRoot2.ContainsHUMN(monkeys))
         {
            monkeyToWorkOn = monkeyRoot1;
            valueToWorkOn = monkeyRoot2.Calculate(monkeys);
         }
         else
         {
            Console.WriteLine("Two variables?");
         }

         // So, starting with the equation for the monkey we're working on, take our value and
         // reverse the operation until we reach the humn
         long humn = valueToWorkOn;
         Monkey thisMonkey = monkeyToWorkOn;
         while (monkeyToWorkOn.Name != "humn")
         {
            Monkey monkey1 = monkeys[monkeyToWorkOn.Monkey1];
            Monkey monkey2 = monkeys[monkeyToWorkOn.Monkey2];

            if (!monkey2.ContainsHUMN(monkeys))
            {
               // Great monkey2 is a constant, reverse the operation and move to monkey1
               humn = Helper.ReverseOperate(humn, monkeyToWorkOn.Operation, monkey2.Calculate(monkeys));
               monkeyToWorkOn = monkeys[monkeyToWorkOn.Monkey1];
            }
            else if (!monkey1.ContainsHUMN(monkeys))
            {
               // Monkey1 is a constant
               // Reverse the operation and move to monkey2
               if ((monkeyToWorkOn.Operation == "+") || (monkeyToWorkOn.Operation == "*"))
               {
                  humn = Helper.ReverseOperate(humn, monkeyToWorkOn.Operation, monkey1.Calculate(monkeys));
               }
               else
               {
                  humn = Helper.Operate(monkey1.Calculate(monkeys), monkeyToWorkOn.Operation, humn);

               }
               monkeyToWorkOn = monkeys[monkeyToWorkOn.Monkey2];
            }
            else
            {
               // Shouldn't ever happen...
            }
         }
            
         Console.WriteLine(string.Format("Value for humn is {0}", humn));

      }

   }

   class Monkey
   {
      public string Name { get; private set; }
      public long Value { get; set; }
      public string Monkey1 { get; set; }
      public string Monkey2 { get; set; }
      public string Operation { get; set; }

      public Monkey(string line)
      {
         Regex calculationMonkey = new Regex(@"^(.*): (.*) (.*) (.*)$");
         Regex valueMonkey = new Regex(@"^(.*): (\d*)$");

         Match vm = valueMonkey.Match(line);

         if (vm.Success)
         {
            Name = vm.Groups[1].Value;
            Value = Convert.ToInt64(vm.Groups[2].Value);
         }
         else
         {
            Match cm = calculationMonkey.Match(line);
            Value = -1;
            Name = cm.Groups[1].Value;
            Monkey1 = cm.Groups[2].Value;
            Operation = cm.Groups[3].Value;
            Monkey2 = cm.Groups[4].Value;
         }
      }

      public long Calculate(Dictionary<string, Monkey> monkeys)
      {
         if (Value != -1)
         {
            return Value;
         }

         return Helper.Operate(monkeys[Monkey1].Calculate(monkeys), Operation, monkeys[Monkey2].Calculate(monkeys));
      }

      public bool ContainsHUMN(Dictionary<string,Monkey> monkeys)
      {
         if (Name == "humn")
         {
            return true;
         }
         else if (monkeys[Name].Value != -1)
         {
            return false;
         }
         else
         {
            bool monkey1Contains = monkeys[monkeys[Name].Monkey1].ContainsHUMN(monkeys);
            bool monkey2Contains = monkeys[monkeys[Name].Monkey2].ContainsHUMN(monkeys);
            return monkey1Contains || monkey2Contains;
         }
      }
   }

   static class Helper
   {
      public static long Operate(long number1, string operation, long number2)
      {
         if (operation == "+")
         {
            return number1 + number2;
         }
         else if (operation == "-")
         {
            return number1 - number2;
         }
         else if (operation == "*")
         {
            return number1 * number2;
         }
         else if (operation == "/")
         {
            return number1 / number2;
         }
         else
         {
            throw new Exception(string.Format("Unknown operation {0}", operation));
         }
      }
      public static long ReverseOperate(long number1, string operation, long number2)
      {
         if (operation == "+")
         {
            return number1 - number2;
         }
         else if (operation == "-")
         {
            return number1 + number2;
         }
         else if (operation == "*")
         {
            return number1 / number2;
         }
         else if (operation == "/")
         {
            return number1 * number2;
         }
         else
         {
            throw new Exception(string.Format("Unknown operation {0}", operation));
         }
      }
   }
}
