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

        public TransactionList()
        {
            Transactions = new List<Transaction>();
        }

        public void Add(Transaction t)
        {
            Transactions.Add(t);

            // update maxwidths
            if (t.From.ToString().Length > MaxWidths["from"]) MaxWidths["from"] = t.From.ToString().Length;
            if (t.To.ToString().Length > MaxWidths["to"]) MaxWidths["to"] = t.To.ToString().Length;
            if (t.Narrative.ToString().Length > MaxWidths["narrative"]) MaxWidths["narrative"] = t.Narrative.ToString().Length;
            if (t.Amount.ToString().Length > MaxWidths["amount"]) MaxWidths["amount"] = t.Amount.ToString().Length;
        }

        public void Print(Func<Transaction, bool> filter)
        {
            foreach (Transaction t in Transactions)
            {
                if (filter(t))
                {
                    Console.WriteLine(
                        "{0:dd/MM/yy} " +
                        "{1,-" + MaxWidths["from"] + "}\t" +
                        "{2,-" + MaxWidths["to"] + "}\t" +
                        "{3,-" + MaxWidths["narrative"] + "}\t" +
                        "{4," + MaxWidths["amount"] + "}",
                        t.Date, t.From, t.To, t.Narrative, t.Amount
                    );
                }
            }
        }

        private Dictionary<string, int> MaxWidths = new Dictionary<string, int>()
            {
                { "date", "dd/MM/yy".Length }, { "from", 0 }, { "to", 0 }, { "narrative", 0 }, { "amount", 0 }
            };
    }

    class Transaction
    {
        static public int Length = 5;

        // TODO: amount should probably be an Int
        public Transaction(DateTime date, string from, string to, string narrative, float amount)
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
        public float Amount { get; set; }
    }
}
