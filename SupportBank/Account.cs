using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportBank.Banking.Transactions;

namespace SupportBank.Banking
{
    class Account
    {
        public string Name;
        public decimal Balance = 0;

        public Account(string name)
        {
            Name = name;
        }

        public void Add(Transaction t)
        {
            if (t.FromAccount.Equals(Name)) Balance -= t.Amount;
            if (t.ToAccount.Equals(Name)) Balance += t.Amount;
        }
    }
}
