using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day17
{
   class Program
   {
      static void Main(string[] args)
      {

         byte[] gameBoard = new byte[1000];
         List<Sprite> sprites = new List<Sprite>()
         {
            new Sprite1(),
            new Sprite2(),
            new Sprite3(),
            new Sprite4(),
            new Sprite5()
         };
         List<int> windOffsets = new List<int>();

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            for (int i = 0; i < s.Length; i++)
            {
               if (s[i] == '<')
               {
                  windOffsets.Add(-1);
               }
               else
               {
                  windOffsets.Add(1);
               }
            }
         }

         int currentSprite = 0;
         int currentWindOffset = 0;
         int currentY = 3;
         long currentYOffset = 0;
         long numberOfRocks = 1;
         int topOfHighestSprite = 0;
         bool droppedARock = true;

         Dictionary<string, (long rocks, long height)> seen = new Dictionary<string, (long, long)>();
         
         //Console.WriteLine("New Sprite:");
         //DrawGameBoard(gameBoard, topOfHighestSprite, sprites[currentSprite], currentY);

         // Start the game loop
         long rocksToLoop = 1000000000000;
         while (numberOfRocks <= rocksToLoop)
         {

            // See whether we have a loop
            // Is there a pattern?
            if ((droppedARock) && (topOfHighestSprite > 100))
            {
               //DrawGameBoard(gameBoard, topOfHighestSprite, sprites[currentSprite], currentY);
               StringBuilder sb = new StringBuilder();
               for (int y = topOfHighestSprite; y > topOfHighestSprite - 50; y--)
               {
                  sb.Append(gameBoard[y].ToString());
               }

               string key = sb.ToString();
               if (seen.TryGetValue(key, out var cache))
               {
                  Console.WriteLine("Last saw this pattern at height {0} with rocks {1}, currently at height {2} with rocks {3}",
                     cache.height, cache.rocks, topOfHighestSprite + currentYOffset, numberOfRocks);

                  // advance
                  // What will the numberOfRocks be after we're done?
                  long numberOfRocksAfter = numberOfRocks + numberOfRocks - cache.rocks;

                  if (numberOfRocksAfter < rocksToLoop)
                  {
                     numberOfRocks += (numberOfRocks - cache.rocks);
                     currentYOffset += (topOfHighestSprite + currentYOffset - cache.height);
                     continue;
                  }
                  else
                  {
                     // It'll push us past the end, just clear the cache and start dropping
                     // rocks again
                     seen.Clear();
                  }
               }
               else
               {
                  seen.Add(key, (numberOfRocks, topOfHighestSprite + currentYOffset));
               }
            }

            // Move this sprite left/right
            if (!sprites[currentSprite].IsCollision(gameBoard, windOffsets[currentWindOffset], currentY))
            {
               sprites[currentSprite].MoveByOffset(windOffsets[currentWindOffset]);
            }

            currentWindOffset++;
            if (currentWindOffset > windOffsets.Count() - 1)
            {
               currentWindOffset = 0;
            }

            // Move this sprite down
            if ((currentY > 0) && (!sprites[currentSprite].IsCollision(gameBoard, 0, currentY - 1)))
            {
               droppedARock = false;
               currentY--;
            }
            else
            {
               droppedARock = true;
               // Sprite stops. 
               int topOfThisSprite = AddSpriteToGameBoard(gameBoard, sprites[currentSprite], currentY);
               topOfHighestSprite = Math.Max(topOfThisSprite, topOfHighestSprite);

               // Get the next one.
               currentSprite++;
               if (currentSprite >= sprites.Count)
               {
                  currentSprite = 0;
               }
               sprites[currentSprite].ResetSprite();
               numberOfRocks++;
               currentY = topOfHighestSprite + 3;

               // Check and if necessary reset gameboard
               if (currentY > 990)
               {
                  currentY -= 500;
                  topOfHighestSprite -= 500;
                  currentYOffset += 500;
                  for (int y = 0; y < 500; y++)
                  {
                     gameBoard[y] = gameBoard[y + 500];
                  }
                  for (int y = 500; y < 1000; y++)
                  {
                     gameBoard[y] = 0;
                  }
               }

               //Console.WriteLine("New Sprite:");
               //DrawGameBoard(gameBoard, topOfHighestSprite, sprites[currentSprite], currentY);

               if ((numberOfRocks % 1000000) == 0)
               {
                  //Console.WriteLine("New Sprite:");
                  //DrawGameBoard(gameBoard, topOfHighestSprite, sprites[currentSprite], currentY);
                  //Console.WriteLine(string.Format("Top of highest sprite {0}", topOfHighestSprite));
                  Console.WriteLine(string.Format("Sprite number {0} - {1} to go.", numberOfRocks, 1000000000000 - numberOfRocks));
               }
            }

            //DrawGameBoard(gameBoard, topOfHighestSprite, sprites[currentSprite], currentY);
         }

         //DrawGameBoard(gameBoard, sprites[currentSprite], currentX, currentY);
         Console.WriteLine("The tower of rocks is {0} rows high", topOfHighestSprite + currentYOffset);

      }



      private static int AddSpriteToGameBoard(byte[] gameBoard, Sprite sprite, int spriteY)
      {
         for (int y = 0; y < sprite.Height; y++)
         {
            gameBoard[y + spriteY] = (byte)(sprite.Matrix[y] | gameBoard[y + spriteY]);
         }

         return spriteY + sprite.Height;
      }

      private static void DrawGameBoard(byte[] gameBoard, int topOfGameBoard, Sprite sprite, int spriteY)
      {
         int bottomOfSpriteY = spriteY;
         int topOfSpriteY = spriteY + sprite.Height;
         for (int y = topOfGameBoard + 7; y >= 0; y--)
         {
            Console.Write("|");
            for (byte x = 7; x > 0; x--)
            {
               byte mask = (byte)Math.Pow(2, x - 1);
               if ((gameBoard[y] & mask) != 0)
               {
                  Console.Write("X");
               }
               else if ((y >= bottomOfSpriteY) && (y < topOfSpriteY))
               {
                  if ((sprite.Matrix[y - bottomOfSpriteY] & mask) != 0)
                     Console.Write("@");
                  else
                     Console.Write(" ");

               }
               else
                  Console.Write(" ");
            }
            Console.WriteLine("|");
         }
         Console.WriteLine("+-------+");
      }
   }

   abstract class Sprite
   {
      public byte[] Matrix = new byte[4];
      public int Height = 0;
      public abstract void ResetSprite();

      internal bool IsCollision(byte[] gameBoard, int xOffset, int yPos)
      {
         uint spriteVal = (uint)((Matrix[0] << 24) + (Matrix[1] << 16) + (Matrix[2] << 8) + Matrix[3]);
         uint gameVal = (uint)((gameBoard[yPos] << 24) + (gameBoard[yPos + 1] << 16) + (gameBoard[yPos + 2] << 8) + gameBoard[yPos + 3]);

         if ((xOffset == -1) && ((spriteVal & 0b01000000010000000100000001000000) != 0))
         {
            return true;
         }
         if ((xOffset == 1) && ((spriteVal & 0b000001000000010000000100000001) != 0))
         {
            return true;
         }

         if (xOffset == -1)
         {
            spriteVal = (uint)(spriteVal << 1);
         }
         else if (xOffset == 1)
         {
            spriteVal = (uint)(spriteVal >> 1);
         }

         if ((spriteVal & gameVal) != 0)
         {
            return true;
         }

         return false;
      }
      internal void MoveByOffset(int xOffset)
      {
         for (int y = 0; y < Height; y++)
         {
            if (xOffset == -1)
            {
               Matrix[y] = (byte)(Matrix[y] << 1);
            }
            else
            {
               Matrix[y] = (byte)(Matrix[y] >> 1);
            }
         }
      }
   }

   class Sprite1 : Sprite
   {
      public Sprite1()
      {
         ResetSprite();
      }
      public override void ResetSprite()
      {
         Matrix[3] = 0b0000000;
         Matrix[2] = 0b0000000;
         Matrix[1] = 0b0000000;
         Matrix[0] = 0b0011110;
         Height = 1;
      }
   }
   class Sprite2 : Sprite
   {
      public Sprite2()
      {
         ResetSprite();
      }
      public override void ResetSprite()
      {
         Matrix[3] = 0b0000000;
         Matrix[2] = 0b0001000;
         Matrix[1] = 0b0011100;
         Matrix[0] = 0b0001000;
         Height = 3;
      }
   }
   class Sprite3 : Sprite
   {
      public Sprite3()
      {
         ResetSprite();
      }
      public override void ResetSprite()
      {
         Matrix[3] = 0b0000000;
         Matrix[2] = 0b0000100;
         Matrix[1] = 0b0000100;
         Matrix[0] = 0b0011100;
         Height = 3;
      }
   }
   class Sprite4 : Sprite
   {
      public Sprite4()
      {
         ResetSprite();
      }
      public override void ResetSprite()
      {
         Matrix[3] = 0b0010000;
         Matrix[2] = 0b0010000;
         Matrix[1] = 0b0010000;
         Matrix[0] = 0b0010000;
         Height = 4;
      }
   }

   class Sprite5 : Sprite
   {
      public Sprite5()
      {
         ResetSprite();
      }
      public override void ResetSprite()
      {
         Matrix[3] = 0b0000000;
         Matrix[2] = 0b0000000;
         Matrix[1] = 0b0011000;
         Matrix[0] = 0b0011000;
         Height = 2;
      }
   }
}
