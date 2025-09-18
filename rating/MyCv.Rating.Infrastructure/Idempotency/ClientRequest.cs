namespace MyCv.Rating.Infrastructure.Idempotency
{
    /// <summary>
    /// Client request for dealing with idempotency.
    /// </summary>
    internal class ClientRequest
    {
        /// <summary>
        /// ID of the client request.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Name of the client request.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Date time of the client request.
        /// </summary>
        public DateTime Time { get; set; }
    }
}
