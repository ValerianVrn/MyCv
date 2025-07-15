using static MyCv.Rating.Application.ResultHandling.ResultError;

namespace MyCv.Rating.Application.ResultHandling
{
    /// <summary>
    /// Error of a command handling.
    /// </summary>
    public sealed record ResultError(ErrorType Type, string Code, string Message)
    {
        /// <summary>
        /// Type of error.
        /// </summary>
        public enum ErrorType
        {
            None,
            Validation,
            Conflict,
            NotFound,
            Unknown
        }

        /// <summary>
        /// No error.
        /// </summary>
        public static readonly ResultError None = new(ErrorType.None, "None", string.Empty);

        /// <summary>
        /// Error when validating the command.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultError Validation(string code, string message)
        {
            return new(ErrorType.Validation, code, message);
        }

        /// <summary>
        /// Conflict of a resource.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultError Conflict(string code, string message)
        {
            return new(ErrorType.Conflict, code, message);
        }

        /// <summary>
        /// Resource not found.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultError NotFound(string code, string message)
        {
            return new(ErrorType.NotFound, code, message);
        }

        /// <summary>
        /// Unknown error.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResultError Unknown(string code, string message)
        {
            return new(ErrorType.Unknown, code, message);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Code} ({Enum.GetName(Type)}): {Message}";
        }
    }
}
