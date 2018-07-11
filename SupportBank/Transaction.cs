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

        public TransactionList(List<Transaction> list)
        {
            Transactions = new List<Transaction>();

            foreach (Transaction t in list)
            {
                Transactions.Add(t);
            }
        }

        public void Add(Transaction t)
        {
            Transactions.Add(t);

            // update accounts
            if (!Accounts.ContainsKey(t.FromAccount)) Accounts.Add(t.FromAccount, 0);
            if (!Accounts.ContainsKey(t.ToAccount)) Accounts.Add(t.ToAccount, 0);

            Accounts[t.FromAccount] -= t.Amount;
            Accounts[t.ToAccount] += t.Amount;

            // update maxwidths
            if (t.FromAccount.ToString().Length > MaxWidths["from"]) MaxWidths["from"] = t.FromAccount.ToString().Length;
            if (t.ToAccount.ToString().Length > MaxWidths["to"]) MaxWidths["to"] = t.ToAccount.ToString().Length;
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
                if (t.FromAccount.Equals(name) || t.ToAccount.Equals(name))
                {
                    Console.WriteLine(
                        "{0:dd/MM/yy}\t" +
                        "{1,-" + MaxWidths["from"] + "}\t" +
                        "{2,-" + MaxWidths["to"] + "}\t" +
                        "{3,-" + MaxWidths["narrative"] + "}\t" +
                        "{4,10:C2}",
                        t.Date, t.FromAccount, t.ToAccount, t.Narrative, t.Amount
                    );
                }
            }
        }
    }

    class Transaction
    {
        static public int Length = 5;
        
        public Transaction(DateTime date, string from, string to, string narrative, decimal amount)
        {
            Date = date;
            FromAccount = from;
            ToAccount = to;
            Narrative = narrative;
            Amount = amount;
        }

        public DateTime Date { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }
    }
}
