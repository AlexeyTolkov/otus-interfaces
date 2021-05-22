using System;

namespace otus_interfaces
{
    public class Income : ITransaction
    {
        public ICurrencyAmount Amount { get; }
        public DateTimeOffset Date { get; }

        public string Source { get; }
        public Income(ICurrencyAmount amount, DateTimeOffset date, string source)
        {
            Amount = amount;
            Date = date;
            Source = source;
        }

        public override string ToString() => $"Зачисление {Amount} от {Source}";
    }
}
