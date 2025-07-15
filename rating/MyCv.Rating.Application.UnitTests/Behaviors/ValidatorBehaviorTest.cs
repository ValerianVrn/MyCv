using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using MyCv.Common.Domain;
using MyCv.Rating.Application.Behaviors;

namespace MyCv.Rating.Application.UnitTests.Behaviors
{
    [TestClass]
    public class ValidatorBehaviorTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorvalidatorIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var logger = new Mock<ILogger<ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>>();
            List<IValidator<IRequest<It.IsAnyType>>>? validator = null;


            // Act & Assert
            _ = new ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>(validator!, logger.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorloggerIsNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            ILogger<ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>? logger = null;
            var validator = new Mock<IValidator<IRequest<It.IsAnyType>>>();


            // Act & Assert
            _ = new ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>([validator.Object], logger!);
        }

        [TestMethod]
        public async Task Handle_AnyArgumentvalidatorIsCalled()
        {
            // Arrange.
            var validator = new Mock<IValidator<IRequest<It.IsAnyType>>>();
            var validationResult = new Mock<ValidationResult>();
            _ = validator.Setup(v => v.Validate(It.IsAny<IRequest<It.IsAnyType>>())).Returns(validationResult.Object);
            var logger = new Mock<ILogger<ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>>();
            var validatorBehavior = new ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>([validator.Object], logger.Object);

            // Act.
            _ = await validatorBehavior.Handle(new Mock<IRequest<It.IsAnyType>>().Object, new Mock<RequestHandlerDelegate<It.IsAnyType>>().Object, CancellationToken.None);

            // Assert.
            validator.Verify(v => v.Validate(It.IsAny<IRequest<It.IsAnyType>>()), Times.Once);
        }

        [TestMethod]
        public void Handle_ValidationFailure_ThrowsException()
        {
            // Arrange.
            var validator = new Mock<IValidator<IRequest<It.IsAnyType>>>();
            var fakeValidationResult = new ValidationResult()
            {
                Errors =
                [
                    new("fake property", "fake error message")
                ]
            };

            _ = validator.Setup(v => v.Validate(It.IsAny<IRequest<It.IsAnyType>>())).Returns(fakeValidationResult);
            var logger = new Mock<ILogger<ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>>();
            var validatorBehavior = new ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>([validator.Object], logger.Object);

            // Act.
            async Task act()
            {
                _ = await validatorBehavior.Handle(new Mock<IRequest<It.IsAnyType>>().Object, new Mock<RequestHandlerDelegate<It.IsAnyType>>().Object, CancellationToken.None);
            }

            // Assert.
            _ = Assert.ThrowsExceptionAsync<DomainException>(act);
        }

        [TestMethod]
        public async Task Handle_ValidationFailureWithWarningLevel_DoNotThrowsException()
        {
            // Arrange.
            var validator = new Mock<IValidator<IRequest<It.IsAnyType>>>();
            var fakeValidationResult = new ValidationResult()
            {
                Errors =
                [
                    new()
                    {
                        Severity = Severity.Warning
                    }
                ]
            };
            _ = validator.Setup(v => v.Validate(It.IsAny<IRequest<It.IsAnyType>>())).Returns(fakeValidationResult);
            var logger = new Mock<ILogger<ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>>>();
            var validatorBehavior = new ValidatorBehavior<IRequest<It.IsAnyType>, It.IsAnyType>([validator.Object], logger.Object);

            // Act.
            _ = await validatorBehavior.Handle(new Mock<IRequest<It.IsAnyType>>().Object, new Mock<RequestHandlerDelegate<It.IsAnyType>>().Object, CancellationToken.None);

            // Assert.
            validator.Verify(v => v.Validate(It.IsAny<IRequest<It.IsAnyType>>()), Times.Once);
        }
    }
}
