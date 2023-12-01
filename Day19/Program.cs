using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day19
{
   class Program
   {
      static int _pruneAt = 10000;

      static void Main(string[] args)
      {
         List<BluePrint> bluePrints = new List<BluePrint>();
         
         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();

            while (s != null)
            {
               if (s == "")
               {
                  s = t.ReadLine();
                  continue;
               }

               BluePrint bluePrint = ParseLineToBluePrint(s);

               if (bluePrint == null)
               {
                  // test file
                  s = s + t.ReadLine() + t.ReadLine() + t.ReadLine() + t.ReadLine();
                  bluePrint = ParseLineToBluePrint(s);
               }

               if (bluePrint == null)
               {
                  Console.WriteLine("Failure to parse");
               }

               bluePrints.Add(bluePrint);

               s = t.ReadLine();
            }
         }

         // Play each of the blueprints
         int totalQualityLevel = 0;
         for (int i = 1; i <= bluePrints.Count; i++)
         {
            BluePrint bp = bluePrints[i- 1];
            int numberOfGeodes = PlayBlueprint(bp, 24);
            int qualityLevel = (numberOfGeodes * i);
            Console.WriteLine(string.Format("This blueprint could produce {0} geodes.", numberOfGeodes));
            Console.WriteLine(string.Format("So its quality level is {0}.", qualityLevel));

            totalQualityLevel += qualityLevel;
         }
         Console.WriteLine(string.Format("Total Quality Level: {0}.", totalQualityLevel));

         int largestGeodes = 1;
         for (int i = 1; i <= 3; i++)
         {
            BluePrint bp = bluePrints[i - 1];
            int numberOfGeodes = PlayBlueprint(bp, 32);

            largestGeodes = largestGeodes * numberOfGeodes;
         }
         Console.WriteLine(string.Format("Largest Number of Geodes[3]: {0}.", largestGeodes));

      }

      static int PlayBlueprint(BluePrint bp, int numberOfLoops)
      {
         List<GameStatus> currentlyPlayingStatus = new List<GameStatus>();
         GameStatus initialPosition = new GameStatus();
         initialPosition.OreRobots = 1;
         currentlyPlayingStatus.Add(initialPosition);

         int gameCounter = 0;

         while (gameCounter < numberOfLoops)
         {
            Console.WriteLine(string.Format("Playing round {0} with {1} options.", gameCounter + 1, currentlyPlayingStatus.Count));

            List<GameStatus> optionsForNextTime = new List<GameStatus>();

            foreach (GameStatus gameStatus in currentlyPlayingStatus)
            {
               GameStatus playingOption = gameStatus;
               
               // Move our pending robots to current robots and reset
               playingOption.OreRobots += playingOption.PendingOreRobots;
               playingOption.PendingOreRobots = 0;
               playingOption.ClayRobots += playingOption.PendingClayRobots;
               playingOption.PendingClayRobots = 0;
               playingOption.ObsidianRobots += playingOption.PendingObsidianRobots;
               playingOption.PendingObsidianRobots = 0;
               playingOption.GeodeRobots += playingOption.PendingGeodeRobots;
               playingOption.PendingGeodeRobots = 0;

               // See what we can build (we can only build one thing,
               // but we could build any of them!)
               
               // We can always buy nothing at this point
               // And in fact this could be the only option.
               optionsForNextTime.Add(playingOption);
               
               // Can we buy an ore robot?
               if (bp.oreRobot.CanOneBeBuilt(playingOption))
               {
                  GameStatus newStatus = BuildRobot(playingOption, bp.oreRobot);
                  optionsForNextTime.Add(newStatus);
               }
               // And so on
               if (bp.clayRobot.CanOneBeBuilt(playingOption))
               {
                  GameStatus newStatus = BuildRobot(playingOption, bp.clayRobot);
                  optionsForNextTime.Add(newStatus);
               }
               if (bp.obsidianRobot.CanOneBeBuilt(playingOption))
               {
                  GameStatus newStatus = BuildRobot(playingOption, bp.obsidianRobot);
                  optionsForNextTime.Add(newStatus);
               }
               if (bp.geodeRobot.CanOneBeBuilt(playingOption))
               {
                  GameStatus newStatus = BuildRobot(playingOption, bp.geodeRobot);
                  optionsForNextTime.Add(newStatus);
               }

#if false
               List<GameStatus> tryingThisTime = new List<GameStatus>();
               tryingThisTime.Add(playingOption);
               while (tryingThisTime.Count > 0)
               {
                  GameStatus thisTime = tryingThisTime[0];
                  tryingThisTime.RemoveAt(0);

                  // We can always buy nothing at this point
                  // And in fact this could be the only option.
                  optionsForNextTime.Add(thisTime);

                  // Can we buy an ore robot?
                  if (bp.oreRobot.CanOneBeBuilt(thisTime))
                  {
                     GameStatus newStatus = BuildRobot(thisTime, bp.oreRobot);
                     tryingThisTime.Add(newStatus);
                  }
                  // And so on
                  if (bp.clayRobot.CanOneBeBuilt(thisTime))
                  {
                     GameStatus newStatus = BuildRobot(thisTime, bp.clayRobot);
                     tryingThisTime.Add(newStatus);
                  }
                  if (bp.obsidianRobot.CanOneBeBuilt(thisTime))
                  {
                     GameStatus newStatus = BuildRobot(thisTime, bp.obsidianRobot);
                     tryingThisTime.Add(newStatus);
                  }
                  if (bp.geodeRobot.CanOneBeBuilt(thisTime))
                  {
                     GameStatus newStatus = BuildRobot(thisTime, bp.geodeRobot);
                     tryingThisTime.Add(newStatus);
                  }
               }
#endif

            }

            // We now have a list of options to add for next time. Play each one
            for (int i = 0; i < optionsForNextTime.Count; i++)
            {
               GameStatus option = optionsForNextTime[i];
               option.NumberOfOre += option.OreRobots;
               option.NumberOfClay += option.ClayRobots;
               option.NumberOfObsidian += option.ObsidianRobots;
               option.NumberOfGeodes += option.GeodeRobots;

               optionsForNextTime[i] = option;
            }

            // And set our gamestatus up for next time
            currentlyPlayingStatus = optionsForNextTime;
            gameCounter++;

            currentlyPlayingStatus.Sort();

            Console.WriteLine(string.Format("Finished round {0} with {1} options.", gameCounter + 1, currentlyPlayingStatus.Count));
            Console.WriteLine(string.Format("Best option has {0} geodes.", currentlyPlayingStatus[0].NumberOfGeodes));

            if (currentlyPlayingStatus.Count > _pruneAt)
            {
               currentlyPlayingStatus.RemoveRange(_pruneAt, currentlyPlayingStatus.Count - _pruneAt);
            }
         }

         // Now select the best from our list
         currentlyPlayingStatus.Sort();

         return currentlyPlayingStatus[0].NumberOfGeodes;
      }

      public static GameStatus BuildRobot(GameStatus current, Robot robot)
      {
         GameStatus newStatus = current;
         newStatus.NumberOfOre -= robot.OreRequired;
         newStatus.NumberOfClay -= robot.ClayRequired;
         newStatus.NumberOfObsidian -= robot.ObsidianRequired;
         if (robot is OreRobot)
         {
            newStatus.PendingOreRobots++;
         }
         else if (robot is ClayRobot)
         {
            newStatus.PendingClayRobots++;
         }
         else if (robot is ObsidianRobot)
         {
            newStatus.PendingObsidianRobots++;
         }
         else
         {
            newStatus.PendingGeodeRobots++;
         }
         return newStatus;
      }

      static BluePrint ParseLineToBluePrint(string s)
      {
         Regex regex =
            new Regex(@"^Blueprint \d*:.*Each ore robot costs (\d*) ore.*Each clay robot costs (\d*) ore.*Each obsidian robot costs (\d*) ore and (\d*) clay.*Each geode robot costs (\d*) ore and (\d*) obsidian.*$");

         Match match = regex.Match(s);

         if (!match.Success)
         {
            return null;
         }

         BluePrint bluePrint = new BluePrint();
         bluePrint.oreRobot = new OreRobot()
            { OreRequired = Convert.ToInt32(match.Groups[1].Value) };
         bluePrint.clayRobot = new ClayRobot()
            { OreRequired = Convert.ToInt32(match.Groups[2].Value) };
         bluePrint.obsidianRobot = new ObsidianRobot()
            { OreRequired = Convert.ToInt32(match.Groups[3].Value),
              ClayRequired = Convert.ToInt32(match.Groups[4].Value)
            };
         bluePrint.geodeRobot = new GeodeRobot()
            { OreRequired = Convert.ToInt32(match.Groups[5].Value),
              ObsidianRequired = Convert.ToInt32(match.Groups[6].Value)
         };

         return bluePrint;
      }
   }

   struct GameStatus : IComparable<GameStatus>
   {

      public int NumberOfOre { get; set; }
      public int NumberOfClay { get; set; }
      public int NumberOfObsidian { get; set; }
      public int NumberOfGeodes { get; set; }

      public int OreRobots { get; set; }
      public int ClayRobots { get; set; }
      public int ObsidianRobots { get; set; }
      public int GeodeRobots { get; set; }
      public int PendingOreRobots { get; set; }
      public int PendingClayRobots { get; set; }
      public int PendingObsidianRobots { get; set; }
      public int PendingGeodeRobots { get; set; }

      public int CompareTo(GameStatus other)
      {
         // prefer geodes and their robots, then obsidien etc.
         if (NumberOfGeodes > other.NumberOfGeodes)
            return -1;
         if (other.NumberOfGeodes > NumberOfGeodes)
            return 1;
         if (GeodeRobots > other.GeodeRobots)
            return -1;
         if (other.GeodeRobots > GeodeRobots)
            return 1;
         if (PendingGeodeRobots > other.PendingGeodeRobots)
            return -1;
         if (other.PendingGeodeRobots > PendingGeodeRobots)
            return 1;

         if (NumberOfObsidian > other.NumberOfObsidian)
            return -1;
         if (other.NumberOfObsidian > NumberOfObsidian)
            return 1;
         if (ObsidianRobots > other.ObsidianRobots)
            return -1;
         if (other.ObsidianRobots > ObsidianRobots)
            return 1;
         if (PendingObsidianRobots > other.PendingObsidianRobots)
            return -1;
         if (other.PendingObsidianRobots > PendingObsidianRobots)
            return 1;

         if (NumberOfClay > other.NumberOfClay)
            return -1;
         if (other.NumberOfClay > NumberOfClay)
            return 1;
         if (ClayRobots > other.ClayRobots)
            return -1;
         if (other.ClayRobots > ClayRobots)
            return 1;
         if (PendingClayRobots > other.PendingClayRobots)
            return -1;
         if (other.PendingClayRobots > PendingClayRobots)
            return 1;

         if (NumberOfOre > other.NumberOfOre)
            return -1;
         if (other.NumberOfOre > NumberOfOre)
            return 1;
         if (OreRobots > other.OreRobots)
            return -1;
         if (other.OreRobots > OreRobots)
            return 1;
         if (PendingOreRobots > other.PendingOreRobots)
            return -1;
         if (other.PendingOreRobots > PendingOreRobots)
            return 1;


         return 0;
      }
   }

   class BluePrint
   {
      public OreRobot oreRobot { get; set; }
      public ClayRobot clayRobot { get; set; }
      public ObsidianRobot obsidianRobot { get; set; }
      public GeodeRobot geodeRobot { get; set; }
   }

   class Robot
   {
      public int OreRequired { get; set; }
      public int ClayRequired { get; set; }
      public int ObsidianRequired { get; set; }

      public bool CanOneBeBuilt(GameStatus gameStatus)
      {
         if (OreRequired > gameStatus.NumberOfOre)
         {
            return false;
         }
         if (ClayRequired > gameStatus.NumberOfClay)
         {
            return false;
         }
         if (ObsidianRequired > gameStatus.NumberOfObsidian)
         {
            return false;
         }
         return true;
      }
   }

   class OreRobot : Robot
   {
   }

   class ClayRobot : Robot
   {
   }
   class ObsidianRobot : Robot
   {
   }

   class GeodeRobot : Robot
   {
   }
}
