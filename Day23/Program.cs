using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day23
{
   class Program
   {
      static Regex _coordinate = new Regex(@"^(-?\d*),(-?\d*)$");

      static void Main(string[] args)
      {
         HashSet<string> gameBoard = new HashSet<string>();

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            int row = 0;

            while (s != null)
            {

               for (int column = 0; column < s.Length; column++)
               {
                  if (s[column] == '#')
                  {
                     string coord = GetStringFromCoords(column, row);
                     gameBoard.Add(coord);
                  }
               }


               row--;
               s = t.ReadLine();
            }
         }

         Console.WriteLine("Starting position:");
         DrawGameBoard(gameBoard);

         int round = 0;
         
         while (true)
         {
            Dictionary<string, List<string>> proposedBoard = new Dictionary<string, List<string>>();
            List<string> notMovingElves = new List<string>();

            foreach (string elf in gameBoard)
            {
               int x, y;
               GetCoordsFromString(elf, out x, out y);

               string n = GetStringFromCoords(x, y + 1);
               string ne = GetStringFromCoords(x + 1, y + 1);
               string e = GetStringFromCoords(x + 1, y);
               string se = GetStringFromCoords(x + 1, y - 1);
               string s = GetStringFromCoords(x, y - 1);
               string sw = GetStringFromCoords(x - 1, y - 1);
               string w = GetStringFromCoords(x - 1, y);
               string nw = GetStringFromCoords(x - 1, y + 1);

               string proposal = "";

               // Firstly check if there is anything around
               if (!gameBoard.Contains(ne) && !gameBoard.Contains(n) && !gameBoard.Contains(nw) &&
                   !gameBoard.Contains(se) && !gameBoard.Contains(s) && !gameBoard.Contains(sw) &&
                   !gameBoard.Contains(w) && !gameBoard.Contains(e))
               {
                  notMovingElves.Add(elf);
               }
               else
               {
                  for (int i = round; i < (round + 4); i++)
                  {
                     if (i % 4 == 0)
                     {
                        if (!gameBoard.Contains(ne) && !gameBoard.Contains(n) && !gameBoard.Contains(nw))
                        {
                           proposal = n;
                           break;
                        }
                     }
                     if (i % 4 == 1)
                     {
                        if (!gameBoard.Contains(se) && !gameBoard.Contains(s) && !gameBoard.Contains(sw))
                        {
                           proposal = s;
                           break;
                        }
                     }
                     if (i % 4 == 2)
                     {
                        if (!gameBoard.Contains(w) && !gameBoard.Contains(nw) && !gameBoard.Contains(sw))
                        {
                           proposal = w;
                           break;
                        }
                     }
                     if (i % 4 == 3)
                     {
                        if (!gameBoard.Contains(e) && !gameBoard.Contains(ne) && !gameBoard.Contains(se))
                        {
                           proposal = e;
                           break;
                        }
                     }
                  }

                  if (proposal != "")
                  {
                     if (proposedBoard.ContainsKey(proposal))
                     {
                        proposedBoard[proposal].Add(elf);
                     }
                     else
                     {
                        List<string> theseElves = new List<string>();
                        theseElves.Add(elf);
                        proposedBoard.Add(proposal, theseElves);
                     }
                  }
                  else
                  {
                     notMovingElves.Add(elf);
                  }
               }
            }

            if (proposedBoard.Count == 0)
            {
               break;
            }

            // Now build our new game board
            gameBoard.Clear();

            foreach (string elf in notMovingElves)
            {
               gameBoard.Add(elf);
            }

            foreach (KeyValuePair<string, List<string>> kvp in proposedBoard)
            {
               if (kvp.Value.Count > 1)
               {
                  // No-one moves!
                  foreach (string elf in kvp.Value)
                  {
                     gameBoard.Add(elf);
                  }
               }
               else
               {
                  gameBoard.Add(kvp.Key);
               }
            }

            Console.WriteLine(string.Format("Completed round {0}", round));
            //DrawGameBoard(gameBoard);
            //Console.ReadLine();

            round++;

            if (round == 10)
            {
               int minX, minY, maxX, maxY;
               GetMinMaxFromGameboard(gameBoard, out minX, out minY, out maxX, out maxY);

               int numberOfSpaces = ((maxX - minX + 1) * (maxY - minY + 1)) - gameBoard.Count;

               Console.WriteLine(string.Format("There are {0} empty spaces in the gameboard after round 10.", numberOfSpaces));
            }
         }

         Console.WriteLine(string.Format("No elves moved in round {0}.", round + 1));
         Console.ReadLine();
      }

      private static void GetMinMaxFromGameboard(HashSet<string> gameBoard, out int minX, out int minY, out int maxX, out int maxY)
      {
         minX = Int32.MaxValue;
         minY = Int32.MaxValue;
         maxX = Int32.MinValue;
         maxY = Int32.MinValue;
         foreach (string coordinate in gameBoard)
         {
            int x, y;
            GetCoordsFromString(coordinate, out x, out y);
           
            minX = Math.Min(minX, x);
            minY = Math.Min(minY, y);
            maxX = Math.Max(maxX, x);
            maxY = Math.Max(maxY, y);
         }
      }

      private static void GetCoordsFromString(string coordinate, out int x, out int y)
      {
         Match match = _coordinate.Match(coordinate);

         x = Convert.ToInt32(match.Groups[1].Value);
         y = Convert.ToInt32(match.Groups[2].Value);
      }

      private static string GetStringFromCoords(int x, int y)
      {
         return string.Format("{0},{1}", x, y);
      }

      private static void DrawGameBoard(HashSet<string> gameBoard)
      {
         int minX, minY, maxX, maxY;
         GetMinMaxFromGameboard(gameBoard, out minX, out minY, out maxX, out maxY);

         for (int y = maxY; y >= minY; y--)
         {
            for (int x = minX; x < maxX + 1; x++)
            {
               if (gameBoard.Contains(GetStringFromCoords(x, y)))
                  Console.Write("#");
               else
                  Console.Write(".");
            }
            Console.WriteLine();
         }

      }
   }
}
