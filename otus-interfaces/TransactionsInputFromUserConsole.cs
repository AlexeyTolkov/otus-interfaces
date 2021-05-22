using System;

namespace otus_interfaces
{
    public class TransactionsInputFromUserConsole : ITransactionsInput
    {
        private readonly IBudgetApplication _budgetApp;
        public TransactionsInputFromUserConsole(IBudgetApplication budgetApplication)
        {
            _budgetApp = budgetApplication;
        }

        public void ReadTransactions()
        {
            Console.WriteLine("How many transactions would you like to enter? :");

            int transCnt = int.Parse(Console.ReadLine());
            for (int transNo = 1; transNo <= transCnt; transNo++)
            {
                Console.WriteLine($"Trans #{transNo}: ");
                _budgetApp.AddTransaction(Console.ReadLine());
            }
        }
    }
}