using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace otus_interfaces
{
    public class BudgetApplication : IBudgetApplication
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionParser _transactionParser;
        private readonly ICommissionCalculator _commissionCalculator;

        public BudgetApplication(ITransactionRepository transactionRepository,
                                 ITransactionParser transactionParser,
                                 ICurrencyConverter currencyConverter,
                                 ICommissionCalculator commissionCalculator)
        {
            _currencyConverter = currencyConverter;
            _transactionRepository = transactionRepository;
            _transactionParser = transactionParser;
            _commissionCalculator = commissionCalculator;
        }

        public void AddTransaction(string input)
        {
            var transaction = _transactionParser.Parse(input);
            _transactionRepository.AddTransaction(transaction);

            var commission = _commissionCalculator.Calc(transaction);
            if (commission != null)
            {
                _transactionRepository.AddTransaction(commission);
            }

            //if (transaction is Transfer)
            //{
            //    var commission = new Commission(transaction);
            //    _transactionRepository.AddTransaction(commission);
            //}
        }

        public void OutputTransactions()
        {
            foreach (var transaction in _transactionRepository.GetTransactions())
            {
                Console.WriteLine(transaction);
            }
        }

        public void OutputBalanceInCurrency(string currencyCode)
        {
            var totalCurrencyAmount = new CurrencyAmount(currencyCode, 0);
            var amounts = _transactionRepository.GetTransactions()
                .Select(t => t.Amount)
                .Select(a => a.CurrencyCode != currencyCode ? _currencyConverter.ConvertCurrency(a, currencyCode) : a)
                .ToArray();

            var totalBalanceAmount = amounts.Aggregate(totalCurrencyAmount, (t, a) => t += a);

            Console.WriteLine($"Balance: {totalBalanceAmount} {currencyCode}");
        }
    }
}
