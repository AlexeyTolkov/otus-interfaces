namespace otus_interfaces
{
    public class TransactionsInputPredefined : ITransactionsInput
    {
        private readonly IBudgetApplication _budgetApp;

        public TransactionsInputPredefined(IBudgetApplication budgetApplication)
        {
            _budgetApp = budgetApplication;
        }

        public void ReadTransactions()
        {
            _budgetApp.AddTransaction("Зачисление 12792 RUB МРОТ_Зарплата");
            _budgetApp.AddTransaction("Трата -400 RUB Продукты Пятерочка");
            _budgetApp.AddTransaction("Трата -2000 RUB Бензин IRBIS");
            _budgetApp.AddTransaction("Трата -500 KZT Кафе Шоколадница");
            _budgetApp.AddTransaction("Трата -100 USD Keyboard Amazon");
            _budgetApp.AddTransaction("Перевод -500 RUB Васе_Иванову Возврат_долга");
            _budgetApp.AddTransaction("Трата -100 KZT Кафе Шоколадница");
        }
    }
}