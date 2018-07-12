using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using SupportBank.Banking.Transactions;

namespace SupportBank.Banking
{
    class Bank
    {
        Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private List<Transaction> transactions;
        private Accounts accounts = new Accounts();
        private Dictionary<string, int> MaxWidths = new Dictionary<string, int>()
            {
                { "date", "dd/MM/yy".Length }, { "from", 0 }, { "to", 0 }, { "narrative", 0 }
            };

        public Bank()
        {
            transactions = new List<Transaction>();
        }

        public void ImportFromFile(string fileName)
        {
            transactions = Importer.Import(fileName);
        }
        
        public void ExportFromFile(string fileName)
        {
            Exporter.Export(fileName, transactions);
        }

        public void Add(Transaction t)
        {
            transactions.Add(t);
            accounts.Add(t);

            // update maxwidths
            if (t.FromAccount.ToString().Length > MaxWidths["from"]) MaxWidths["from"] = t.FromAccount.ToString().Length;
            if (t.ToAccount.ToString().Length > MaxWidths["to"]) MaxWidths["to"] = t.ToAccount.ToString().Length;
            if (t.Narrative.ToString().Length > MaxWidths["narrative"]) MaxWidths["narrative"] = t.Narrative.ToString().Length;
        }

        public void Add(List<Transaction> ts)
        {
            foreach (Transaction t in ts)
            {
                Add(t);
            }
        }

        public void PrintAll()
        {
            accounts.PrintBalances(Math.Max(MaxWidths["from"], MaxWidths["to"]), 10);
        }

        public void PrintAccount(string name)
        {
            logger.Debug("Listing transactions involving '" + name + "'");
            Console.WriteLine("Listing transactions involving '" + name + "':");

            // header
            Console.WriteLine(
                        "{0,-" + "dd/MM/yy".Length + "}\t" +
                        "{1,-" + MaxWidths["from"] + "}\t" +
                        "{2,-" + MaxWidths["to"] + "}\t" +
                        "{3,-" + MaxWidths["narrative"] + "}\t" +
                        "{4,10}",
                        "Date", "From", "To", "Narrative", "Amount"
                    );

            // contents
            foreach (Transaction t in transactions)
            {
                if (t.FromAccount.Equals(name) || t.ToAccount.Equals(name))
                {
                    t.Print(MaxWidths);
                }
            }
        }
    }
}
