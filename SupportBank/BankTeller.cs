using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SupportBank.Banking;

namespace SupportBank
{
    class BankTeller
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static private Bank bank = new Bank();

        public void StartCommandLineInterface()
        {
            string input = "";
            while (!input.Equals("q"))
            {
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                if (!ValidCommand(input))
                {
                    logger.Debug("Invalid input '" + input + "'");
                    Console.WriteLine("Command must be 'Import [Filename]', 'List All', 'List [Account]' or 'q' to quit");
                }
                else
                {
                    Console.WriteLine();
                    string[] command = input.Split(new[] { ' ' }, 2);
                    switch (command[0])
                    {
                        case "Import":
                            Import(command[1]);
                            break;
                        case "Export":
                            Export(command[1]);
                            break;
                        case "List":
                            List(command[1]);
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine();
                }
            }

            logger.Info("Quit Program by 'q' command");
        }
        
        private static bool ValidCommand(string input)
        {
            if (input.Equals("q")) return true;

            string command = input.Split(new[] { ' ' }, 2)[0];
            switch (command)
            {
                case "List":
                case "Import":
                case "Export":
                    return true;
                default:
                    return false;
            }
        }

        void Import(string fileName)
        {
            bank.ImportFromFile(fileName);
        }

        public void Export(string fileName)
        {
            bank.ExportFromFile(fileName);
        }

        public void List(string query)
        {
            switch (query)
            {
                case "All":
                    bank.PrintAll();
                    break;
                default:
                    bank.PrintAccount(query);
                    break;
            }
        }
    }
}