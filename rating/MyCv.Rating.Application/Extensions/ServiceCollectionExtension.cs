using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using MyCv.Rating.Application.Behaviors;
using MyCv.Rating.Application.Commands;
using MyCv.Rating.Application.Queries;
using MyCv.Rating.Application.ResultHandling;
using MyCv.Rating.Application.Validators.Commands;

namespace MyCv.Rating.Application.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddQueries(this IServiceCollection services)
        {
            return services
                .AddScoped<IAssessmentQueries, AssessmentQueries>();
        }

        public static IServiceCollection AddMediator(this IServiceCollection services)
        {
            _ = services
                .AddScoped<IMediator, Mediator>()
                // Command classes (request handlers).
                .AddScoped<IRequestHandler<CreateAssessmentCommand, Result>, CreateAssessmentCommandHandler>()
                // Command validators.
                .AddScoped<IValidator<CreateAssessmentCommand>, CreateAssessmentCommandValidator>()
                // Behaviors.
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(MetricsBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>))
                .AddScoped(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>))
                ;

            return services;
        }
    }
}
