namespace otus_interfaces
{
    internal class LazyCurrencyConverter : ICurrencyConverter
    {
        public ICurrencyAmount ConvertCurrency(ICurrencyAmount amount, string currencyCode)
        {
            return new CurrencyAmount(currencyCode, amount.Amount);
        }
    }
}