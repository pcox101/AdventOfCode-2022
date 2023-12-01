using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if false
namespace Day22
{
   class Part1
   {
      static bool _debug = false;
      static void Main(string[] args)
      {
         char[,] gameBoard = new char[1000, 1000];
         List<Move> moves = new List<Move>();
         int gameBoardWidth = 0;
         int gameBoardHeight = 0;

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            int row = 0;
            while (s != string.Empty)
            {
               gameBoardWidth = Math.Max(gameBoardWidth, s.Length);
               for (int i = 0; i < s.Length; i++)
               {
                  gameBoard[i, row] = s[i];
               }

               row++;
               s = t.ReadLine();
            }
            gameBoardHeight = row;

            s = t.ReadLine();

            Move currentMove = new Move();
            for (int i = 0; i < s.Length; i++)
            {
               if ((s[i] == 'R') || (s[i] == 'L'))
               {
                  // Save off the current move
                  moves.Add(currentMove);
                  moves.Add(new Move() { Rotation = s[i] });
                  currentMove = new Move();
               }
               else
               {
                  currentMove.Moves = currentMove.Moves * 10;
                  currentMove.Moves += Convert.ToInt32(s[i].ToString()); 
               }
            }
            if (currentMove.Moves > 0)
               moves.Add(currentMove);
         }

         int x, y;
         y = 0;
         x = 0;
         Direction direction = Direction.Right;

         for (int i = 0; i < gameBoardWidth; i++)
         {
            if (gameBoard[i,0] != ' ')
            {
               x = i;
               break;
            }
         }

         // Play the game
         foreach (Move move in moves)
         {
            if (move.Rotation == 'R')
            {
               if (_debug) Console.WriteLine("Rotating R");
               int d = (int)direction;
               d++;
               direction = (Direction)(d % 4);
            }
            else if (move.Rotation == 'L')
            {
               if (_debug) Console.WriteLine("Rotating L");
               int d = (int)direction;
               if (d == 0)
                  d = 3;
               else
                  d--;
               direction = (Direction)(d % 4);
            }
            else
            {
               int xOffset = 0;
               int yOffset = 0;

               switch (direction)
               {
                  case Direction.Right:
                     xOffset = 1;
                     break;
                  case Direction.Down:
                     yOffset = 1;
                     break;
                  case Direction.Left:
                     xOffset = -1;
                     break;
                  case Direction.Up:
                     yOffset = -1;
                     break;
                  default:
                     break;
               }

               int numberOfMovesLeft = move.Moves;

               int newX = x;
               int newY = y;
               int lastValidX = x;
               int lastValidY = y;
               bool wrapping = false;
               int startx = x;
               int starty = y;

               if (_debug) Console.WriteLine(string.Format("Moving {0}", move.Moves));
               wrapping = false;
               while (true)
               {
                  newX = newX + xOffset;
                  newY = newY + yOffset;

                  // Wrap
                  if (newX < 0)
                  {
                     wrapping = true;
                     newX = gameBoardWidth - 1;
                  }
                  if (newX > gameBoardHeight)
                  {
                     wrapping = true;
                     newX = 0;
                  }
                  if (newY < 0)
                  {
                     wrapping = true;
                     newY = gameBoardHeight - 1;
                  }
                  if (newY > gameBoardHeight)
                  {
                     wrapping = true;
                     newY = 0;
                  }

                  // Move through empty space
                  if ((gameBoard[newX,newY] == 0) || (gameBoard[newX,newY] == ' '))
                  {
                     // Doesn't count as a move
                     continue;
                  }
                  else if (gameBoard[newX,newY] == '#')
                  {
                     break;
                  }
                  else
                  {
                     lastValidX = newX;
                     lastValidY = newY;

                     // Can move
                     numberOfMovesLeft--;
                     if (numberOfMovesLeft <= 0)
                     {
                        break;
                     }
                  }
               }

               // Move complete
               x = lastValidX;
               y = lastValidY;

               if (wrapping)
               {
                  if (_debug)
                  {
                     Console.WriteLine(string.Format("After move {0} we wrapped:", move.Moves));
                     Console.WriteLine(string.Format("From {0},{1} to {2},{3}", startx, starty, x, y));
                     DrawGameBoard(gameBoard, x, y, direction, gameBoardWidth, gameBoardHeight, startx, starty);
                     Console.ReadLine();
                  }
               }

               
            }

         }

         if (_debug)
         {
            Console.WriteLine("Final position:");
            DrawGameBoard(gameBoard, x, y, direction, gameBoardWidth, gameBoardHeight, 0, 0);
         }

         int perspective = ((y + 1) * 1000) + ((x + 1) * 4) + (int)direction;
         Console.WriteLine("Perspective value is {0}", perspective);
         Console.ReadLine();
      }

      private static void DrawGameBoard(char[,] gameboard,
                                        int playerX,
                                        int playerY,
                                        Direction direction,
                                        int maxX,
                                        int maxY,
                                        int startX,
                                        int startY)
      {
         for (int y = 0; y < maxY; y++)
         {
            for (int x = 0; x < maxX; x++)
            {
               if ((x == playerX) && (y == playerY))
               {
                  Console.BackgroundColor = ConsoleColor.Red;
                  switch (direction)
                  {
                     case Direction.Right:
                        Console.Write(">");
                        break;
                     case Direction.Down:
                        Console.Write("V");
                        break;
                     case Direction.Left:
                        Console.Write("<");
                        break;
                     case Direction.Up:
                        Console.Write("^");
                        break;
                     default:
                        break;
                  }
                  Console.BackgroundColor = ConsoleColor.Black;
               }
               else if ((x == startX) && (y == startY))
               {
                  Console.BackgroundColor = ConsoleColor.Blue;
                  Console.Write("S");
                  Console.BackgroundColor = ConsoleColor.Black;

               }
               else if (gameboard[x, y] == 0)
                  Console.Write(" ");
               else
                  Console.Write(gameboard[x, y].ToString());
            }

            Console.WriteLine();
         }
      }
   }

   class Move
   {
      public int Moves = 0;
      public char Rotation;
   }

   enum Direction
   {
      Right = 0,
      Down = 1,
      Left = 2,
      Up = 3
   }
}
#endif
