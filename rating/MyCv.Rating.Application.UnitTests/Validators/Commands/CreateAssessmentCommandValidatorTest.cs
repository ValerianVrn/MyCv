using FluentValidation.TestHelper;
using MyCv.Rating.Application.Commands;
using MyCv.Rating.Application.Validators.Commands;

namespace MyCv.Rating.Application.UnitTests.Validators.Commands
{
    [TestClass]
    public class CreateAssessmentCommandValidatorTest
    {
        private static CreateAssessmentCommand CreateCommand(
                            string visitorId = "123456789",
                            int score = 3,
                            bool recommend = false)
        {
            return new CreateAssessmentCommand(
                            VisitorId: visitorId,
                            Score: score,
                            Recommend: recommend);
        }

        /// <summary>
        /// Invalid commands.
        /// </summary>
        public static IEnumerable<object[]> InvalidCommands
        {
            get
            {
                return
                [
                    [
                        nameof(CreateAssessmentCommand.VisitorId), CreateCommand(visitorId: string.Empty)
                    ],
                    [
                        nameof(CreateAssessmentCommand.Score), CreateCommand(score: -1)
                    ]
                ];
            }
        }

        [TestMethod]
        public async Task ValidateAsync_ValidCommand_HasNoValidationError()
        {
            // Arrange.
            var command = CreateCommand();
            var commandValidator = new CreateAssessmentCommandValidator();

            // Act.
            var validationResult = await commandValidator.TestValidateAsync(command);

            // Assert.
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [TestMethod]
        [DynamicData(nameof(InvalidCommands))]
        public async Task ValidateAsync_InvalidCommand_HasValidationError(string propertyName, CreateAssessmentCommand commmand)
        {
            // Arrange.
            var commandValidator = new CreateAssessmentCommandValidator();

            // Act.
            var validationResult = await commandValidator.TestValidateAsync(commmand);

            // Assert.
            validationResult.ShouldHaveValidationErrorFor(propertyName);
        }
    }
}
