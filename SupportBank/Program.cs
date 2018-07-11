using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                            break;
                        default:
                            // TODO: check if valid account
                            Console.WriteLine("listing the account " + target);
                            break;
                    }
                    Console.WriteLine();
                }
            }

            C
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
    }
}
