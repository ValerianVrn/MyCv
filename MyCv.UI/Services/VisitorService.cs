using Microsoft.JSInterop;

namespace MyCv.UI.Services
{
    /// <inheritdoc/>
    internal class VisitorService(ILogger<VisitorService> logger, IJSRuntime jSRuntime) : IVisitorService
    {
        /// <summary>
        /// Logger.
        /// </summary>
        private readonly ILogger<VisitorService> _logger = logger;

        /// <summary>
        /// JS runtime.
        /// </summary>
        private readonly IJSRuntime _jSRuntime = jSRuntime;

        /// <summary>
        /// ID of the visitor.
        /// </summary>
        private string _visitorId;

        /// <inheritdoc/>
        public async Task<string> GetVisitorId()
        {
            if (!string.IsNullOrWhiteSpace(_visitorId))
            {
                _logger.LogError(_visitorId);
                return _visitorId;
            }

            _visitorId = await _jSRuntime.InvokeAsync<string>("getOrCreateVisitorId");
            
                _logger.LogInformation(_visitorId);
            return _visitorId;
        }
    }
}
