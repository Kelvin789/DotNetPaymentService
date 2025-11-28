using Bank.DeveloperTest.Types;

namespace Bank.DeveloperTest.Services
{
    public interface IAccountService
    {
        Account GetAccountByNumber(string dataStoreType, string debtorAccountNumber);
        void UpdateAccount(string dataStoreType, Account account);
    }
}
