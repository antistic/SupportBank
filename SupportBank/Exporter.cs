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
    class Exporter
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Export(string fileName, List<Transaction> data)
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

        private static void ExportCSV(string fileName, List<Transaction> data)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("Date,From,To,Narrative,Amount");
                foreach (Transaction t in data)
                {
                    file.WriteLine(
                        "{0:dd/MM/yy},{1},{2},{3},{4}",
                        t.Date, t.FromAccount, t.ToAccount, t.Narrative, t.Amount
                    );
                }
            }
        }

        private static void ExportJSON(string fileName, List<Transaction> data)
        {
            string output = JsonConvert.SerializeObject(data);
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

        private static void ExportXML(string fileName, List<Transaction> data)
        {
            DateTime epoch = DateTime.Parse("01/01/1900");
            XElement xml =
                new XElement("TransactionList",
                    data.Select(
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
