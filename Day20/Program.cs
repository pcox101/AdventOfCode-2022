using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day20
{
   class Program
   {
      static void Main(string[] args)
      {
         Number start = null;
         List<Number> data = new List<Number>();

         int encryptionKey = 811589153;
         int numberOfMixings = 10;

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);
            Number previous = null;
            Number thisNumber = null;

            string s = t.ReadLine();

            while (s != null)
            {
               thisNumber = new Number() { Value = Convert.ToInt64(s) * encryptionKey, Previous = previous };
               if (previous != null)
               {
                  previous.Next = thisNumber;
               }
               data.Add(thisNumber);

               if (start == null)
               {
                  start = thisNumber;
               }

               previous = thisNumber;

               s = t.ReadLine();
            }

            // Ensure the list wraps
            thisNumber.Next = start;
            start.Previous = thisNumber;
         }

         Console.WriteLine("Starting Position:");
         DrawData(start);

         for (int mixing = 0; mixing < numberOfMixings; mixing++)
         {
            for (int i = 0; i < data.Count; i++)
            {
               Number thisNumber = data[i];

               // (Just to make the output consistent)
               if (thisNumber == start)
               {
                  start = thisNumber.Next;
               }

               // Pull this number out, move its number then put it back in
               Number insertAfter = thisNumber.Previous;

               thisNumber.Previous.Next = thisNumber.Next;
               thisNumber.Next.Previous = thisNumber.Previous;

               Number currentPosition = thisNumber.Previous;

               // We only need to move the remainder (but remember that we've removed the item)
               int numberToMove = (int)(thisNumber.Value % ((long)data.Count - 1));

               for (int counter = 0; counter < Math.Abs(numberToMove); counter++)
               {
                  if (thisNumber.Value < 0)
                  {
                     currentPosition = currentPosition.Previous;
                  }
                  else
                  {
                     currentPosition = currentPosition.Next;
                  }
               }

               // Now put it back in after the current position
               currentPosition.Next.Previous = thisNumber;

               thisNumber.Next = currentPosition.Next;
               thisNumber.Previous = currentPosition;

               currentPosition.Next = thisNumber;

               //Console.WriteLine(string.Format("After moving {0}", data[i].Value));
               //DrawData(start);
            }
            Console.WriteLine(string.Format("Done mixing {0}", mixing + 1));
            DrawData(start);
         }

         Number zero = start;        
         while (true)
         {
            if (zero.Value == 0)
            {
               break;
            }
            zero = zero.Next;
         }

         long sum = 0;
         Number current = zero;
         for (int i = 0; i <= 3000; i++)
         {
            if (i == 1000)
            {
               sum = sum + current.Value;
            }
            else if (i == 2000)
            {
               sum = sum + current.Value;
            }
            else if (i == 3000)
            {
               sum = sum + current.Value;
            }
            current = current.Next;
         }

         Console.WriteLine(string.Format("Grove coordinates have a sum of {0}", sum));
      }

      private static void DrawData(Number start)
      {
         StringBuilder sb = new StringBuilder();
         Number position = start;
         while (true)
         {
            sb.Append(position.Value.ToString());
            sb.Append(", ");
            position = position.Next;
            if (position == start)
            {
               break;
            }
         }
         Console.WriteLine(sb.ToString());
      }
   }
   class Number
   {
      public long Value { get; set; }
      public Number Next { get; set; }
      public Number Previous { get; set; }
   }
}
