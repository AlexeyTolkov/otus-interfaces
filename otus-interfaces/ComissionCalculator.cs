namespace otus_interfaces
{
    public class ComissionCalculator : ICommissionCalculator
    {
        public ITransaction Calc(ITransaction transaction)
        {
            if (!(transaction is Transfer))
                return null;

            // Фиксированно 2%
            return new Commission(transaction, 2);
        }
    }
}