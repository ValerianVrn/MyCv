using Microsoft.AspNetCore.Mvc;
using MyCv.Rating.Api.Read.Extensions.Loggers;
using MyCv.Rating.Application.Queries;
using MyCv.Rating.Application.ViewModels;
using System.Net;

namespace MyCv.Rating.Api.Read.Controllers
{
    /// <summary>
    /// 
    /// Assessments controller.
    /// </summary>
    /// <param name="assessmentQueries"></param>
    [Route("api/[controller]")]
    [ApiController]
    public class AssessmentController(IAssessmentQueries assessmentQueries, ILogger<AssessmentController> logger) : Controller
    {
        /// <summary>
        /// Assessment queries.
        /// </summary>
        private readonly IAssessmentQueries _assessmentQueries = assessmentQueries ?? throw new ArgumentNullException(nameof(assessmentQueries));

        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Get the assessment of a visitor.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        [Route("{visitorId}")]
        [HttpGet]
        [ProducesResponseType(typeof(AssessmentViewModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult> GetAssessmentAsync(string visitorId)
        {
            if (string.IsNullOrEmpty(visitorId))
            {
                return BadRequest("The visitor ID can not be null nor empty.");
            }

            try
            {
                var assessment = await _assessmentQueries.GetVisitorAssessmentAsync(visitorId);

                if (assessment is null)
                {
                    return NotFound();
                }

                return Ok(assessment);
            }
            catch (Exception exception)
            {
                _logger.FailedToExecuteMethod(exception);
                return Problem();
            }
        }
    }
}
