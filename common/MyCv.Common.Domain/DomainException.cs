namespace MyCv.Common.Domain
{
    /// <summary>
    /// Exception type for domain exceptions.
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DomainException()
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        public DomainException(string message) : base(message)
        {

        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public DomainException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
