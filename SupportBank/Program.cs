﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using NLog;
using NLog.Config;
using NLog.Targets;
using Newtonsoft.Json;

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
                        case "Export":
                            Export(command[1], data);
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
            switch (split[0])
            {
                case "List":
                case "Import":
                case "Export":
                    return true;
                default:
                    return false;
            }
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
                case ".xml":
                    data = ParseXML(fileName);
                    break;
                default:
                    Console.WriteLine("Invalid extension (should be .json, .csv or .xml)");
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
            try
            {
                string fileInput = File.ReadAllText(fileName);
                TransactionList data = new TransactionList(JsonConvert.DeserializeObject<List<Transaction>>(fileInput));

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while parsing JSON: " + e.Message);
                logger.Error("Error while parsing JSON: " + e.Message);
            }

            return null;
        }

        static TransactionList ParseXML(string fileName)
        {
            try
            {
                XElement xml = XElement.Load(fileName);

                // TODO: indicate error locations
                DateTime epoch = DateTime.Parse("01/01/1900");
                TransactionList data = new TransactionList(
                        (
                            from transaction in xml.Elements("SupportTransaction")
                            select new Transaction(
                                epoch.AddDays(int.Parse(transaction.Attribute("Date").Value)),
                                transaction.Element("Parties").Element("From").Value,
                                transaction.Element("Parties").Element("To").Value,
                                transaction.Element("Description").Value,
                                decimal.Parse(transaction.Element("Value").Value)
                        )
                    ).ToList()
                );

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while parsing XML: " + e.Message);
                logger.Error("Error while parsing XML: " + e.Message);
            }

            return null;
        }

        static void Export(string fileName, TransactionList data)
        {
            logger.Debug("Exporting to file '" + fileName + "'");

            if (data == null)
            {
                Console.WriteLine("No data to export!");
                logger.Warn("Cannot export to " + fileName + " because no data found");
            }
            
            string ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".csv":
                    ExportCSV(fileName, data);
                    break;
                case ".json":
                    ExportJSON(fileName, data);
                    break;
                case ".xml":
                    ExportXML(fileName, data);
                    break;
                default:
                    Console.WriteLine("Invalid extension (should be .json, .csv or .xml)");
                    logger.Debug("Invalid extension - found " + ext);
                    break;
            }
        }

        static void ExportCSV(string fileName, TransactionList data)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("Date,From,To,Narrative,Amount");
                foreach (Transaction t in data.Transactions)
                {
                    file.WriteLine(
                        "{0:dd/MM/yy},{1},{2},{3},{4}",
                        t.Date, t.FromAccount, t.ToAccount, t.Narrative, t.Amount
                    );
                }
            }
        }

        static void ExportJSON(string fileName, TransactionList data)
        {
            string output = JsonConvert.SerializeObject(data.Transactions);
            try
            {
                File.WriteAllText(fileName, output);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not write to file.");
                logger.Debug("Could not write to file " + e.Message);
                throw;
            }
        }

        static void ExportXML(string fileName, TransactionList data)
        {
            List<Transaction> ts = data.Transactions;
            DateTime epoch = DateTime.Parse("01/01/1900");
            XElement xml =
                new XElement("TransactionList",
                    ts.Select(
                        (t) =>
                            new XElement("SupportTransaction",
                                new XAttribute("Date", (t.Date - epoch).Days),
                                new XElement("Description", new XText(t.Narrative)),
                                new XElement("Parties",
                                    new XElement("From", new XText(t.FromAccount)),
                                    new XElement("To", new XText(t.ToAccount))
                                ),
                                new XElement("Value", new XText(t.Amount.ToString()))
                            )
                    )
                );

            File.WriteAllText(fileName, xml.ToString());
        }
    }
}
