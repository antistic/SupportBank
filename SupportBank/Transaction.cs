using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupportBank.Banking.Transactions
{
    class Transaction
    {
        static public int Length = 5;

        public DateTime Date { get; set; }
        public string FromAccount { get; set; }
        public string ToAccount { get; set; }
        public string Narrative { get; set; }
        public decimal Amount { get; set; }

        public Transaction(DateTime date, string from, string to, string narrative, decimal amount)
        {
            Date = date;
            FromAccount = from;
            ToAccount = to;
            Narrative = narrative;
            Amount = amount;
        }

        public void Print(Dictionary<string, int> maxWidths)
        {
            Console.WriteLine(
                "{0:dd/MM/yy}\t" +
                "{1,-" + maxWidths["from"] + "}\t" +
                "{2,-" + maxWidths["to"] + "}\t" +
                "{3,-" + maxWidths["narrative"] + "}\t" +
                "{4,10:C2}",
                Date, FromAccount, ToAccount, Narrative, Amount
            );
        }
    }
}
