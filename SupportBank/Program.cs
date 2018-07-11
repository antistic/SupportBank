using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
            TransactionList data = null;

            // set up logging
            var config = new LoggingConfiguration();
            var target = new FileTarget { FileName = @"..\..\..\Logs\SupportBank.log", Layout = @"${longdate} ${level} - ${logger}: ${message}" };
            config.AddTarget("File Logger", target);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, target));
            LogManager.Configuration = config;

            logger.Info("Program Started");

            // program loop
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
                            data = Import(command[1]);
                            if (data != null) Console.WriteLine("File imported.");
                            break;
                        case "List":
                            if (data == null)
                            {
                                Console.WriteLine("You need to import some data first.");
                            }
                            else
                            {
                                switch (command[1])
                                {
                                    case "All":
                                        logger.Debug("Listing All");
                                        Console.WriteLine("Listing All:");
                                        data.PrintAll();
                                        break;
                                    default:
                                        logger.Debug("Listing transactions involving '" + command[1] + "'");
                                        Console.WriteLine("Listing transactions involving '" + command[1] + "':");
                                        data.PrintAccount(command[1]);
                                        break;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    Console.WriteLine();
                }
            }

            logger.Info("Quit Program by 'q' command");
        }

        static bool ValidCommand(string command)
        {
            if (command.Equals("q")) return true;

            string[] split = command.Split(new[] { ' ' }, 2);
            if (split[0].Equals("List") || split[0].Equals("Import"))
            {
                return true;
            }

            return false;
        }

        static TransactionList Import(string fileName)
        {
            logger.Debug("Importing file '" + fileName + "'");

            TransactionList data = null;
            
            if (!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
                logger.Error("File does not exist - " + fileName);
                return data;
            }

            string ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".csv":
                    data = ParseCSV(fileName);
                    break;
                case ".json":
                    data = ParseJSON(fileName);
                    break;
                default:
                    Console.WriteLine("Invalid extension (should be .json or .csv)");
                    logger.Debug("Invalid extension - found " + ext);
                    break;
            }

            return data;
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


                if (errorsFound)
                {
                    Console.WriteLine("You should fix these errors and try importing again.");
                }
            }

            return data;
        }

        static TransactionList ParseJSON(string fileName)
        {
            TransactionList data = new TransactionList();

            return data;
        }
    }
}
