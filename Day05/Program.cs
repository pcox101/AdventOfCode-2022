using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day05
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string line = t.ReadLine();

            // Get the stacks (loop until a blank line)
            Stack<string> allStacks = new Stack<string>();
            while (line != "")
            {
               allStacks.Push(line);
               line = t.ReadLine();
            }

            List<Stack<char>> stacks = ParseStack(allStacks);

            // Now do the moves
            line = t.ReadLine();
            while (line != null)
            {
               int number = 0, source = 0, destination = 0;

               Regex regex = new Regex("^move (\\d*) from (\\d*) to (\\d*)$");
               Match match = regex.Match(line);

               number = Convert.ToInt32(match.Groups[1].Value);
               source = Convert.ToInt32(match.Groups[2].Value);
               destination = Convert.ToInt32(match.Groups[3].Value);

               Stack<char> tempStack = new Stack<char>();

               for (int i = 0; i < number; i++)
               {
                  char moveIt = stacks[source - 1].Pop();
                  tempStack.Push(moveIt);
               }
               for (int i = 0; i < number; i++)
               {
                  char moveIt = tempStack.Pop();
                  stacks[destination - 1].Push(moveIt);
               }


               line = t.ReadLine();
            }

            for (int i = 0; i < stacks.Count(); i++)
            {
               Console.WriteLine(string.Format("Stack {0} has {1} at the top", i, stacks[i].Peek()));
            }
         }
      }

      static List<Stack<char>> ParseStack(Stack<string> allStacks)
      {
         List<Stack<char>> stacks = new List<Stack<char>>();

         // First one is list of stacks, but does tell us how many stacks there are
         string stackIdentifiers = allStacks.Pop();

         int numberOfStacks = (stackIdentifiers.Length + 1) / 4;
         for (int i = 0; i < numberOfStacks; i++)
         {
            stacks.Add(new Stack<char>());
         }

         while (allStacks.Count() > 0)
         {
            string aRow = allStacks.Pop();

            for (int i = 0; i < numberOfStacks; i++)
            {
               char content = aRow[(i * 4) + 1];
               if (content != ' ')
               {
                  stacks[i].Push(content);
               }
            }
         }
         return stacks;
      }
   }
}
