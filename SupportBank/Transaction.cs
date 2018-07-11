using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank
{
    class TransactionList
    {
        public List<Transaction> Transactions { get; }
        private Dictionary<string, int> MaxWidths = new Dictionary<string, int>()
            {
                { "date", "dd/MM/yy".Length }, { "from", 0 }, { "to", 0 }, { "narrative", 0 }
            };
        private Dictionary<string, decimal> Accounts = new Dictionary<string, decimal>();

        public TransactionList()
        {
            Transactions = new List<Transaction>();
        }

        public void Add(Transaction t)
        {
            Transactions.Add(t);

            // update accounts
            if (!Accounts.ContainsKey(t.From)) Accounts.Add(t.From, 0);
            if (!Accounts.ContainsKey(t.To)) Accounts.Add(t.To, 0);

            Accounts[t.From] -= t.Amount;
            Accounts[t.To] += t.Amount;

            // update maxwidths
            if (t.From.ToString().Length > MaxWidths["from"]) MaxWidths["from"] = t.From.ToString().Length;
            if (t.To.ToString().Length > MaxWidths["to"]) MaxWidths["to"] = t.To.ToString().Length;
            if (t.Narrative.ToString().Length > MaxWidths["narrative"]) MaxWidths["narrative"] = t.Narrative.ToString().Length;
        }

        public void PrintAll()
        {
            int width = Math.Max(MaxWidths["from"], MaxWidths["to"]);

            // header
            Console.WriteLine("{0,-" + width + "}\t{1,10:C2}", "Name", "Amount Owed");

            // content
            foreach (KeyValuePair<string, decimal> kvp in Accounts)
            {
                Console.WriteLine("{0,-" + width + "}\t{1,10:C2}", kvp.Key, kvp.Value);
            }
        }

        public void PrintAccount(string name)
        {
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
            foreach (Transaction t in Transactions)
            {
                if (t.From.Equals(name) || t.To.Equals(name))
                {
                    Console.WriteLine(
                        "{0:dd/MM/yy}\t" +
                        "{1,-" + MaxWidths["from"] + "}\t" +
                        "{2,-" + MaxWidths["to"] + "}\t" +
                        "{3,-" + MaxWidths["narrative"] + "}\t" +
                        "{4,10:C2}",
                        t.Date, t.From, t.To, t.Narrative, t.Amount
                    );
                }
            }
        }
    }

    class Transaction
    {
        static public int Length = 5;

        // TODO: amount should probably be an Int
        public Transaction(DateTime date, string from, string to, string narrative, decimal amount)
        {
            Date = date;
            From = from;
            To = to;
            Narrative = narrative;
            Amount = amount;
        }

        public DateTime Date { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }
    }
}
