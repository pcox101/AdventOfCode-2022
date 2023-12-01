using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day14
{
   class Program
   {
      static void Main(string[] args)
      {
         Coordinate min, max;

         List<CoordinatePair> allCoords = ParseInputFile(out min, out max);

         Space[,] space = BuildSpace(allCoords, min, max);

         DrawSpace(space, min, max);

         // Sand starts to pour in
         int counter = 0;

         while (true)
         {
            if (DropInSand(space, ref min, ref max))
            {
               break;
            }

            counter++;
            //DrawSpace(space, min, max);
         }
         
         DrawSpace(space, min, max);
         
         Console.WriteLine(string.Format("There were {0} grains of sand before we dropped off.", counter + 1));

      }

      private static bool DropInSand(Space[,] gameSpace, ref Coordinate min, ref Coordinate max)
      {
         bool done = false;
         
         Coordinate sandPosition = new Coordinate("500,0");

         while (true)
         {
            // Down/Downleft/Downright
            if (gameSpace[sandPosition.X,sandPosition.Y + 1] == Space.Empty)
            {
               sandPosition.Move(0, 1);
            }
            else if (gameSpace[sandPosition.X - 1, sandPosition.Y + 1] == Space.Empty)
            {
               sandPosition.Move(-1, 1);
            }
            else if (gameSpace[sandPosition.X + 1, sandPosition.Y + 1] == Space.Empty)
            {
               sandPosition.Move(1, 1);
            }
            else
            {
               break;
            }

            if (sandPosition.X > max.X)
            {
               max.X = sandPosition.X;
            }

            // Stop on the bottom row
            if (sandPosition.Y > max.Y)
            {
               break;
            }
         }

         if ((sandPosition.X == 500) && (sandPosition.Y == 0))
         {
            done = true;
         }

         gameSpace[sandPosition.X, sandPosition.Y] = Space.Sand;

         return done;

      }

      private static Space[,] BuildSpace(List<CoordinatePair> allCoords, Coordinate min, Coordinate max)
      {
         Space[,] builtSpace = new Space[max.X + 500, max.Y + 2];

         foreach(CoordinatePair pair in allCoords)
         {
            int lowerX, higherX, lowerY, higherY;

            if (pair.Start.X > pair.End.X)
            {
               lowerX = pair.End.X;
               higherX = pair.Start.X;
            }
            else
            {
               lowerX = pair.Start.X;
               higherX = pair.End.X;
            }
            if (pair.Start.Y > pair.End.Y)
            {
               lowerY = pair.End.Y;
               higherY = pair.Start.Y;
            }
            else
            {
               lowerY = pair.Start.Y;
               higherY = pair.End.Y;
            }

            // These should always be in the same column/row
            for (int x = lowerX; x <= higherX; x++)
            {
               for (int y = lowerY; y <= higherY; y++)
               {
                  builtSpace[x, y] = Space.Rock;
               }
            }
         }

         return builtSpace;
      }

      static List<CoordinatePair> ParseInputFile(out Coordinate min, out Coordinate max)
      {
         min = new Coordinate("1000,1000");
         max = new Coordinate("0,0");

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            List<CoordinatePair> coordinates = new List<CoordinatePair>();

            string s = t.ReadLine();

            while (s != null)
            {
               coordinates.AddRange(ParseInputLine(s, ref min, ref max));

               s = t.ReadLine();
            }

            return coordinates;
         }
      }

      private static List<CoordinatePair> ParseInputLine(string inputLine,ref  Coordinate min, ref Coordinate max)
      {
         List<CoordinatePair> coordinates = new List<CoordinatePair>();

         string[] split = inputLine.Split(' ');

         Coordinate previous = null;
         foreach(string coords in split)
         {
            if (coords != "->")
            {
               if (previous == null)
               {
                  previous = new Coordinate(coords);
                  UpdateMinMax(min, max, previous);
               }
               else
               {
                  CoordinatePair pair = new CoordinatePair();
                  pair.Start = previous;
                  pair.End = new Coordinate(coords);
                  previous = pair.End;
                  coordinates.Add(pair);
                  UpdateMinMax(min, max, previous);
               }
            }
         }

         return coordinates;
      }

      private static void UpdateMinMax(Coordinate min, Coordinate max, Coordinate previous)
      {
         if (previous.X < min.X)
         {
            min.X = previous.X;
         }
         if (previous.Y < min.Y)
         {
            min.Y = previous.Y;
         }
         if (previous.X > max.X)
         {
            max.X = previous.X;
         }
         if (previous.Y > max.Y)
         {
            max.Y = previous.Y;
         }
      }

      static void DrawSpace(Space[,] space, Coordinate min, Coordinate max)
      {
         for (int y = 0; y < max.Y + 2; y++)
         {
            for (int x = min.X - 1; x < max.X + 1; x++)
            {
               switch (space[x, y])
               {
                  case Space.Rock:
                     Console.Write("#");
                     break;
                  case Space.Empty:
                     Console.Write(".");
                     break;
                  case Space.Sand:
                     Console.Write("o");
                     break;
                  default:
                     break;
               }
            }
            Console.WriteLine();
         }
      }
   }

   enum Space
   {
      Empty,
      Rock,
      Sand
   }

   class CoordinatePair
   {
      public Coordinate Start { get; set; }
      public Coordinate End { get; set; }
   }

   class Coordinate
   {
      public Coordinate(string input)
      {
         string[] split = input.Split(',');
         X = Convert.ToInt32(split[0]);
         Y = Convert.ToInt32(split[1]);
      }

      public void Move(int relativeX, int relativeY)
      {
         X = X + relativeX;
         Y = Y + relativeY;
      }
      public int X { get; set; }
      public int Y { get; set; }
   }
}
