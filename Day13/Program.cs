using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Day13
{
   class Program
   {
      static void Main(string[] args)
      {
         List<Packet> completeListOfPackets = new List<Packet>();
         completeListOfPackets.Add(ParsePacket("[[6]]"));
         completeListOfPackets.Add(ParsePacket("[[2]]"));

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();
            int index = 1;
            int sum = 0;

            while (s != null)
            {
               string leftPacketValue = s;
               Packet leftPacket = ParsePacket(s);
               
               s = t.ReadLine();
               string rightPacketValue = s;
               Packet rightPacket = ParsePacket(s);

               bool? result = PacketsInRightOrder(leftPacket, rightPacket);

               if (result == true)
               {
                  Console.WriteLine(string.Format("Packet {0} is in right order.", index));
                  sum += index;
               }
               else
               {
                  Console.WriteLine(string.Format("Packet {0} is NOT in right order.", index));
               }

               // add the packets to our list
               completeListOfPackets.Add(leftPacket);
               completeListOfPackets.Add(rightPacket);

               index++;
               // Move to the next packet
               s = t.ReadLine();
               if (s != null)
                  s = t.ReadLine();
            }

            Console.WriteLine(string.Format("Sum of indices is {0}.", sum));
         }

         // Sort our packets
         completeListOfPackets.Sort((left, right) =>
         {
            bool? b = PacketsInRightOrder(left, right);
            if (b == true)
               return -1;
            else
               return 1;
         });

         int decoderKey = 1;
         Regex dividerPacket1 = new Regex(@"^\[*6\]*$");
         Regex dividerPacket2 = new Regex(@"^\[*2\]*$");

         Console.WriteLine("Full list of packets:");
         int i = 1;
         foreach(Packet packet in completeListOfPackets)
         {
            string content = packet.ToString();
            Console.WriteLine(packet.ToString());
            if (dividerPacket1.IsMatch(content))
               decoderKey *= i;
            if (dividerPacket2.IsMatch(content))
               decoderKey *= i;
            i++;
         }

         Console.WriteLine(string.Format("DecoderKey is {0}.", decoderKey));

      }

      private static Packet ParsePacket(string content)
      {
         Stack<Packet> packetStack = new Stack<Packet>();

         Packet topLevelPacket = new Packet();
         
         // Top level packets are always lists
         topLevelPacket.packet = PacketType.List;
         topLevelPacket.list = new List<Packet>();
         packetStack.Push(topLevelPacket);

         int currentNumber = -1;
         
         int index = 1;
         while (index < content.Length - 1)
         { 
            if (content[index] == '[')
            {
               // Create a new list packet, add it to the item that's on the top
               // of the stack and push it to the stack
               Packet newPacket = new Packet();
               newPacket.packet = PacketType.List;
               newPacket.list = new List<Packet>();

               packetStack.Peek().list.Add(newPacket);
               packetStack.Push(newPacket);

            }
            else if (content[index] == ']')
            {
               // ending a list (and possibly a number)
               if (currentNumber != -1)
               {
                  Packet newPacket = new Packet();
                  newPacket.Value = currentNumber;
                  newPacket.packet = PacketType.Integer;
                  packetStack.Peek().list.Add(newPacket);
                  currentNumber = -1;
               }
               {
                  packetStack.Pop();
               }
            }
            else if (content[index] == ',')
            {
               // ending a number or adding the list (do nothing)
               if (currentNumber != -1)
               {
                  Packet newPacket = new Packet();
                  newPacket.Value = currentNumber;
                  newPacket.packet = PacketType.Integer;
                  packetStack.Peek().list.Add(newPacket);
                  currentNumber = -1;
               }
            }
            else
            {
               if (currentNumber == -1)
                  currentNumber = 0;
               currentNumber = currentNumber * 10 + Convert.ToInt32(content[index].ToString());
            }

            index++;
         }

         // add the final number
         if (currentNumber != -1)
         {
            Packet newPacket = new Packet();
            newPacket.Value = currentNumber;
            newPacket.packet = PacketType.Integer;
            packetStack.Peek().list.Add(newPacket);
            currentNumber = -1;
         }

         return topLevelPacket;
      }

      private static bool? PacketsInRightOrder(Packet leftPacket, Packet rightPacket)
      {
         //Console.WriteLine(string.Format("Comparing {0} to {1}", leftPacket.ToString(), rightPacket.ToString()));
         // if both values are integers, the lower integer should come first
         if ((leftPacket.packet == PacketType.Integer) && (rightPacket.packet == PacketType.Integer))
         {
            if (leftPacket.Value < rightPacket.Value)
            {
               //Console.WriteLine("Left packet is smaller so inputs are in the RIGHT order");
               return true;
            }
            else if (leftPacket.Value > rightPacket.Value)
            {
               //Console.WriteLine("Right packet is smaller so inputs are in the WRONG order");
               return false;
            }
            else
            {
               return null;
            }
         }

         // One or other is a value.
         if ((leftPacket.packet == PacketType.Integer) || (rightPacket.packet == PacketType.Integer))
         {
            if (leftPacket.packet == PacketType.Integer)
               leftPacket.ConvertToList();
            else
               rightPacket.ConvertToList();
         }

         
         // if both values are lists, then compare each value of the lists
         if ((leftPacket.packet == PacketType.List) && (rightPacket.packet == PacketType.List))
         {
            // Get each value of the packet and recurse
            int index = 0;
            while (true)
            {
               if ((index > rightPacket.list.Count - 1) && (index > leftPacket.list.Count - 1))
               {
                  // both sides ran out at the same time
                  return null;
               }
               else if (index > rightPacket.list.Count - 1)
               {
                  //Console.WriteLine("Right side ran out of items so inputs are in the WRONG order");
                  // right list ran out first
                  return false;
               }
               else if (index > leftPacket.list.Count - 1)
               {
                  //Console.WriteLine("Left side ran out of items so inputs are in the RIGHT order");
                  // left list ran out first
                  return true;
               }
               else
               {
                  bool? comparison = PacketsInRightOrder(leftPacket.list[index], rightPacket.list[index]);

                  if (comparison != null)
                  {
                     return comparison;
                  }
               }
               index++;
            }
         }

         return null;
      }
   }

   enum PacketType
   {
      List,
      Integer
   }
   class Packet
   {
      public PacketType packet { get; set; }
      public List<Packet> list { get; set; }
      public int Value { get; set; }
      public void ConvertToList()
      {
         packet = PacketType.List;
         list = new List<Packet>();
         Packet newPacket = new Packet();
         newPacket.packet = PacketType.Integer;
         newPacket.Value = this.Value;
         list.Add(newPacket);
      }
      public override string ToString()
      {
         if (packet == PacketType.Integer)
            return Value.ToString();
         else
         {
            StringBuilder sb = new StringBuilder("[");
            bool removeLastComma = false;
            foreach (Packet item in list)
            {
               sb.Append(item.ToString());
               sb.Append(",");
               removeLastComma = true;
            }
            if (removeLastComma) 
               sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
         }   
      }
   }
}
