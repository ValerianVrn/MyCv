namespace MyCv.Common.Domain.UnitTests
{
    [TestClass]
    public class DomainExceptionTests
    {
        [TestMethod]
        public void Constructor_Default_ShouldCreateInstance()
        {
            // Act
            var exception = new DomainException();

            // Assert
            Assert.IsNotNull(exception);
        }

        [TestMethod]
        public void Constructor_WithMessage_ShouldSetMessage()
        {
            // Arrange
            var expectedMessage = "This is a domain exception.";

            // Act
            var exception = new DomainException(expectedMessage);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [TestMethod]
        public void Constructor_WithMessageAndInnerException_ShouldSetProperties()
        {
            // Arrange
            var expectedMessage = "This is a domain exception.";
            var innerException = new InvalidOperationException("Inner exception message.");

            // Act
            var exception = new DomainException(expectedMessage, innerException);

            // Assert
            Assert.IsNotNull(exception);
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }
    }
}
