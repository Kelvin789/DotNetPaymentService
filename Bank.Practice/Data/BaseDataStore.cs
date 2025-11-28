using Bank.DeveloperTest.Types;

namespace Bank.DeveloperTest.Data
{
    public class BaseDataStore
    {
        public Account GetAccount(string accountNumber)
        {
            // Access (backup) database to retrieve account, code removed for brevity 
            return new Account();
        }

        public void UpdateAccount(Account account)
        {
            // Update account in (backup) database, code removed for brevity
        }
    }
}
