namespace Stella.Models
{
    public class Stats
    {
        public string NumServices;
        public string NumCustomers;
        public string NumTransactions;

        public Stats(string numServices, string numCustomers, string numTransactions)
        {
            NumServices = numServices;
            NumCustomers = numCustomers;
            NumTransactions = numTransactions;
        }
    }
}
