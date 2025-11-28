using Bank.DeveloperTest.Services;
using Bank.DeveloperTest.Types;
using Bank.DeveloperTest.Utilities;
using Moq;

namespace Bank.DeveloperTest.MSTests.UnitTests
{
    [TestClass]
    public class AccountServiceTests
    {
        private Mock<IAccountService> _mockAccountService;

        [TestInitialize]
        public void Initialize()
        {
            _mockAccountService = new Mock<IAccountService>();
        }

        [TestMethod]
        public void GetAccount_WithNonBackupDataStoreType_ShouldReturnOk()
        {
            // Arrange
            var expectedResult = new Account();
            _mockAccountService.Setup(service => service.GetAccountByNumber(It.IsAny<string>(), It.IsAny<string>())).Returns(expectedResult);

            // Act
            var result = _mockAccountService.Object.GetAccountByNumber("NotBackup", "Debtor001");

            // Assert
            Assert.AreEqual(result, expectedResult);
        }

        [TestMethod]
        public void GetAccount_WithBackupDataStoreType_ShouldReturnOk()
        {
            // Arrange
            var expectedResult = new Account();
            _mockAccountService.Setup(service => service.GetAccountByNumber(DataStoreConstants.Backup, It.IsAny<string>())).Returns(expectedResult);

            // Act
            var result = _mockAccountService.Object.GetAccountByNumber(DataStoreConstants.Backup, "Debtor001");

            // Assert
            Assert.AreEqual(result, expectedResult);
        }
    }
}
