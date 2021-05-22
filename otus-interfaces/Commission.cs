using System;

namespace otus_interfaces
{
    public class Commission : ITransaction // декоратор
    {
        public ITransaction OriginalTransaction { get; }
        public ICurrencyAmount Amount { get; }
        public DateTimeOffset Date { get; }
        public Commission(ITransaction transaction, decimal commissionPersent)
        {
            OriginalTransaction = transaction;
            Date = transaction.Date;

            var currencyAmount = transaction.Amount;
            Amount = new CurrencyAmount(currencyAmount.CurrencyCode, currencyAmount.Amount * commissionPersent / 100);
        }

        public override string ToString() => $"Комиссия в размере {Amount} за транзакцию: {OriginalTransaction}";
    }
}
