using Microsoft.Extensions.DependencyInjection;
using MyCv.Rating.Domain.AggregateModels.AssessmentAggregate;
using MyCv.Rating.Infrastructure.Repositories;

namespace MyCv.Rating.Infrastructure.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            return services
                .AddScoped<IAssessmentRepository, AssessmentRepository>();
        }
    }
}
