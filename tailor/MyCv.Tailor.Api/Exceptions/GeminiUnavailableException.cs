using System.Net;

namespace MyCv.Tailor.Api.Exceptions
{
    /// <summary>
    /// Thrown when Gemini is temporarily unavailable (overloaded or quota exceeded), after retries.
    /// </summary>
    public sealed class GeminiUnavailableException : Exception
    {
        public GeminiUnavailableException()
        {
        }

        public GeminiUnavailableException(string message) : base(message)
        {
        }

        public GeminiUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public GeminiUnavailableException(HttpStatusCode statusCode) : base($"Gemini unavailable: {(int)statusCode} {statusCode}")
        {
            StatusCode = statusCode;
        }

        /// <summary>
        /// Status code returned by Gemini.
        /// </summary>
        public HttpStatusCode? StatusCode { get; }
    }
}
