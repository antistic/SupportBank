using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
using Microsoft.VisualBasic.FileIO;
using NLog;

namespace SupportBank.Banking.Transactions
{
    class Importer
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static List<Transaction> Import(string fileName)
        {
            logger.Debug("Importing file '" + fileName + "'");

            if (!File.Exists(fileName))
            {
                Console.WriteLine("File does not exist");
                logger.Error("File does not exist - " + fileName);
                return null;
            }

            string ext = Path.GetExtension(fileName);
            switch (ext)
            {
                case ".csv":
                    return CSVImport(fileName);
                case ".json":
                    return JSONImport(fileName);
                case ".xml":
                    return XMLImport(fileName);
                default:
                    Console.WriteLine("Invalid extension (should be .json, .csv or .xml)");
                    logger.Debug("Invalid extension - found " + ext);
                    return null;
            }
        }

        private static List<Transaction> CSVImport(string fileName)
        {
            List<Transaction> data = new List<Transaction>();

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

        private static List<Transaction> JSONImport(string fileName)
        {
            try
            {
                string fileInput = File.ReadAllText(fileName);
                List<Transaction> data = JsonConvert.DeserializeObject<List<Transaction>>(fileInput);

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while parsing JSON: " + e.Message);
                logger.Error("Error while parsing JSON: " + e.Message);
            }

            return null;
        }

        private static List<Transaction> XMLImport(string fileName)
        {
            try
            {
                XElement xml = XElement.Load(fileName);

                // TODO: indicate error locations
                DateTime epoch = DateTime.Parse("01/01/1900");
                List<Transaction> data = (
                        from transaction in xml.Elements("SupportTransaction")
                        select new Transaction(
                            epoch.AddDays(int.Parse(transaction.Attribute("Date").Value)),
                            transaction.Element("Parties").Element("From").Value,
                            transaction.Element("Parties").Element("To").Value,
                            transaction.Element("Description").Value,
                            decimal.Parse(transaction.Element("Value").Value)
                    )
                ).ToList();

                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error while parsing XML: " + e.Message);
                logger.Error("Error while parsing XML: " + e.Message);
            }

            return null;
        }
    }
}
