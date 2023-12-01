using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day16
{
   class Program
   {
      private static int _trimSize = 50000;
      private static bool _debug = true;

      private static int numberOfPeople = 2;
      private static int numberOfSeconds = 26;

      static void Main(string[] args)
      {
         Dictionary<string, Valve> valves = new Dictionary<string, Valve>();

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();
            Regex regex = new Regex(@"^Valve (\w*) has flow rate=(\d*); tunnels? leads? to valves? ((?>\w\w,? ?)*)$");

            while (s != null)
            {
               Match match = regex.Match(s);
               if (match.Success)
               {
                  Valve valve = new Valve()
                  {
                     Name = match.Groups[1].Value,
                     FlowRate = Convert.ToInt32(match.Groups[2].Value)
                  };

                  string[] v = match.Groups[3].Value.Split(',');
                  foreach(string vn in v)
                  {
                     valve.Valves.Add(vn.Trim());
                  }
                  valves.Add(valve.Name, valve);
               }
               else
               {
                  Console.WriteLine(string.Format("Unable to match string '{0}'",s));
               }

               s = t.ReadLine();
            }
         }

         List<State> currentStates = new List<State>();
         State startingState = new State(numberOfPeople);
         startingState.VisitedValves.Add(new string[] { "AA", "AA" });

         currentStates.Add(startingState);

         int counter = 1;
         while (counter < numberOfSeconds)
         {
            Console.WriteLine(string.Format("Running step {0} with {1} plays.", counter, currentStates.Count));

            List<State> optionsForNextTime = new List<State>();

            foreach (State state in currentStates)
            {
               List<string> optionsForFirstPerson = new List<string>();

               // Move the first person
               {
                  string thisValveName = state.VisitedValves[state.VisitedValves.Count - 1][0];
                  bool alreadyOpened = false;

                  if (thisValveName == "OPEN")
                  {
                     State newState = new State(state);
                     thisValveName = state.VisitedValves[state.VisitedValves.Count - 2][0];
                     alreadyOpened = true;
                  }

                  Valve thisValve = valves[thisValveName];

                  // One option is to open the current valve. We indicate this with "OPEN";
                  // Don't bother if the valve rate is zero (although that would probably get filtered anyway)
                  if ((thisValve.FlowRate != 0) && (!alreadyOpened))
                  {
                     optionsForFirstPerson.Add("OPEN");
                  }

                  foreach (string newValve in thisValve.Valves)
                  {
                     // We can still visit places we've already been, but
                     // they won't be included in the flow rate calculation
                     optionsForFirstPerson.Add(newValve);
                  }
               }

               if (numberOfPeople == 2)
               {
                  foreach (string firstPersonOption in optionsForFirstPerson)
                  {
                     // Add a new option for our second person
                     // This could be a loop, but it's hard
                     string thisValveName = state.VisitedValves[state.VisitedValves.Count - 1][1];
                     bool alreadyOpened = false;

                     if (thisValveName == "OPEN")
                     {
                        State newState = new State(state);
                        thisValveName = state.VisitedValves[state.VisitedValves.Count - 2][1];
                        alreadyOpened = true;
                     }

                     Valve thisValve = valves[thisValveName];

                     // One option is to open the current valve. We indicate this with "OPEN";
                     // Don't bother if the valve rate is zero (although that would probably get filtered anyway)
                     if ((thisValve.FlowRate != 0) && (!alreadyOpened))
                     {
                        State newState = new State(state);
                        string[] options = new string[2] { firstPersonOption, "OPEN" };
                        newState.VisitedValves.Add(options);
                        optionsForNextTime.Add(newState);
                     }

                     foreach (string newValve in thisValve.Valves)
                     {
                        // We can still visit places we've already been, but
                        // they won't be included in the flow rate calculation
                        State newState = new State(state);
                        string[] options = new string[2] { firstPersonOption, newValve };
                        newState.VisitedValves.Add(options);
                        optionsForNextTime.Add(newState);
                     }
                  }
               }
               else
               {
                  // Just add all the options
                  foreach(string option in optionsForFirstPerson)
                  {
                     State newState = new State(state);
                     string[] options = new string[1] { option };
                     newState.VisitedValves.Add(options);
                     optionsForNextTime.Add(newState);
                  }
               }

            }

            currentStates = optionsForNextTime;

            // Trim our state list
            if (currentStates.Count > _trimSize)
            {
               currentStates.Sort((one, two) =>
               {
                  return two.TotalFlow(valves, counter, numberOfSeconds) - one.TotalFlow(valves, counter, numberOfSeconds);
               });
               currentStates.RemoveRange(_trimSize, currentStates.Count - _trimSize);
            }

            counter++;
         }
      
         currentStates.Sort((one, two) =>
         {
            return two.TotalFlow(valves, counter, numberOfSeconds) - one.TotalFlow(valves, counter, numberOfSeconds);
         });

         Console.WriteLine(string.Format("Our best flow is {0}.", currentStates[0].TotalFlow(valves,numberOfSeconds, numberOfSeconds)));
         Console.ReadLine();
      }
   }

   class Valve
   {
      public List<string> Valves = new List<string>();
      public string Name;
      public int FlowRate;
   }

   class State
   {

      public int NumberOfPeople;
      public int MostRecentCalculatedFlow;
      public State(int numberOfPeople)
      {
         NumberOfPeople = numberOfPeople;
      }

      public State(State other)
      {
         VisitedValves = new List<string[]>(other.VisitedValves);
         NumberOfPeople = other.NumberOfPeople;
      }

      public List<string[]> VisitedValves = new List<string[]>();
      public int TotalFlow(Dictionary<string,Valve> valves, int minutes, int totalMinutes)
      {
         HashSet<string> openedValves = new HashSet<string>();

         int totalFlow = 0;
         for (int i = 0; i < minutes; i++)
         {
            string[] playsToMake = VisitedValves[i];

            for (int j = 0; j < NumberOfPeople; j++)

            if (playsToMake[j] == "OPEN")
            {
               string valveToOpen = VisitedValves[i - 1][j];
            
               if (!openedValves.Contains(valveToOpen))
               {
                  Valve openedValve = valves[valveToOpen];
                  totalFlow += (totalMinutes - i) * openedValve.FlowRate;
                  openedValves.Add(valveToOpen);
               }
            }
         }

         MostRecentCalculatedFlow = totalFlow;

         return totalFlow;
      }
   }
}
