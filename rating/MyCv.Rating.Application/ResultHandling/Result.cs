namespace MyCv.Rating.Application.ResultHandling
{
    /// <summary>
    /// Result of a command.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Indicate that the command has succeeded.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Indicate that the command has failed.
        /// </summary>
        public bool IsFailure
        {
            get
            {
                return !IsSuccess;
            }
        }

        /// <summary>
        /// Error of the command.
        /// </summary>
        public ResultError Error { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="isSuccess"></param>
        /// <param name="error"></param>
        /// <exception cref="InvalidOperationException"></exception>
        protected Result(bool isSuccess, ResultError error)
        {
            if (isSuccess && error != ResultError.None)
            {
                throw new InvalidOperationException("Successful result must not have an error.");
            }

            if (!isSuccess && error == ResultError.None)
            {
                throw new InvalidOperationException("Failure result must have an error.");
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        /// <summary>
        /// Return a successful result.
        /// </summary>
        /// <returns></returns>
        public static Result Success()
        {
            return new(true, ResultError.None);
        }

        /// <summary>
        /// Return a failure result.
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public static Result Failure(ResultError error)
        {
            return new(false, error);
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return IsSuccess ? "Success" : $"Failure: {Error}";
        }
    }
}
