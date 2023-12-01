using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day03
{
   class Program
   {
      static void Main(string[] args)
      {
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            int totalScore = 0;
            int totalElfensetScore = 0;

            string backpackContent = t.ReadLine();

            int counter = 0;
            Backpack[] elfenset = new Backpack[3];

            while (backpackContent != null)
            {
               Backpack backpack = new Backpack(backpackContent);

               Console.WriteLine(backpack.ToString());

               totalScore += backpack.GetCommonItemScore();

               elfenset[counter] = backpack;

               if (counter == 2)
               {
                  // Full elfenset
                  totalElfensetScore += GetElfensetScore(elfenset);
                  counter = 0;
               }
               else
               {
                  counter++;
               }

               backpackContent = t.ReadLine();
            }
      
            Console.WriteLine(string.Format("Common item score is {0}.", totalScore));
            Console.WriteLine(string.Format("Full elfenset score is {0}.", totalElfensetScore));
         }
      }

      private static int GetElfensetScore(Backpack[] backpacks)
      {
         List<string> contents = new List<string>();

         for (int i = 0; i < 3; i++)
         {
            contents.Add(backpacks[i].FullBackpackContent);
         }

         List<char> common = Helper.GetCommonItems(contents);
         int score = Helper.ScoreASetOfItems(common);

         Console.WriteLine(String.Format("Elfenset complete : Common Item {0} with score {1}",
                                    new string(common.ToArray()),
                                    score));

         return score;
      }
   }

   static class Helper
   {
      static string _score = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
      public static int Score(char ch)
      {
         return _score.IndexOf(ch) + 1;
      }
      public static int ScoreASetOfItems(List<char> setOfItems)
      {
         int totalScore = 0;

         // Score the common items
         foreach (char ch in setOfItems)
         {
            totalScore += Helper.Score(ch);
         }

         return totalScore;
      }

      public static List<char> GetCommonItems(List<string> itemSet)
      {
         List<char> commonItems = new List<char>(itemSet[0]);
         
         for (int i = 1; i < itemSet.Count(); i++)
         {
            commonItems = GetCommonItems(new string(commonItems.ToArray()), itemSet[i]);   
         }

         return commonItems;
      }

      public static List<char> GetCommonItems(string item1, string item2)
      {
         List<char> commonItems = new List<char>();

         HashSet<char> itemSet = new HashSet<char>(item1);

         foreach(char ch in item2)
         {
            if (itemSet.Contains(ch) && !commonItems.Contains(ch))
            {
               commonItems.Add(ch);
            }
         }

         return commonItems;

      }
   }
   
   class Backpack
   {
      string _compartment1;
      string _compartment2;
      public string FullBackpackContent
      {
         get
         {
            return _compartment1 + _compartment2;
         }
      }
      
      public Backpack(string contents)
      {
         // split the string into 2
         _compartment1 = contents.Substring(0, contents.Length / 2);
         _compartment2 = contents.Substring(contents.Length / 2, contents.Length / 2);
      }

      public int GetCommonItemScore()
      {
         List<char> commonItems = GetCommonItems();
         return Helper.ScoreASetOfItems(commonItems);
      }

      private List<char> GetCommonItems()
      {
         List<string> compartments = new List<string>();
         compartments.Add(_compartment1);
         compartments.Add(_compartment2);

         return Helper.GetCommonItems(compartments);
      }

      public override string ToString()
      {
         List<char> commonItems = GetCommonItems();
         string common = new string(commonItems.ToArray());
         return string.Format("Backpack:\r\n  Compartment1:{0}\r\n  Compartment2:{1}\r\n  Common Items:{2} with score {3}",
                              _compartment1,
                              _compartment2,
                              common,
                              GetCommonItemScore());
      }
   }
}
