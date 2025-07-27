using MediatR;
using Prometheus;

namespace MyCv.Rating.Application.Behaviors
{
    /// <summary>
    /// Behavior used for metrics about the execution of commands processed through MediatR.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal class MetricsBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Wrap all command execution.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            MetricsCollectors.CommandExecutionCounter.WithLabels(request.GetType().Name).Inc();

            using (MetricsCollectors.CommandExecutionDurationHistogram.WithLabels(request.GetType().Name).NewTimer())
            {
                return await next();
            }
        }
    }
}
