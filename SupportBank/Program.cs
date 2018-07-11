using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.FileIO;


namespace SupportBank
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "";
            while (!input.Equals("q"))
            {
                Console.WriteLine("Enter a command");
                input = Console.ReadLine();
                if (!ValidCommand(input))
                {
                    Console.WriteLine("Command must be 'List All', 'List [Account]' or 'q' to quit");
                }
                else
                {
                    Console.WriteLine();
                    TransactionList data = ParseCSV(@"../../../Transactions2014.csv");
                    string target = input.Split(new[] { ' ' }, 2)[1];
                    switch (target)
                    {
                        case "All":
                            Console.WriteLine("Listing All:");
                            data.Print((anything) => true);
                            break;
                        default:
                            Console.WriteLine("Listing transactions involving '" + target + "':");
                            data.Print((transaction) => transaction.From.Equals(target) || transaction.To.Equals(target) );
                            break;
                    }
                    Console.WriteLine();
                }
            }
        }

        static bool ValidCommand(string command)
        {
            string[] split = command.Split(new[] { ' ' }, 2);
            if (split[0].Equals("List"))
            {
                return true;
            }

            return false;
        }

        static TransactionList ParseCSV(string fileName)
        {
            TransactionList data = new TransactionList();

            using (TextFieldParser parser = new TextFieldParser(fileName))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                
                if (!parser.EndOfData)
                {
                    // header
                    // TODO?: check header is as expected
                    parser.ReadFields();
                }

                // rest of csv
                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    // TODO exception handling on convert
                    Transaction t = new Transaction(
                        DateTime.Parse(fields[0]),
                        fields[1],
                        fields[2],
                        fields[3],
                        float.Parse(fields[4])
                    );

                    data.Add(t);
                }
            }

            return data;
        }
    }
}
