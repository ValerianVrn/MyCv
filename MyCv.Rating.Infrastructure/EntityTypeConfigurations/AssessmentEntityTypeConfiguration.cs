using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;

namespace MyCv.Rating.Infrastructure.EntityTypeConfigurations
{
    internal class AssessmentEntityTypeConfiguration : IEntityTypeConfiguration<Assessment>
    {
        /// <inheritdoc/>
        public void Configure(EntityTypeBuilder<Assessment> entityTypeBuilder)
        {
            _ = entityTypeBuilder.ToTable("Assessment");

            // Domain events.
            _ = entityTypeBuilder.Ignore(b => b.DomainEvents);

            // GUID.
            _ = entityTypeBuilder.HasKey(b => b.EntityGuid);

            _ = entityTypeBuilder.Property(b => b.EntityGuid)
                              .HasMaxLength(36)
                              .IsRequired();

            _ = entityTypeBuilder.HasIndex(b => b.EntityGuid)
                              .IsUnique();

            // Visitor ID.
            _ = entityTypeBuilder.Property(b => b.VisitorId)
                              .IsRequired();

            // Score.
            _ = entityTypeBuilder.Property(b => b.Score)
                              .IsRequired();

            // Recommend.
            _ = entityTypeBuilder.Property(b => b.Recommend)
                              .IsRequired();
        }
    }
}
