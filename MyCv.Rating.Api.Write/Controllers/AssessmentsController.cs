using MediatR;
using Microsoft.AspNetCore.Mvc;
using MyCv.Rating.Api.Write.Extensions.Loggers;
using MyCv.Rating.Application.Commands;
using MyCv.Rating.Application.ResultHandling;
using MyCv.Rating.Application.ViewModels;
using System.Net;

namespace MyCv.Rating.Api.Write.Controllers
{
    /// <summary>
    /// Assessments controller.
    /// </summary>
    /// <param name="mediator"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentsController(IMediator mediator, ILogger<AssessmentsController> logger) : Controller
    {
        /// <summary>
        /// Mediator.
        /// </summary>
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger _logger = logger;

        /// <summary>
        /// Create an assessment.
        /// </summary>
        /// <param name="createAssessmentCommand"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<ActionResult> CreateAssessmentAsync(CreateAssessmentCommand createAssessmentCommand)
        {
            try
            {
                var result = await _mediator.Send(createAssessmentCommand);

                if (result.IsFailure)
                {
                    if (result.Error.Type.Equals(ResultError.ErrorType.Conflict))
                    {
                        return Conflict();
                    }

                    return Problem();
                }

                return NoContent();
            }
            catch (Exception exception)
            {
                _logger.FailedToExecuteMethod(exception);
                return Problem();
            }
        }
    }
}
