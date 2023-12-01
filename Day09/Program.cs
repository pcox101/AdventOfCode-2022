using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day09
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);
            HashSet<string> visitedPositions = new HashSet<string>();

            int ropeLength = 9;
            int[] ropePositionX = new int[ropeLength + 1];
            int[] ropePositionY = new int[ropeLength + 1];

            string s = t.ReadLine();

            while (s != null)
            {
               int offsetX = 0, offsetY = 0;
               string[] split = s.Split(' ');
               switch (split[0])
               {
                  case "R":
                     offsetX = 1; offsetY = 0;
                     break;
                  case "L":
                     offsetX = -1; offsetY = 0;
                     break;
                  case "U":
                     offsetX = 0; offsetY = -1;
                     break;
                  case "D":
                     offsetX = 0; offsetY = 1;
                     break;
               }

               for (int i = 0; i < Convert.ToInt32(split[1]); i++)
               {
                  // Move the first item
                  ropePositionX[0] += offsetX;
                  ropePositionY[0] += offsetY;

                  // Move each step in the rope relative to the previous
                  for (int j = 1; j < ropeLength + 1; j++)
                  {
                     int diffX = ropePositionX[j] - ropePositionX[j - 1];
                     int diffY = ropePositionY[j] - ropePositionY[j - 1];

                     if ((Math.Abs(diffX) > 1) || (Math.Abs(diffY) > 1))
                     {

                        // Move this item
                        // Movement is slightly more complicated than
                        // just comparison. If in same row/column then move
                        // within that column. If in different row & column
                        // move diagonally
                        if ((ropePositionX[j] == ropePositionX[j - 1]) || (ropePositionY[j] == ropePositionY[j - 1]))
                        {
                           if (ropePositionX[j] < ropePositionX[j - 1] - 1)
                           {
                              ropePositionX[j]++;
                           }
                           else if (ropePositionX[j] > ropePositionX[j - 1] + 1)
                           {
                              ropePositionX[j]--;
                           }
                           if (ropePositionY[j] < ropePositionY[j - 1] - 1)
                           {
                              ropePositionY[j]++;
                           }
                           else if (ropePositionY[j] > ropePositionY[j - 1] + 1)
                           {
                              ropePositionY[j]--;
                           }
                        }
                        else
                        {
                           if ((ropePositionX[j] < ropePositionX[j - 1]))
                           {
                              ropePositionX[j]++;
                           }
                           else if (ropePositionX[j] > ropePositionX[j - 1])
                           {
                              ropePositionX[j]--;
                           }
                           if (ropePositionY[j] < ropePositionY[j - 1])
                           {
                              ropePositionY[j]++;
                           }
                           else if (ropePositionY[j] > ropePositionY[j - 1])
                           {
                              ropePositionY[j]--;
                           }
                        }
                     }
                  }

                  // Now see if this tail position has already been visited
                  string tailPosition = string.Format("{0},{1}", ropePositionX[ropeLength], ropePositionY[ropeLength]);
                  if (!visitedPositions.Contains(tailPosition))
                  {
                     visitedPositions.Add(tailPosition);
                  }

               }

               //RenderMap(ropePositionX, ropePositionY, visitedPositions);

               s = t.ReadLine();
            }

            //RenderMap(ropePositionX, ropePositionY, visitedPositions);
            Console.WriteLine("The tail has visited {0} positions", visitedPositions.Count());
         }
      }

      static void RenderMap(int[] ropePositionX, int[] ropePositionY, HashSet<string> visitedPositions)
      {
         int gridSizeX = 20;
         int gridSizeY = 20;
         //gridSizeX = Math.Max(gridSizeX, Math.Abs(headX));
         //gridSizeY = Math.Max(gridSizeY, Math.Abs(headY));
         //gridSizeX = Math.Max(gridSizeX, Math.Abs(tailX));
         //gridSizeY = Math.Max(gridSizeY, Math.Abs(tailY));

         Console.WriteLine("After move, position is:");
         StringBuilder sb = new StringBuilder();
         for (int y = -gridSizeY; y < gridSizeY; y++)
         { 
            for (int x = -gridSizeX; x < gridSizeX; x++)
            {
               char c = '.';
               string position = string.Format("{0},{1}", x, y);
               for (int i = 0; i < ropePositionX.Length; i++)
               {
                  char insertChar = i.ToString()[0];
                  if (i == 0) insertChar = 'H';
                  if (i == ropePositionX.Length - 1) insertChar = 'T';

                  if ((x == ropePositionX[i]) && (y == ropePositionY[i]))
                  {
                     c = insertChar;
                  }
                  else if (visitedPositions.Contains(position))
                  {
                     c = '#';
                  }
               }
               sb.Append(c);
            }
            sb.AppendLine();
         }
         Console.WriteLine(sb.ToString());
      }
   }
}
