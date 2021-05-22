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

            //var currencyConverter = new LazyCurrencyConverter();
            var currencyConverter = new ExchangeRatesApiConverter(new HttpClient(), new MemoryCache(new MemoryCacheOptions()), "950ad6c9199ec17f1dab4f8a93c1739b");
            var transactionRepository = new InMemoryTransactionRepository();
            var transactionParser = new TransactionParser();
            var comissionCalculator = new ComissionCalculator();

            var budgetApp = new BudgetApplication(transactionRepository, transactionParser, currencyConverter, comissionCalculator);

            // 1. Predefined constant transactions input
            var transactionsInputPredefined = new TransactionsInputPredefined(budgetApp);
            transactionsInputPredefined.ReadTransactions();
             
            // 2. Loading from file 
            //var transactionsInputLoadingFromFile = new TransactionsInputLoadingFromFile(budgetApp);
            //transactionsInputLoadingFromFile.ReadTransactions("file.txt");

            // 3. Console user input 
            //var transactionsInputFromUserConsole = new TransactionsInputFromUserConsole(budgetApp);
            //transactionsInputFromUserConsole.ReadTransactions();

            budgetApp.OutputTransactions();
            budgetApp.OutputBalanceInCurrency("USD");

            Console.Read();
        }
    }
}