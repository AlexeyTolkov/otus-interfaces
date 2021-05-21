namespace otus_interfaces
{
    internal class CurrencyConverter : ICurrencyConverter
    {
        public ICurrencyAmount ConvertCurrency(ICurrencyAmount amount, string currencyCode)
        {
            return new CurrencyAmount(currencyCode, amount.Amount);
        }
    }
}