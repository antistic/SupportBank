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

            logger.Info("Program Started");

            TransactionList data = ParseCSV(@"..\..\..\DodgyTransactions2015.csv");

            // program loop
            string input = "";
            while (!input.Equals("q"))
            {
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                if (!ValidCommand(input))
                {
                    logger.Debug("Invalid input '" + input + "'");
                    Console.WriteLine("Command must be 'List All', 'List [Account]' or 'q' to quit");
                }
                else
                {
                    Console.WriteLine();
                    string item = input.Split(new[] { ' ' }, 2)[1];
                    switch (item)
                    {
                        case "All":
                            logger.Debug("Listing All");
                            Console.WriteLine("Listing All:");
                            data.PrintAll();
                            break;
                        default:
                            logger.Debug("Listing transactions involving '" + item + "'");
                            Console.WriteLine("Listing transactions involving '" + item + "':");
                            data.PrintAccount(item);
                            break;
                    }
                    Console.WriteLine();
                }
            }

            logger.Info("Quit Program by 'q' command");
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

            logger.Debug("Parsing file '" + fileName + "'");
            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                int lineNumber = 0;
                
                if (!parser.EndOfData)
                {
                    // header
                    // TODO?: check header is as expected
                    parser.ReadFields();
                    lineNumber += 1;
                }

                // rest of csv
                // TODO: indicate which field contains the problem
                bool errorsFound = false;
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    lineNumber += 1;

                    try
                    {
                        DateTime date;
                        decimal amount;

                        try
                        {
                            date = DateTime.Parse(fields[0]);
                        }
                        catch (Exception)
                        {
                            throw new FormatException("Invalid Date");
                        }

                        try
                        {
                            amount = decimal.Parse(fields[4]);
                        }
                        catch (Exception)
                        {
                            throw new FormatException("Invalid Amount");
                        }

                        Transaction t = new Transaction(
                            date,
                            fields[1],
                            fields[2],
                            fields[3],
                            amount
                        );

                        data.Add(t);
                    }
                    catch (Exception e)
                    {
                        logger.Error("Error found on line " + lineNumber + " in the CSV:\n" + e);

                        if (!errorsFound)
                        {
                            Console.WriteLine("Errors have been found. These transactions will be ignored.");
                            errorsFound = true;
                        }
                        
                        Console.WriteLine("\t" + lineNumber + ": " + e.Message);
                    }
                }
            }

            return data;
        }
    }
}
