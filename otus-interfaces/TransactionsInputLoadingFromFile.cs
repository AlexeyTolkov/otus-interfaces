namespace otus_interfaces
{
    public class TransactionsInputLoadingFromFile : ITransactionsInput
    {
        private readonly IBudgetApplication _budgetApp;
        public TransactionsInputLoadingFromFile(IBudgetApplication budgetApplication)
        {
            _budgetApp = budgetApplication;
        }

        public void ReadTransactions(string fileName)
        {
            var fileReader = new FileReader(fileName);
            foreach (var line in fileReader)
            {
                _budgetApp.AddTransaction(line);
            }
        }

        public void ReadTransactions()
        {
        }
    }
}