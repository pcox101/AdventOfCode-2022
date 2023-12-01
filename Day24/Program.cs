using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day24
{
   class Program
   {
      static void Main(string[] args)
      {
         List<Blizzard> gameBoard = new List<Blizzard>();
         int startingX;
         int startingY;
         int endingX;
         int endingY;

         int gameBoardHeight = 0;
         int gameBoardWidth = 0;
         
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            List<string> entireFile = new List<string>();

            string s = t.ReadLine();

            while (s != null)
            {
               entireFile.Add(s);

               gameBoardWidth = s.Length - 2;
               gameBoardHeight++;

               s = t.ReadLine();
            }
            gameBoardHeight -= 2;

            startingX = entireFile[0].IndexOf('.') - 1;
            endingX = entireFile[entireFile.Count - 1].IndexOf('.') - 1;
            startingY = gameBoardHeight;
            endingY = -1;

            for (int y = entireFile.Count; y > 2; y--)
            {
               string l = entireFile[y - 2];
               for (int x = 1; x < gameBoardWidth + 1; x++)
               {
                  if (l[x] != '.')
                  {
                     Blizzard blizzard = new Blizzard(x - 1, gameBoardHeight - y + 2, l[x], gameBoardHeight, gameBoardWidth);
                     gameBoard.Add(blizzard);
                  }
               }
            }
         }

         int round = 0;
         //Console.WriteLine("Starting Position:");
         //DrawGameBoard(gameBoard, round, gameBoardHeight, gameBoardWidth, startingX, startingY, endingX, endingY);

         List<string> movesForThisTime = new List<string>();
         movesForThisTime.Add(string.Format("{0},{1}", startingX, startingY));

         bool done = false;
         bool gettingBack = false;
         bool gotThereAndBack = false;

         while (!done)
         {
            List<string> movesForNextTime = new List<string>();
            Dictionary<string, char> blizzardLocations = BuildDictionaryOfBlizzardLocations(gameBoard, round + 1);

            foreach(string move in movesForThisTime)
            {
               string[] xy = move.Split(',');
               int x = Convert.ToInt32(xy[0]);
               int y = Convert.ToInt32(xy[1]);

               // Check we can finish (if we need to)
               if ((x == endingX) && (y == 0) && ((!gettingBack) || gotThereAndBack))
               {
                  Console.WriteLine(string.Format("Got there in {0} rounds", round + 1));
                  movesForNextTime.Clear();
                  movesForNextTime.Add(string.Format("{0},{1}",endingX, endingY));
                  gettingBack = true;
                  if (gotThereAndBack)
                     done = true;
                  break;
               }
               if ((x == startingX) && (y == startingY - 1) && gettingBack)
               {
                  Console.WriteLine(string.Format("Got back in {0} rounds", round + 1));
                  movesForNextTime.Clear();
                  movesForNextTime.Add(string.Format("{0},{1}", startingX, startingY));
                  gotThereAndBack = true;
                  gettingBack = false;
                  break;
               }

               string stay = string.Format("{0},{1}", x, y);
               string n = string.Format("{0},{1}", x, y + 1);
               string e = string.Format("{0},{1}", x + 1, y);
               string s = string.Format("{0},{1}", x, y - 1);
               string w = string.Format("{0},{1}", x - 1, y);

               if (((y != gameBoardHeight - 1) || (gettingBack && (x == startingX)))
                  && (y != startingY)
                  && !blizzardLocations.ContainsKey(n)
                  && !movesForNextTime.Contains(n))
                  movesForNextTime.Add(n);

               if ((y != 0) && (y != endingY) && !blizzardLocations.ContainsKey(s) && !movesForNextTime.Contains(s))
                  movesForNextTime.Add(s);

               if ((x != 0) && (y != endingY) && !blizzardLocations.ContainsKey(w) && !movesForNextTime.Contains(w))
                  movesForNextTime.Add(w);

               if ((x != gameBoardWidth - 1) && (y != startingY) && !blizzardLocations.ContainsKey(e) && !movesForNextTime.Contains(e))
                  movesForNextTime.Add(e);

               if (!blizzardLocations.ContainsKey(move) && !movesForNextTime.Contains(move))
                  movesForNextTime.Add(move);

            }

            Console.WriteLine(string.Format("Completed round {0} with {1} moves for next time.", round, movesForNextTime.Count));
            movesForThisTime = movesForNextTime;
            
            round++;
         }

         Console.WriteLine(string.Format("Reached the end after {0} rounds.", round));
         Console.ReadLine();
      }

      private static void DrawGameBoard(List<Blizzard> gameBoard,
                                       int round,
                                       int gameBoardHeight,
                                       int gameBoardWidth,
                                       int startingX, int startingY,
                                       int endingX, int endingY)
      {
         Dictionary<string, char> blizzards = BuildDictionaryOfBlizzardLocations(gameBoard, round);

         for (int y = gameBoardHeight; y >= -1; y--)
         {
            for (int x = -1; x <= gameBoardWidth; x++)
            {
               if (x == -1)
               {
                  Console.Write("#");
                  continue;
               }
               if (x == gameBoardWidth)
               {
                  Console.Write("#");
                  continue;
               }
               if ((x == startingX) && (y == startingY))
               {
                  Console.Write("S");
                  continue;
               }
               if ((x == endingX) && (y == endingY))
               {
                  Console.Write("E");
                  continue;
               }
               if (y == startingY)
               {
                  Console.Write("#");
                  continue;
               }
               if (y == endingY)
               {
                  Console.Write("#");
                  continue;
               }

               string s = string.Format("{0},{1}", x, y);
               if (blizzards.ContainsKey(s))
               {
                  Console.Write(blizzards[s].ToString());
               }
               else
               {
                  Console.Write(" ");
               }
            }
            Console.WriteLine();
         }
      }

      private static Dictionary<string, char> BuildDictionaryOfBlizzardLocations(List<Blizzard> gameBoard, int round)
      {
         Dictionary<string, char> blizzards = new Dictionary<string, char>();
         foreach (Blizzard blizzard in gameBoard)
         {
            int x, y;
            blizzard.GetLocation(round, out x, out y);
            string s = string.Format("{0},{1}", x, y);
            if (blizzards.ContainsKey(s))
            {
               blizzards[s] = 'X';
            }
            else
            {
               blizzards.Add(s, blizzard.direction);
            }
         }

         return blizzards;
      }
   }

   class Blizzard
   {
      public Blizzard(int x, int y, char directionChar, int gameBoardHeight, int gameBoardWidth)
      {
         startingX = x;
         startingY = y;
         direction = directionChar;
         GameBoardHeight = gameBoardHeight;
         GameBoardWidth = gameBoardWidth;
      }
      public int startingX;
      public int startingY;
      public char direction;
      int GameBoardHeight;
      int GameBoardWidth;

      public void GetLocation(int round, out int currentX, out int currentY)
      {
         switch (direction)
         {
            case '^':
               currentX = startingX;
               currentY = (startingY + round) % GameBoardHeight;
               break;
            case '>':
               currentY = startingY;
               currentX = (startingX + round) % GameBoardWidth;
               break;
            case 'v':
               currentX = startingX;
               currentY = (startingY - round);
               while (currentY < 0)
                  currentY += GameBoardHeight;
               break;
            case '<':
               currentY = startingY;
               currentX = (startingX - round);
               while (currentX < 0)
                  currentX += GameBoardWidth;
               break;
            default:
               currentX = 0;
               currentY = 0;
               break;
         }
      }
   }
}
