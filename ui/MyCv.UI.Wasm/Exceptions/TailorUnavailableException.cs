namespace MyCv.UI.Wasm.Exceptions
{
    /// <summary>
    /// The Tailor API could not be reached or returned an error.
    /// </summary>
    public class TailorUnavailableException : Exception
    {
        public TailorUnavailableException()
        {
        }

        public TailorUnavailableException(string message) : base(message)
        {
        }

        public TailorUnavailableException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// The visitor exceeded the Tailor API rate limit (HTTP 429).
    /// </summary>
    public sealed class TailorRateLimitedException : TailorUnavailableException
    {
        public TailorRateLimitedException()
        {
        }

        public TailorRateLimitedException(string message) : base(message)
        {
        }

        public TailorRateLimitedException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
