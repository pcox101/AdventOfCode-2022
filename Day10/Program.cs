using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day10
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s;
            StringBuilder crt = new StringBuilder();

            bool done = false;
            int cycle = 1;
            string currentInstruction = String.Empty;
            int xregister = 1;
            int operationCycle = 0;
            int operationValue = 0;
            int sumOperation = 0;

            while (!done)
            {
               if (string.IsNullOrEmpty(currentInstruction))
               {
                  s = t.ReadLine();
                  if (s == null)
                  {
                     done = true;
                     break;
                  }

                  string[] split = s.Split(' ');

                  currentInstruction = split[0];
                  if (currentInstruction == "noop")
                  {
                     // takes 1 cycle
                     operationCycle = 1;
                  }
                  else if (currentInstruction == "addx")
                  {
                     // takes 2 cycles
                     operationCycle = 2;
                     operationValue = Convert.ToInt32(split[1]);
                  }
               }

               // Is it a special cycle?
               if ((cycle == 20) || ((cycle + 20) % 40) == 0)
               {
                  Console.WriteLine(string.Format("Cycle {0} is special. xregister is {1}", cycle, xregister));
                  sumOperation += (cycle * xregister);
               }

               // Add a character to the CRT
               int crtPosition = cycle % 40;
               if ((xregister == crtPosition) || (xregister == crtPosition - 1) || (xregister == crtPosition - 2))
               {
                  crt.Append("#");
               }
               else
               {
                  crt.Append(".");
               }
               RenderCRT(crt);


               // Now process the instruction
               operationCycle--;

               // Is it complete?
               if (operationCycle == 0)
               {
                  if (currentInstruction == "noop")
                  {

                  }
                  else if (currentInstruction == "addx")
                  {
                     xregister += operationValue;
                  }
                  currentInstruction = "";
               }


               cycle++;
            }

            Console.WriteLine(string.Format("At the end the sum is {0}", sumOperation));
            RenderCRT(crt);
         }
      }

      static void RenderCRT(StringBuilder crt)
      {
         int counter = 1;
         while (counter < (crt.Length + 1))
         {
            Console.Write(crt[counter - 1]);
            if (counter % 40 == 0)
            {
               Console.WriteLine();
            }
            counter++;
         }
         Console.WriteLine();
      }
   }
}
