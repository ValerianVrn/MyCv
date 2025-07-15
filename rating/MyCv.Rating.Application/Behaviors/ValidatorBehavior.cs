using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using MyCv.Common.Domain;
using MyCv.Rating.Application.Extensions.Loggers;

namespace MyCv.Rating.Application.Behaviors
{
    /// <summary>
    /// Behavior used for validating command processed through MediatR.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <remarks>
    /// Constructor.
    /// </remarks>
    /// <param name="validators"></param>
    /// <param name="logger"></param>
    internal class ValidatorBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Validators.
        /// </summary>
        private readonly IEnumerable<IValidator<TRequest>> _validators = validators ?? throw new ArgumentNullException(nameof(validators));

        /// <summary>
        /// Validate command on command processing.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="next"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="DomainException"></exception>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeName = request.GetType().Name;

            _logger.ValidatingCommand(typeName);

            var failures = _validators
                           .Select(v => v.Validate(request))
                           .SelectMany(result => result.Errors)
                           .Where(error => error != null)
                           .ToList();

            if (failures.Count != 0)
            {
                foreach (var failure in failures)
                {
                    switch (failure.Severity)
                    {
                        case Severity.Error:
                            _logger.ErrorLogOnCommandValidation(typeName, request, failure.ToString());
                            throw new DomainException($"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
                        case Severity.Warning:
                            _logger.WarningLogOnCommandValidation(typeName, request, failure.ToString());
                            break;
                        case Severity.Info:
                            _logger.InformationLogOnCommandValidation(typeName, request, failure.ToString());
                            break;
                        default:
                            break;
                    }

                }
            }
            return await next();
        }
    }
}
