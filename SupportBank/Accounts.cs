using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SupportBank.Banking.Transactions;
using NLog;

namespace SupportBank.Banking
{
    class Accounts
    {
        private static Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private Dictionary<string, Account> accounts = new Dictionary<string, Account>();

        public void Add(Transaction t)
        {
            // new account if one does not exist
            if (!accounts.ContainsKey(t.FromAccount)) AddNewAccount(t.FromAccount);
            if (!accounts.ContainsKey(t.ToAccount)) AddNewAccount(t.ToAccount);

            // add transactions to accounts
            accounts[t.FromAccount].Add(t);
            accounts[t.ToAccount].Add(t);
        }

        public void PrintBalances(int nameWidth, int balanceWidth)
        {
            logger.Debug("Listing All");
            Console.WriteLine("Listing All:");

            int width = nameWidth;

            // header
            Console.WriteLine(
                "{0,-" + width + "}\t" +
                "{1," + balanceWidth + ":C2}",
                "Name", "Amount Owed");

            // content
            foreach (KeyValuePair<string, Account> kvp in accounts)
            {
                Console.WriteLine(
                    "{0,-" + width + "}\t" +
                    "{1," + balanceWidth + ":C2}",
                    kvp.Key, kvp.Value.Balance);
            }
        }

        private void AddNewAccount(string name)
        {
            Account newAccount = new Account(name);
            accounts.Add(name, newAccount);
        }
    }
}
