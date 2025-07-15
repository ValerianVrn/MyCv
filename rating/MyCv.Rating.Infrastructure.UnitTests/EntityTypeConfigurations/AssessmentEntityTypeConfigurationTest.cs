using Microsoft.EntityFrameworkCore;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;
using MyCv.Rating.Infrastructure.EntityTypeConfigurations;

namespace MyCv.Rating.Infrastructure.UnitTests.EntityTypeConfigurations
{
    [TestClass]
    public class AssessmentEntityTypeConfigurationTest
    {
        [TestMethod]
        public void Configure_ShouldApplyConfiguration()
        {
            // Arrange
            var modelBuilder = new ModelBuilder();
            var configuration = new AssessmentEntityTypeConfiguration();
            var entity = modelBuilder.Entity<Assessment>();

            // Act
            configuration.Configure(entity);

            // Assert
            var entityGuidProperty = entity.Property(e => e.EntityGuid);
            Assert.IsNotNull(entityGuidProperty);

            var visitorIdProperty = entity.Property(e => e.VisitorId);
            Assert.IsNotNull(visitorIdProperty);

            var scoreProperty = entity.Property(e => e.Score);
            Assert.IsNotNull(scoreProperty);

            var recommendProperty = entity.Property(e => e.Recommend);
            Assert.IsNotNull(recommendProperty);
        }
    }
}
