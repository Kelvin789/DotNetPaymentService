using Bank.DeveloperTest.Types;

namespace Bank.DeveloperTest.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IAccountService _accountService;

        private string _dataStoreType = null;

        public PaymentService(IAccountService accountService)
        {
            _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));

            //_dataStoreType = ConfigurationManager.AppSettings["DataStoreType"];
            _dataStoreType = "NotBackup";
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            Account account = _accountService.GetAccountByNumber(_dataStoreType, request.DebtorAccountNumber);

            var result = new MakePaymentResult()
            {
                Success = true
            };

            if (account == null)
            {
                result.Success = false;
                return result;
            }

            switch (request.PaymentScheme)
            {
                case PaymentScheme.FasterPayments:
                    bool canUseFasterPayments = account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments);
                    bool hasEnoughBalance = account.Balance >= request.Amount;

                    result.Success = canUseFasterPayments && hasEnoughBalance;
                    break;
                case PaymentScheme.Bacs:
                    result.Success = account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
                    break;
                case PaymentScheme.Chaps:
                    bool canUseChaps = account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps);
                    bool accountIsLive = account.Status == AccountStatus.Live;

                    result.Success = canUseChaps && accountIsLive;
                    break;
                default:
                    result.Success = false;
                    break;
            }

            if (result.Success)
            {
                account.Balance -= request.Amount;

                _accountService.UpdateAccount(_dataStoreType, account);
            }

            return result;
        }
    }
}
