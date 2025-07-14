using Moq;
using MyCv.Common.Domain;
using MyCv.Rating.Application.Commands;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;

namespace MyCv.Rating.Application.UnitTests.Commands
{
    [TestClass]
    public class CreateAssessmentCommandHandlerTest
    {
        [TestMethod]
        public async Task Handle_AssessmentDoesNotExist_ReturnsSuccess()
        {
            // Arrange.
            var unitOfWork = new Mock<IUnitOfWork>();
            _ = unitOfWork.Setup(r => r.SaveEntitiesAsync( It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var assessmentRepository = new Mock<IAssessmentRepository>();
            _ = assessmentRepository.Setup(r => r.UnitOfWork).Returns(unitOfWork.Object);
            var createAssessmentCommandHandler = new CreateAssessmentCommandHandler(assessmentRepository.Object);
            var createAssessmentCommand = new CreateAssessmentCommand(VisitorId: "123456789", Score: 3, Recommend: true);

            // Act.
            var result = await createAssessmentCommandHandler.Handle(createAssessmentCommand, CancellationToken.None);

            // Assert.
            Assert.IsTrue(result.IsSuccess);
        }

        [TestMethod]
        public async Task Handle_AssessmentSaveFailed_ReturnsFailure()
        {
            // Arrange.
            var unitOfWork = new Mock<IUnitOfWork>();
            _ = unitOfWork.Setup(r => r.SaveEntitiesAsync( It.IsAny<CancellationToken>())).ReturnsAsync(false);
            var assessmentRepository = new Mock<IAssessmentRepository>();
            _ = assessmentRepository.Setup(r => r.UnitOfWork).Returns(unitOfWork.Object);
            var createAssessmentCommandHandler = new CreateAssessmentCommandHandler(assessmentRepository.Object);
            var createAssessmentCommand = new CreateAssessmentCommand(VisitorId: "123456789", Score: 3, Recommend: true);

            // Act.
            var result = await createAssessmentCommandHandler.Handle(createAssessmentCommand, CancellationToken.None);

            // Assert.
            Assert.IsTrue(result.IsFailure);
        }

        [TestMethod]
        public async Task Handle_AssessmentAlreadyExists_ReturnsFailure()
        {
            // Arrange.
            var createAssessmentCommand = new CreateAssessmentCommand(VisitorId: "123456789", Score: 3, Recommend: true);
            var assessmentRepository = new Mock<IAssessmentRepository>();
            assessmentRepository.Setup(r => r.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new Assessment(Guid.NewGuid(), createAssessmentCommand.VisitorId, 2, true));
            var createAssessmentCommandHandler = new CreateAssessmentCommandHandler(assessmentRepository.Object);

            // Act.
            var result = await createAssessmentCommandHandler.Handle(createAssessmentCommand, CancellationToken.None);

            // Assert.
            Assert.IsTrue(result.IsFailure);
        }
    }
}
