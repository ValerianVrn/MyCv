using MediatR;
using Microsoft.Extensions.Logging;
using MyCv.Rating.Application.Extensions.Loggers;

namespace MyCv.Rating.Application.Behaviors
{
    /// <summary>
    /// Behavior used for logging information about the execution for all commands processed through MediatR.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="logger"></param>
    internal class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Wrap all command execution.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestGuid = Guid.NewGuid();

            _logger.HandlingCommand(request.GetType().Name, requestGuid, request);

            var response = await next();

            _logger.ResponseCommand(request.GetType().Name, requestGuid, response);

            return response;
        }
    }
}
