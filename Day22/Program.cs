using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day22
{
   class Program
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
               int numberOfMovesLeft = move.Moves;
               Direction previousDirection = direction;

               int newX = x;
               int newY = y;
               int lastValidX = x;
               int lastValidY = y;
               int startx = x;
               int starty = y;
               bool wrapThisMove = false;
               bool wrapThisStep = false;
               bool successfulMoveAfterWrap = false;

               if (_debug) Console.WriteLine(string.Format("Moving {0}", move.Moves));
               while (true)
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

                  newX = newX + xOffset;
                  newY = newY + yOffset;

                  // Wrap
                  DoWrap(gameBoardWidth, gameBoardHeight, ref newX, ref newY, ref wrapThisStep, ref direction);
                  if (wrapThisStep)
                  {
                     wrapThisMove = true;
                     successfulMoveAfterWrap = false;
                  }

                  // Move through empty space
                  if ((gameBoard[newX, newY] == 0) || (gameBoard[newX, newY] == ' '))
                  {
                     // Doesn't count as a move
                     // And don't reset if we've wrapped
                     continue;
                  }
                  else if (gameBoard[newX, newY] == '#')
                  {
                     if (wrapThisStep && !successfulMoveAfterWrap)
                     {
                        Console.WriteLine("Got blocked straight after wrap");
                        direction = previousDirection;
                     }
                     break;
                  }
                  else
                  {
                     wrapThisStep = false;
                     successfulMoveAfterWrap = true;
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

               if (wrapThisMove)
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

      private static void DoWrap(int gameBoardWidth, int gameBoardHeight, ref int newX, ref int newY, ref bool wrapping, ref Direction direction)
      {
         int squareSize = gameBoardWidth / 3;
         
         // Moving off the left hand side
         if (newX < 0)
         {
            wrapping = true;
            // Which cube?
            if (newY < squareSize)
            {
               newY = (squareSize * 3) - newY - 1;
               newX = 0;
               direction = Direction.Right;
            }
            else if (newY < (squareSize * 2))
            {
               // Maps to the top 
               newX = newY - squareSize;
               newY = 0;
               direction = Direction.Down;
            }
            else if (newY < (squareSize * 3))
            {
               newY = (squareSize * 3) - newY - 1;
               newX = 0;
               direction = Direction.Right;
            }
            else
            {
               newX = newY - squareSize * 2;
               newY = 0;
               direction = Direction.Down;
            }
         }
         // Wrapping off the right hand side
         if (newX > gameBoardWidth)
         {
            wrapping = true;
            if (newY < squareSize)
            {
               newY = squareSize * 3 - newY - 1;
               newX = squareSize * 2 - 1;
               direction = Direction.Left;
            }
            else if (newY < (squareSize * 2))
            {
               newX = newY + squareSize;
               newY = gameBoardHeight - 1;
               direction = Direction.Up;
            }
            else if (newY < (squareSize * 3))
            {
               newY = (squareSize * 3) - newY - 1;
               newX = gameBoardWidth - 1;
               direction = Direction.Left;
            }
            else
            {
               newX = newY - (squareSize * 2);
               newY = gameBoardHeight - 1;
               direction = Direction.Up;
            }
         }
         // Wrapping off the top
         if (newY < 0)
         {
            wrapping = true;
            if (newX < squareSize)
            {
               newY = (squareSize + newX);
               newX = 0;
               direction = Direction.Right;
            }
            else if (newX < (squareSize * 2))
            {
               newY = ((squareSize * 3) + (newX - squareSize));
               newX = 0;
               direction = Direction.Right;
            }
            else if (newX < (squareSize * 3))
            {
               newY = gameBoardHeight - 1;
               newX = newX - (squareSize * 2);
               direction = Direction.Up;
            }
         }
         // Wrapping off the bottom
         if (newY > gameBoardHeight)
         {
            wrapping = true;
            if (newX < squareSize)
            {
               newX = newX + (squareSize * 2);
               newY = 0;
               direction = Direction.Down;
            }
            else if (newX < (squareSize * 2))
            {
               newY = (squareSize * 3) + (newX - squareSize);
               newX = gameBoardWidth - 1;
               direction = Direction.Left;
            }
            else if (newX < (squareSize * 3))
            {
               newY = newX - squareSize;
               newX = gameBoardWidth - 1;
               direction = Direction.Left;
            }
         }
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
