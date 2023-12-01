using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day15
{
   class Program
   {
      static void Main(string[] args)
      {
         int minX, maxX, maxManhatten;
         int minSensorX, minSensorY, maxSensorX, maxSensorY;

         List<CoordinatePair> inputSet = ParseInputFile(out minX, out maxX, out minSensorX, out minSensorY, out maxSensorX, out maxSensorY, out maxManhatten);

         // Part 1
#if false
         int yToTest = 2000000;

         int count = 0;
         for (int x = minX - maxManhatten - 1; x < maxX + maxManhatten + 1; x++)
         {
            foreach (CoordinatePair pair in inputSet)
            {
               if ((pair.Beacon.X == x) && (pair.Beacon.Y == yToTest))
               {
                  //Console.WriteLine(string.Format("Hit for Beacon at {0},{1}", x, yToTest));
                  break;
               }
               if (PositionWithinRange(pair, x, yToTest))
               {
                  //Console.WriteLine(string.Format("Hit for sensor range at {0},{1}", x, yToTest));
                  count++;
                  break;
               }
            }
         }

         Console.WriteLine(string.Format("There are {0} coordinates that we can sense on that row", count));
#endif
         // Part 2
         long signalFrequency = 0;
         //Console.WriteLine("Testing from {0},{1} to {2},{3}.", minSensorX, minSensorY, maxSensorX, maxSensorY);

         // Check the perimeters! (There is only one point, it must be on the edge of at least one
         // of the perimeters
         Console.WriteLine(string.Format("Minimum position {0},{1}", minSensorX, minSensorY));
         Console.WriteLine(string.Format("Maximum position {0},{1}", maxSensorX, maxSensorY));
         
         foreach (CoordinatePair pair in inputSet)
         {
            int x = pair.Sensor.X - pair.ManhattenDistance - 1;
            int y = pair.Sensor.Y;
            int xMove = 1;
            int yMove = 1;
            bool completed = false;
            bool foundIt = false;

            Console.WriteLine(String.Format("Sensor position {0},{1}", pair.Sensor.X, pair.Sensor.Y));

            while (!completed)
            {
               //Console.WriteLine(string.Format("Testing position {0},{1}", x, y));
               if (!((x < minSensorX) || (x > maxSensorX) || (y < minSensorY) || (y > maxSensorY)))
               {
                  bool anyMatching = false;
                  foreach (CoordinatePair otherPair in inputSet)
                  {
                     if (pair != otherPair)
                     {
                        if (PositionWithinRange(otherPair, x, y))
                        {
                           anyMatching = true;
                           break;
                        }
                     }
                  }
                  if (!anyMatching)
                  {
                     completed = true;
                     foundIt = true;
                     signalFrequency = (long)x * 4000000 + y;
                  }
                  else
                  {
                     //Console.WriteLine("No match");
                  }
               }

               // Move to the next around the sensor
               if (x == pair.Sensor.X + pair.ManhattenDistance + 1)
               {
                  xMove = -1;
               }
               if (y == pair.Sensor.Y + pair.ManhattenDistance + 1)
               {
                  yMove = -1;
               }
               if (x == pair.Sensor.X - pair.ManhattenDistance - 1)
               {
                  xMove = 1;
               }
               if (y == pair.Sensor.Y - pair.ManhattenDistance - 1)
               {
                  yMove = 1;
               }

               x = x + xMove;
               y = y + yMove;

               if ((x == pair.Sensor.X - pair.ManhattenDistance - 1) && (y == pair.Sensor.Y))
               {
                  completed = true;
               };
            }
            if (foundIt)
            {
               break;
            }
            Console.WriteLine("Completed a sensor.");
         }


#if false
         for (int x = minSensorX; x <= maxSensorX; x++)
         {
            for (int y = minSensorY; y <= maxSensorY; y++)
            {
               bool anySensorInRange = false;
               foreach (CoordinatePair pair in inputSet)
               {
                  if (PositionWithinRange(pair, x, y))
                  {
                     anySensorInRange = true;

                     // I can also jump forward on this column
                     int jumpForwardBy = (pair.Sensor.Y - y) * 2;
                     if (jumpForwardBy > 0)
                     {
                        y = y + jumpForwardBy;
                     }
                     break;
                  }
               }

               // Found it!
               if (!anySensorInRange)
               {
                  Console.WriteLine(string.Format("Found gap at {0},{1}", x, y));
                  signalFrequency = x * 4000000 + y;
                  // there can only be one.
                  x = maxSensorX;
                  y = maxSensorY;
               }
            }
            Console.WriteLine(string.Format("Done row {0}.", x));
         }
#endif

         Console.WriteLine(string.Format("Signal frequency is {0}", signalFrequency));
      }

      static List<CoordinatePair> ParseInputFile(out int minX, out int maxX, out int minSensorX, out int minSensorY, out int maxSensorX, out int maxSensorY, out int maxManhatten)
      {
         //min = new Coordinate("1000,1000");
         //max = new Coordinate("0,0");
         minX = 1000;
         maxX = 0;
         maxManhatten = 0;

         minSensorX = 1000000;
         minSensorY = 1000000;
         maxSensorX = 0;
         maxSensorY = 0;

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            List<CoordinatePair> coordinates = new List<CoordinatePair>();

            string s = t.ReadLine();

            while (s != null)
            {
               CoordinatePair pair = new CoordinatePair();
               // "Sensor at x = 2, y = 18: closest beacon is at x = -2, y = 15"

               Regex regex = new Regex(@"^Sensor at x=(-?\d*), y=(-?\d*): closest beacon is at x=(-?\d*), y=(-?\d*)$");

               Match match = regex.Match(s);

               pair.Sensor = new Coordinate();
               pair.Sensor.X = Convert.ToInt32(match.Groups[1].Value);
               pair.Sensor.Y = Convert.ToInt32(match.Groups[2].Value);

               pair.Beacon = new Coordinate();
               pair.Beacon.X = Convert.ToInt32(match.Groups[3].Value);
               pair.Beacon.Y = Convert.ToInt32(match.Groups[4].Value);

               maxX = Math.Max(pair.Sensor.X, maxX);
               maxX = Math.Max(pair.Beacon.X, maxX);
               minX = Math.Min(pair.Sensor.X, minX);
               minX = Math.Min(pair.Beacon.X, minX);

               minSensorX = Math.Min(pair.Sensor.X, minSensorX);
               minSensorY = Math.Min(pair.Sensor.Y, minSensorY);
               maxSensorX = Math.Max(pair.Sensor.X, maxSensorX);
               maxSensorY = Math.Max(pair.Sensor.Y, maxSensorY);

               pair.ManhattenDistance = GetManhattenDistance(pair.Beacon, pair.Sensor);

               if (pair.ManhattenDistance > maxManhatten)
                  maxManhatten = pair.ManhattenDistance;

               coordinates.Add(pair);

               s = t.ReadLine();
            }

            return coordinates;
         }
      }

      // For a given position, is it within range of this sensor?
      private static bool PositionWithinRange(CoordinatePair pair, int x, int y)
      {
         int thisPositionManhattenDistance = GetManhattenDistance(pair.Sensor, x, y);
         
         if (thisPositionManhattenDistance > pair.ManhattenDistance)
         {
            return false;
         }
         else
         {
            return true;
         }

      }

      private static int GetManhattenDistance(Coordinate first, Coordinate second)
      {
         return Math.Abs(first.X - second.X) + Math.Abs(first.Y - second.Y);
      }
      private static int GetManhattenDistance(Coordinate first, int x, int y)
      {
         return Math.Abs(first.X - x) + Math.Abs(first.Y - y);

      }

   }

   class CoordinatePair
   {

      public Coordinate Sensor { get; set; }
      public Coordinate Beacon { get; set; }

      public int ManhattenDistance = 0;
   }

   class Coordinate
   {
      public int X { get; set; }
      public int Y { get; set; }
   }
}
