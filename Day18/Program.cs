using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day18
{
   class Program
   {
      private static Random _random = new Random();
      static void Main(string[] args)
      {
         HashSet<string> hashSet = new HashSet<string>();
         HashSet<string> outsideHashSet;// = new HashSet<string>();
         HashSet<string> insideHashSet = new HashSet<string>();

         int minX = 9999, minY = 9999, minZ = 9999;
         int maxX = 0, maxY = 0, maxZ = 0;

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            while (s != null)
            {
               hashSet.Add(s);

               int x, y, z;
               GetXYZfromString(s, out x, out y, out z);
               minX = Math.Min(minX, x);
               minY = Math.Min(minY, y);
               minZ = Math.Min(minZ, z);

               maxX = Math.Max(maxX, x);
               maxY = Math.Max(maxY, y);
               maxZ = Math.Max(maxZ, z);

               s = t.ReadLine();
            }
         }

         int numberOfSides = 0;
         int numberOfExternalSides = 0;

         // Build our outside hashset
         {
            outsideHashSet = BuildOutsideHashset(hashSet, "0,0,0", maxX, maxY, maxZ);
         }

         foreach (string cube in hashSet)
         {
            int x, y, z;
            GetXYZfromString(cube, out x, out y, out z);

            for (int i = 0; i < 6; i++)
            {
               string sToTest = MoveLocation(x, y, z, i);
               if (!hashSet.Contains(sToTest))
               {
                  numberOfSides += 1;

                  // Now see if this side is one of the ones that we've already
                  // tested for inside
                  if (insideHashSet.Contains(sToTest))
                  {
                     // Definitely inside
                     continue;
                  }

                  if (IsOutside(hashSet, ref outsideHashSet, ref insideHashSet, sToTest, minX, minY, minZ, maxX, maxY, maxZ))
                  //if (outsideHashSet.Contains(sToTest))
                  {
                     numberOfExternalSides += 1;
                  }
               }
            }
         }

         Console.WriteLine(string.Format("This shape has {0} sides.", numberOfSides));
         Console.WriteLine(string.Format("Of which {0} are external.", numberOfExternalSides));
      }

      private static HashSet<string> BuildOutsideHashset(HashSet<string> hashSet, string v, int maxX, int maxY, int maxZ)
      {
         List<string> positionsToTest = new List<string>();
         HashSet<string> outsideHashSet = new HashSet<string>();
         positionsToTest.Add(v);

         while (positionsToTest.Count > 0)
         {
            string s = positionsToTest[0];
            positionsToTest.RemoveAt(0);

            // Add this position
            if (outsideHashSet.Contains(s))
            {
               continue;
            }

            outsideHashSet.Add(s);

            int x, y, z;
            GetXYZfromString(s, out x, out y, out z);

            for (int i = 0; i < 6; i++)
            {
               int newx, newy, newz;
               string news = MoveLocation(x, y, z, i);

               if (hashSet.Contains(news))
               {
                  continue;
               }

               GetXYZfromString(news, out newx, out newy, out newz);

               if ((newx >= 0) && (newx <= maxX) &&
                   (newy >= 0) && (newy <= maxY) && 
                   (newz >= 0) && (newz <= maxZ))
               {
                  if (!outsideHashSet.Contains(news))
                  {
                     positionsToTest.Add(news);
                  }
               }
            }
         }
         return outsideHashSet;
      }

      private static string MoveLocation(int x, int y, int z, int i)
      {
         int newx = x;
         int newy = y;
         int newz = z;

         if (i == 0)
         {
            newx = x + 1;
         }
         else if (i == 1)
         {
            newx = x - 1;
         }
         else if (i == 2)
         {
            newy = y + 1;
         }
         else if (i == 3)
         {
            newy = y - 1;
         }
         else if (i == 4)
         {
            newz = z + 1;
         }
         else if (i == 5)
         {
            newz = z - 1;
         }

         return string.Format("{0},{1},{2}", newx, newy, newz);
      }

      static void GetXYZfromString(string cube, out int x, out int y, out int z)
      {
         Regex reg = new Regex(@"^(-?\d*),(-?\d*),(-?\d*)$");
         Match match = reg.Match(cube);
         x = Convert.ToInt32(match.Groups[1].Value);
         y = Convert.ToInt32(match.Groups[2].Value);
         z = Convert.ToInt32(match.Groups[3].Value);
      }

      private static bool IsOutside(HashSet<string> hashSet,
                                    ref HashSet<string> outsideHashSet,
                                    ref HashSet<string> insideHashSet,
                                    string sToTest,
                                    int minX, int minY, int minZ,
                                    int maxX, int maxY, int maxZ)
      {
         HashSet<string> allTheOnesWeTried = new HashSet<string>();
         List<string> optionsToTry = new List<string>();

         optionsToTry.Add(sToTest);

         bool isOutside = false;
         int counter = 0;

         while (optionsToTry.Count > 0)
         {
            counter++;
            string s = optionsToTry[0];
            optionsToTry.RemoveAt(0);

            // Check we've not tried it already
            if (allTheOnesWeTried.Contains(s))
            {
               continue;
            }
            else
            {
               allTheOnesWeTried.Add(s);
            }

            int x, y, z;
            GetXYZfromString(s, out x, out y, out z);

            // Already tested this position and it's outside, we must be outside
            if (outsideHashSet.Contains(s))
            {
               isOutside = true;
               break;
            }

            // Already tested this position and it's inside, we must be inside
            if (insideHashSet.Contains(s))
            {
               isOutside = false;
               break;
            }

            // Is this outside?
            if ((x > maxX + 1) || (y > maxY + 1) || (z > maxZ + 1)
               || (x < minX - 1) || (y < minY - 1) || (z < minZ - 1))
            {
               isOutside = true;
               break;
            }

            // Add all directions (that are not walls) to the collection
            for (int i = 0; i < 6; i++)
            {
               string newPosition = MoveLocation(x, y, z, i);

               // Is it a wall?
               if (!hashSet.Contains(newPosition))
               {
                  optionsToTry.Add(newPosition);
                  //allTheOnesWeTried.Add(newPosition);
               }
            }
         }

         if (isOutside)
         {
            //Console.WriteLine(string.Format("Position {0} is definitely outside!", sToTest));
            outsideHashSet.UnionWith(allTheOnesWeTried);
         }
         else
         {
            insideHashSet.UnionWith(allTheOnesWeTried);
            Console.WriteLine(string.Format("Position {0} appears to be inside!", sToTest));
            Console.WriteLine(string.Format("We ascertained this after {0} steps", counter));
         }

         return isOutside;
      }
   }
}
