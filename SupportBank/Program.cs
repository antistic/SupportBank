using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;


namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "";
            while (!input.Equals("q"))
            {
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                if (!validCommand(input))
                {
                    Console.WriteLine("Command must be 'List All', 'List [Account]' or 'q' to quit");
                }
                else
                {
                    string target = input.Split(' ')[1];
                    switch (target)
                    {
                        case "All":
                            Console.WriteLine("listing all");
                            parseCSV((anything) => true);
                            break;
                        default:
                            // TODO: check if valid account
                            Console.WriteLine("listing the account " + target);
                            // TODO: parse CSV here too
                            break;
                    }
                    Console.WriteLine();
                }
            }
        }

        static bool validCommand(string command)
        {
            string[] split = command.Split(' ');
            if (split.Length == 2 && split[0].Equals("List"))
            {
                // TODO: check if valid account/"All"
                return true;
            }

            return false;
        }

        static void parseCSV(Func<string[], bool> filter)
        {
            // TODO: reference the file somewhere else so that this function is more generic
            using (TextFieldParser parser = new TextFieldParser(@"../../../Transactions2014.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                // TODO: pretty print by finding out max length of each column
                // header
                if (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    foreach (string field in fields)
                    {
                        Console.Write(field + '\t');
                    }
                    Console.WriteLine();
                }

                // rest of csv
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (filter(fields))
                    {
                        foreach (string field in fields)
                        {
                            Console.Write(field + '\t');
                        }
                        Console.WriteLine();
                    }
                }
            }
        }
    }
}
