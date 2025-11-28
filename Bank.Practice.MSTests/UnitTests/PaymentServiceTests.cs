using Bank.DeveloperTest.Services;
using Bank.DeveloperTest.Types;
using Moq;

namespace Bank.DeveloperTest.MSTests.UnitTests
{
    [TestClass]
    public class PaymentServiceTests
    {
        private Mock<IAccountService> _mockAccountService;
        private PaymentService _paymentService;

        private MakePaymentRequest _testMakePaymentRequest;
        private Account _testAccount;

        [TestInitialize]
        public void Initialize()
        {
            _mockAccountService = new Mock<IAccountService>();
            _paymentService = new PaymentService(_mockAccountService.Object);

            _testMakePaymentRequest = new MakePaymentRequest
            {
                CreditorAccountNumber = "Creditor001",
                DebtorAccountNumber = "Debtor001",
                Amount = 100.00M,
                PaymentDate = DateTime.Now,
                PaymentScheme = PaymentScheme.FasterPayments
            };

            _testAccount = new Account
            {
                AccountNumber = "AccountNumber001",
                Balance = 200,
                AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments,
                Status = AccountStatus.Live
            };
        }

        #region Payment Scheme = FasterPayments
        [TestMethod]
        public void MakePayment_ForValidFasterPayments_ShouldSucceed()
        {
            // Arrange
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.FasterPayments;
            _testAccount.Balance = 500;
            _testMakePaymentRequest.Amount = 39.99M;
            decimal expectedBalance = 460.01M;
            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual(expectedBalance, _testAccount.Balance);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), _testAccount), Times.Once);
        }

        [TestMethod]
        public void MakePayment_WhenInsufficientBalanceForFasterPayments_ShouldReturnFailure()
        {
            // Arrange
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.FasterPayments;
            _testAccount.Balance = 50;
            _testMakePaymentRequest.Amount = 100;
            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsFalse(result.Success);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), It.IsAny<Account>()), Times.Never);
        }
        #endregion

        #region Payment Scheme = Bacs
        [TestMethod]
        public void MakePayment_ForValidBacs_ShouldSucceed()
        {
            // Arrange
            _testAccount.AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs;
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.Bacs;
            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsTrue(result.Success);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), _testAccount), Times.Once);
        }

        [TestMethod]
        public void MakePayment_WhenAllowedPaymentSchemesNotBacs_ShouldReturnFailure()
        {
            // Arrange
            _testAccount.AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments; // no Bacs
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.Bacs;
            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsFalse(result.Success);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), It.IsAny<Account>()), Times.Never);
        }
        #endregion

        #region Payment Scheme = Chaps
        [TestMethod]
        public void MakePayment_WhenAccountIsLiveForChaps_ShouldSucceed()
        {
            // Arrange
            _testAccount.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            _testAccount.Status = AccountStatus.Live;
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.Chaps;
            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsTrue(result.Success);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), _testAccount), Times.Once);
        }

        public void MakePayment_WhenAccountIsDisabledForChaps_ShouldReturnFailure()
        {
            // Arrange
            _testAccount.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            _testAccount.Status = AccountStatus.Disabled;
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.Chaps;

            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsFalse(result.Success);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), It.IsAny<Account>()), Times.Never);
        }

        public void MakePayment_WhenAccountIsInboundPaymentsOnlyForChaps_ShouldReturnFailure()
        {
            // Arrange
            _testAccount.AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps;
            _testAccount.Status = AccountStatus.Disabled;
            _testMakePaymentRequest.PaymentScheme = PaymentScheme.Chaps;

            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsFalse(result.Success);
            _mockAccountService.Verify(s => s.UpdateAccount(It.IsAny<string>(), It.IsAny<Account>()), Times.Never);
        }
        #endregion

        #region Non Specific Payment Scheme
        [TestMethod]
        public void MakePayment_ForUnsupportedPaymentScheme_ShouldReturnFailure()
        {
            // Arrange
            _testMakePaymentRequest.PaymentScheme = (PaymentScheme)123;
            _mockAccountService.Setup(s => s.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(_testAccount);

            // Act
            var result = _paymentService.MakePayment(_testMakePaymentRequest);

            // Assert
            Assert.IsFalse(result.Success);
        }
        #endregion

        #region For Debugging Purposes Only
        /*
        /// <summary>
        /// Using this method to debug/invoke the MakePayment() method
        /// </summary>
        [TestMethod]
        public void MakePayment_ForDebugging_ShouldReturnOk()
        {
            var accountService = new AccountService();
            var service = new PaymentService(accountService);
            service.MakePayment(_testMakePaymentRequest);
            Assert.IsTrue(true);
        }
        */
        #endregion
    }
}
