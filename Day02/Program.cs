using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day02
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            int totalScore = 0;

            string round = t.ReadLine();
            while (round != null)
            {
               string[] moves = round.Split(' ');

               Move theirMove = new Move(moves[0], true);
               //Move myMove = new Move(moves[1], false);
               WinLoseDraw howToPlay = MapFromString(moves[1]);

               Move myMove = new Move(theirMove, howToPlay);

               totalScore += myMove.CalculateMyScore(theirMove);

               round = t.ReadLine();
            }

            Console.WriteLine(string.Format("I scored {0} with this strategy.", totalScore));

         }

         WinLoseDraw MapFromString(string map)
         {
            switch (map)
            {
               case "X":
                  return WinLoseDraw.Lose;
               case "Y":
                  return WinLoseDraw.Draw;
               default:
                  return WinLoseDraw.Win;
            }
         }
      }

   }

   enum RockPaperScissors
   {
      Rock,
      Paper,
      Scissors
   }

   enum WinLoseDraw
   {
      Win,
      Lose,
      Draw
   }

   class Move
   {
      public Move(string map, bool firstColumn)
      {
         if (firstColumn)
         {
            switch (map)
            {
               case "A":
                  ThisMove = RockPaperScissors.Rock;
                  break;
               case "B":
                  ThisMove = RockPaperScissors.Paper;
                  break;
               case "C":
                  ThisMove = RockPaperScissors.Scissors;
                  break;
               default:
                  break;
            }
         }
         else
         {
            switch (map)
            {
               case "X":
                  ThisMove = RockPaperScissors.Rock;
                  break;
               case "Y":
                  ThisMove = RockPaperScissors.Paper;
                  break;
               case "Z":
                  ThisMove = RockPaperScissors.Scissors;
                  break;
               default:
                  break;
            }
         }
      }

      public Move(Move theirMove, WinLoseDraw howToPlay)
      {
         switch (howToPlay)
         {
            case WinLoseDraw.Draw:
               ThisMove = theirMove.ThisMove;
               break;
            case WinLoseDraw.Win:
               switch (theirMove.ThisMove)
               {
                  case RockPaperScissors.Rock:
                     ThisMove = RockPaperScissors.Paper;
                     break;
                  case RockPaperScissors.Paper:
                     ThisMove = RockPaperScissors.Scissors;
                     break;
                  case RockPaperScissors.Scissors:
                     ThisMove = RockPaperScissors.Rock;
                     break;
               }
               break;
            case WinLoseDraw.Lose:
               switch (theirMove.ThisMove)
               {
                  case RockPaperScissors.Rock:
                     ThisMove = RockPaperScissors.Scissors;
                     break;
                  case RockPaperScissors.Paper:
                     ThisMove = RockPaperScissors.Rock;
                     break;
                  case RockPaperScissors.Scissors:
                     ThisMove = RockPaperScissors.Paper;
                     break;
               }
               break;
         }
      }
      public RockPaperScissors ThisMove { get; private set; }

      public int CalculateMyScore(Move theirMove)
      {
         int theScore = 0;

         if (theirMove.ThisMove == ThisMove)
         {
            theScore = 3;
         }

         switch (ThisMove)
         {
            case RockPaperScissors.Rock:
               theScore += 1;
               if (theirMove.ThisMove == RockPaperScissors.Scissors)
               {
                  theScore += 6;
               }
               break;
            case RockPaperScissors.Paper:
               theScore += 2;
               if (theirMove.ThisMove == RockPaperScissors.Rock)
               {
                  theScore += 6;
               }
               break;
            case RockPaperScissors.Scissors:
               theScore += 3;
               if (theirMove.ThisMove == RockPaperScissors.Paper)
               {
                  theScore += 6;
               }
               break;
         }

         return theScore;
      }
   }
}
