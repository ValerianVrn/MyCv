using MediatR;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace MyCv.Rating.Application.Behaviors
{
    /// <summary>
    /// Behavior used to retry a command.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Retry handler.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            // For DbUpdateConcurrencyException, max duration : 10,20s
            var retryPolicy = Policy<TResponse>.Handle<DbUpdateConcurrencyException>().WaitAndRetryAsync(20, retryAttempt => TimeSpan.FromMilliseconds(Math.Pow(1, retryAttempt) * 50));
            return await retryPolicy.ExecuteAsync(async () => await next());
        }
    }
}
