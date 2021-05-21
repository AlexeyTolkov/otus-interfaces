using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace otus_interfaces
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            //var currencyConverter = new CurrencyConverter();
            var currencyConverter = new ExchangeRatesApiConverter(new HttpClient(), new MemoryCache(new MemoryCacheOptions()), "950ad6c9199ec17f1dab4f8a93c1739b");
            var transactionRepository = new InMemoryTransactionRepository();
            var transactionParser = new TransactionParser();

            var budgetApp = new BudgetApplication(transactionRepository, transactionParser, currencyConverter);

            var predefinedTransactionsInput = new PredefinedTransactionsInput(budgetApp);
            predefinedTransactionsInput.ReadTransactions();

            budgetApp.OutputTransactions();
            budgetApp.OutputBalanceInCurrency("USD");

            Console.Read();
        }
    }
}