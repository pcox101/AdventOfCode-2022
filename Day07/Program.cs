using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day07
{
   class Program
   {
      static void Main(string[] args)
      {
         DirectoryEntry topLevel = new DirectoryEntry("dir ROOT", null);

         using (FileStream fs = new FileStream("input.txt", FileMode.Open))
         {
            TextReader t = new StreamReader(fs);

            string s = t.ReadLine();
            DirectoryEntry currentDirectory = topLevel;
            while (s != null)
            {
               // Build our tree
               if (s.StartsWith("$"))
               {
                  string[] commandLine = s.Split(' ');
                  if (commandLine[1] == "cd")
                  {
                     if (commandLine[2] == "..")
                     {
                        currentDirectory = currentDirectory.Parent;
                     }
                     else if (commandLine[2] == "/")
                     {
                        currentDirectory = topLevel;
                     }
                     else
                     {
                        currentDirectory = (DirectoryEntry)currentDirectory.ChildEntries[commandLine[2]];
                     }
                  }
                  else if (commandLine[1] == "ls")
                  {
                     // Good
                  }
                  else
                  {
                     throw new Exception("Unknown command");
                  }
               }
               else
               {
                  // Must be an entry
                  if (s.StartsWith("dir"))
                  {
                     DirectoryEntry entry = new DirectoryEntry(s, currentDirectory);
                     currentDirectory.ChildEntries.Add(entry.Name, entry);
                  }
                  else
                  {
                     FileEntry entry = new FileEntry(s);
                     currentDirectory.ChildEntries.Add(entry.Name, entry);
                  }
               }
               s = t.ReadLine();
            }
         }

         // Now we have built our tree, find all entries (other than top level)
         // with size greater than 10000
         int sum = 0;
         DirectoryEntry entryToDelete = topLevel;

         int requiredSpace = 30000000 - (70000000 - topLevel.Size);

         sum = RecurseChildren(topLevel, requiredSpace, ref entryToDelete) ;

         Console.WriteLine(string.Format("Total is {0}", sum));
         Console.WriteLine(string.Format("Directory to delete has name {0} of size {1}", entryToDelete.Name, entryToDelete.Size));
      }
      static private int RecurseChildren(DirectoryEntry entry, int requiredSpace, ref DirectoryEntry entryToDelete)
      {
         int sum = 0;
         // This folder should already be included, so see if any child folders match the criteria
         foreach(KeyValuePair<string, FileSystemEntry> childEntry in entry.ChildEntries)
         {
            if (childEntry.Value is DirectoryEntry)
            {
               DirectoryEntry thisEntry = (DirectoryEntry)(childEntry.Value);
               if (thisEntry.Size <= 100000)
               {
                  sum += thisEntry.Size;
               }

               if (thisEntry.Size >= requiredSpace)
               {
                  Console.WriteLine(string.Format("Directory {0} is big enough.", thisEntry.Name));
                  if (entryToDelete.Size > thisEntry.Size)
                  {
                     Console.WriteLine(string.Format("New lowest {0} with size {1}", thisEntry.Name, thisEntry.Size));
                     entryToDelete = thisEntry;
                  }
               }

               sum += RecurseChildren((DirectoryEntry)childEntry.Value, requiredSpace, ref entryToDelete);
            }
         }
         return sum;
      }
   }


   abstract class FileSystemEntry
   {
      public abstract int Size
      {
         get;
         set;
      }
      public string Name { get; protected set; }
   }

   class FileEntry : FileSystemEntry
   {
      public FileEntry(string nameAndSize)
      {
         string[] parse = nameAndSize.Split(' ');
         Size = Convert.ToInt32(parse[0]);
         Name = parse[1];
      }
      public override int Size
      {
         get; set;
      }
   }

   class DirectoryEntry : FileSystemEntry
   {
      public DirectoryEntry(string fullLine, DirectoryEntry parent)
      {
         string[] parse = fullLine.Split(' ');
         Name = parse[1];
         Parent = parent;
      }
      public override int Size
      {
         get
         {
            int sum = 0;
            foreach(KeyValuePair<string, FileSystemEntry> entry in ChildEntries)
            {
               sum += entry.Value.Size;
            }
            return sum;
         }
         set
         {
            throw new Exception();
         }
      }

      public Dictionary<string, FileSystemEntry> ChildEntries = new Dictionary<string,FileSystemEntry>();

      public DirectoryEntry Parent { get; private set; }
   }
}
