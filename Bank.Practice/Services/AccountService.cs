using Bank.DeveloperTest.Data;
using Bank.DeveloperTest.Types;
using Bank.DeveloperTest.Utilities;

namespace Bank.DeveloperTest.Services
{
    public class AccountService : IAccountService
    {
        public AccountService() 
        { 
        }

        public Account GetAccountByNumber(string dataStoreType, string debtorAccountNumber)
        {
            Account account = null;

            if (dataStoreType == DataStoreConstants.Backup)
            {
                var backupAccountDataStore = new BackupAccountDataStore();
                account = backupAccountDataStore.GetAccount(debtorAccountNumber);
            }
            else
            {
                var accountDataStore = new AccountDataStore();
                account = accountDataStore.GetAccount(debtorAccountNumber);
            }

            return account;
        }

        public void UpdateAccount(string dataStoreType, Account account)
        {
            if (dataStoreType == DataStoreConstants.Backup)
            {
                var backupAccountDataStore = new BackupAccountDataStore();
                backupAccountDataStore.UpdateAccount(account);
            }
            else
            {
                var accountDataStore = new AccountDataStore();
                accountDataStore.UpdateAccount(account);
            }
        }
    }
}
