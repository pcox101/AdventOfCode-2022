using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day12
{
   class Program
   {

      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            List<string> rows = new List<string>();
            int width = s.Length;
            int length = 0;

            while (s != null)
            {
               length++;
               rows.Add(s);

               s = t.ReadLine();
            }

            int[,] heightmap = new int[width, length];

            int startX = 0, startY = 0, endX = 0, endY = 0;
            for (int row = 0; row < length; row++)
            {
               for (int column = 0; column < width; column++)
               {
                  if (rows[row][column] == 'S')
                  {
                     startX = column; startY = row;
                     heightmap[column, row] = 1;
                  }
                  else if (rows[row][column] == 'E')
                  {
                     endX = column; endY = row;
                     heightmap[column, row] = 26;
                  }
                  else
                  {
                     heightmap[column, row] = Convert.ToInt32(rows[row][column]) - 96;
                  }
               }
            }

            // We can start at any point with height 1
            int lowestCount = 0;
            for (int x = 0; x < width; x++)
            {
               for (int y = 0; y < length; y++)
               {
                  int[,] minMoves = new int[width, length];
                  int numberOfMoves = 1;

                  if (heightmap[x, y] == 1)
                  {
                     Console.WriteLine(string.Format("Calculating [{0},{1}]", x, y));
                     if ((x == startX) && (y == startY))
                     {
                        Console.WriteLine("This was the starting position");
                     }
                     minMoves[x, y] = numberOfMoves;
                     CalculateMoves(heightmap, x, y, minMoves, numberOfMoves + 1, width, length);
                     int minimumMoves = minMoves[endX, endY] - 1;
                     Console.WriteLine(string.Format("Minimum moves to end is {0}.", minimumMoves));
                     if (minimumMoves == -1)
                     {
                        Console.WriteLine("No solution");
                     }
                     else if ((minimumMoves < lowestCount) || (lowestCount == 0))
                     {

                        Console.WriteLine("This is a new minimum");
                        DrawVisualMap(minMoves, width, length, endX, endY);
                        lowestCount = minimumMoves;
                     }
                  }
               }
            }
            Console.WriteLine(string.Format("Best moves to end is {0}.", lowestCount));

         }
      }

      private static void CalculateMoves(int[,] heightmap,
         int x,
         int y,
         int[,] minMoves,
         int numberOfMoves,
         int width,
         int length)
      {
         bool moveUp = false;
         bool moveDown = false;
         bool moveLeft = false;
         bool moveRight = false;

         // Up
         if (y != 0)
         {
            // Can we move there?
            if (heightmap[x, y] + 1 >= heightmap[x, y - 1])
            {
               // Should we move there?
               if ((numberOfMoves < minMoves[x, y - 1]) || (minMoves[x, y - 1] == 0))
               {
                  // we can move there in this number of moves and it's better than the best route we have
                  // so update this one
                  minMoves[x, y - 1] = numberOfMoves;
                  moveUp = true;
               }
            }
         }
         // Down
         if (y != length - 1)
         {
            // Can we move there?
            if (heightmap[x, y] >= heightmap[x, y + 1] - 1)
            {
               // Should we move there?
               if ((numberOfMoves < minMoves[x, y + 1]) || (minMoves[x, y + 1] == 0))
               {
                  // we can move there in this number of moves and it's better than the best route we have
                  minMoves[x, y + 1] = numberOfMoves;
                  moveDown = true;
               }
            }
         }
         // Left
         if (x != 0)
         {
            // Can we move there?
            if (heightmap[x, y] >= heightmap[x - 1, y] - 1)
            {
               // Should we move there?
               if ((numberOfMoves < minMoves[x - 1, y]) || (minMoves[x - 1, y] == 0))
               {
                  // we can move there in this number of moves and it's better than the best route we have
                  minMoves[x - 1, y] = numberOfMoves;
                  moveLeft = true;
               }
            }
         }
         // Right
         if (x != width - 1)
         {
            // Can we move there?
            if (heightmap[x, y] >= heightmap[x + 1, y] - 1)
            {
               // Should we move there?
               if ((numberOfMoves < minMoves[x + 1, y]) || (minMoves[x + 1, y] == 0))
               {
                  // we can move there in this number of moves and it's better than the best route we have
                  minMoves[x + 1, y] = numberOfMoves;
                  moveRight = true;
               }
            }
         }

         // Now move to each one
         if (moveUp)
            CalculateMoves(heightmap, x, y - 1, minMoves, numberOfMoves + 1, width, length);
         if (moveDown)
            CalculateMoves(heightmap, x, y + 1, minMoves, numberOfMoves + 1, width, length);
         if (moveLeft)
            CalculateMoves(heightmap, x - 1, y, minMoves, numberOfMoves + 1, width, length);
         if (moveRight)
            CalculateMoves(heightmap, x + 1, y, minMoves, numberOfMoves + 1, width, length);
      }

      static void DrawVisualMap(int[,] minMoves, int width, int length, int endX, int endY)
      {
         string[,] visual = new string[width, length];

         // Fill
         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < length; y++)
            {
               visual[x, y] = minMoves[x, y].ToString("000 ");
            }
         }

         // Map the path
         {
            int x = endX, y = endY;
            while (true)
            {
               int valAtPosition = minMoves[x, y];
               visual[x, y] = "XXX ";

               if ((x != 0) && (minMoves[x - 1, y] == valAtPosition - 1))
               {
                  x--;
               }
               else if ((x != width - 1) && (minMoves[x + 1, y] == valAtPosition - 1))
               {
                  x++;
               }
               else if ((y != 0) && (minMoves[x, y - 1] == valAtPosition - 1))
               {
                  y--;
               }
               else if ((y != length - 1) && (minMoves[x, y + 1] == valAtPosition - 1))
               {
                  y++;
               }
               else
               {
                  Console.Write("Impossible!");
                  break;
               }

               if (minMoves[x, y] == 1)
                  break;
            }

         }

         Console.WriteLine("Visual Representation:");
         for (int x = 0; x < width; x++)
         {
            for (int y = 0; y < length; y++)
            {
               if (visual[x, y] == "XXX ")
               {
                  Console.BackgroundColor = ConsoleColor.Red;
                  Console.Write(visual[x, y].Substring(0, 3));
                  Console.BackgroundColor = ConsoleColor.Black;
                  Console.Write(" ");
               }
               else
               {
                  Console.Write(visual[x, y]);
               }
            }
            Console.WriteLine();
         }
      }
   }
}
