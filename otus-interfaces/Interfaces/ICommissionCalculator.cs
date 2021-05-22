namespace otus_interfaces
{
    public interface ICommissionCalculator
    {
        public ITransaction Calc(ITransaction transaction);
    }
}