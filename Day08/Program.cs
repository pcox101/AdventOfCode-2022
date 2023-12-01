using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day08
{
   class Program
   {
      static void Main(string[] args)
      {
         int rows = 0, columns  = 0;
         List<string> rawData = new List<string>();

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();
            columns = s.Length;

            while (s != null)
            {
               rawData.Add(s);
               rows++;

               s = t.ReadLine();
            }
         }

         int[,] trees = ParseRawData(rawData, rows, columns);

         int visibleTrees = 0;
         int scenicScore = 0;
         for (int i = 0; i < rows; i++)
         {
            for (int j = 0; j < columns; j++)
            {
               int leftScenic, rightScenic, topScenic, bottomScenic;
               bool leftVisible, rightVisible, topVisible, bottomVisible;
               // Is this one visible?
               leftVisible = VisibleFromLeft(trees, i, j, out leftScenic);
               rightVisible = VisibleFromRight(rows, trees, i, j, out rightScenic);
               topVisible = VisibleFromTop(trees, i, j, out topScenic);
               bottomVisible = VisibleFromBottom(columns, trees, i, j, out bottomScenic);

               if (topVisible || rightVisible || leftVisible || bottomVisible)
               {
                  visibleTrees++;
               }

               int thisScenicScore = leftScenic * rightScenic * topScenic * bottomScenic;

               if (thisScenicScore > scenicScore)
               {
                  scenicScore = thisScenicScore;
                  Console.WriteLine(string.Format("New scenic score of {0}", scenicScore));
               }   
            }
         }

         Console.WriteLine(string.Format("There are {0} visible trees", visibleTrees));
         Console.WriteLine(string.Format("Tree with scenic score of {0} is best", scenicScore));

      }

      private static bool VisibleFromBottom(int columns, int[,] trees, int i, int j, out int scenicScore)
      {
         if (j == columns - 1)
         {
            scenicScore = 0;
            return true;
         }

         scenicScore = 1;
         for (int y = j + 1; y < columns; y++)
         {
            if (trees[i, y] >= trees[i, j])
            {
               return false;
            }
            else
            {
               scenicScore++;
            }
         }
         scenicScore--;

         return true;
      }

      private static bool VisibleFromTop(int[,] trees, int i, int j, out int scenicScore)
      {
         if (i == 0)
         {
            scenicScore = 0;
            return true;
         }

         scenicScore = 1;
         for (int y = j - 1; y >= 0; y--)
         {
            if (trees[i, y] >= trees[i, j])
            {
               return false;
            }
            else
            {
               scenicScore++;
            }
         }

         scenicScore--;
         return true;
      }

      private static bool VisibleFromRight(int rows, int[,] trees, int i, int j, out int scenicScore)
      {
         if (i == rows - 1)
         {
            scenicScore = 0;
            return true;
         }

         scenicScore = 1;
         for (int x = i + 1; x < rows; x++)
         {
            if (trees[x, j] >= trees[i, j])
            {
               return false;
            }
            else
            {
               scenicScore++;
            }
         }

         scenicScore--;
         return true;
      }

      private static bool VisibleFromLeft(int[,] trees, int i, int j, out int scenicScore)
      {
         if (i == 0)
         {
            scenicScore = 0;
            return true;
         }
         
         scenicScore = 1;
         for (int x = i - 1; x >= 0; x--)
         {
            if (trees[x, j] >= trees[i, j])
            {
               return false;
            }
            else
            {
               scenicScore++;
            }
         }

         scenicScore--;
         return true;
      }

      private static int[,] ParseRawData(List<string> rawData, int rows, int columns)
      {
         int[,] result = new int[rows, columns];

         for (int i = 0; i < rows; i++)
         {
            for (int j = 0; j < columns; j++)
            {
               result[i,j] = Convert.ToInt32(Convert.ToString(rawData[i][j]));
            }
         }

         return result;
      }
   }
}
