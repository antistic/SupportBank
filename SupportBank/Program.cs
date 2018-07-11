using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace SupportBank
{
    class Program
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            // set up logging
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"..\..\..\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            logger.Log(LogLevel.Info, "Program Started");

            TransactionList data = ParseCSV(@"..\..\..\DodgyTransactions2015.csv");

            // program loop
            string input = "";
            while (!input.Equals("q"))
            {
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                if (!ValidCommand(input))
                {
                    logger.Log(LogLevel.Debug, "Invalid input '" + input + "'");
                    Console.WriteLine("Command must be 'List All', 'List [Account]' or 'q' to quit");
                }
                else
                {
                    Console.WriteLine();
                    string item = input.Split(new[] { ' ' }, 2)[1];
                    switch (item)
                    {
                        case "All":
                            logger.Log(LogLevel.Debug, "Listing All");
                            Console.WriteLine("Listing All:");
                            data.PrintAll();
                            break;
                        default:
                            logger.Log(LogLevel.Debug, "Listing transactions involving '" + item + "'");
                            Console.WriteLine("Listing transactions involving '" + item + "':");
                            data.PrintAccount(item);
                            break;
                    }
                    Console.WriteLine();
                }
            }

            logger.Log(LogLevel.Info, "Quit Program by 'q' command");
        }

        static bool ValidCommand(string command)
        {
            string[] split = command.Split(new[] { ' ' }, 2);
            if (split[0].Equals("List"))
            {
                return true;
            }

            return false;
        }

        static TransactionList ParseCSV(string fileName)
        {
            TransactionList data = new TransactionList();

            logger.Log(LogLevel.Debug, "Parsing file '" + fileName + "'");
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                int index = 0;
                
                if (!parser.EndOfData)
                {
                    // header
                    // TODO?: check header is as expected
                    parser.ReadFields();
                    index += 1;
                }

                // rest of csv
                List<int> errorLines = new List<int>();
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    index += 1;

                    try
                    {
                        Transaction t = new Transaction(
                            DateTime.Parse(fields[0]),
                            fields[1],
                            fields[2],
                            fields[3],
                            decimal.Parse(fields[4])
                        );

                        data.Add(t);
                    }
                    catch (Exception e)
                    {
                        logger.Log(LogLevel.Error, "Error found on line " + index + " in the CSV: " + e);
                        errorLines.Add(index);
                    }
                }

                if (errorLines.Any())
                {
                    Console.Write("Errors have been found on lines ");
                    foreach (int line in errorLines)
                    {
                        Console.Write(line + ", ");
                    }
                    Console.WriteLine("these transactions will be ignored.");
                }
            }

            return data;
        }
    }
}
