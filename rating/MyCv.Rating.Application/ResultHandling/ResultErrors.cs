namespace MyCv.Rating.Application.ResultHandling
{
    /// <summary>
    /// Predefined errors.
    /// </summary>
    public static class ResultErrors
    {
        /// <summary>
        /// Conflict when Xinius already exists.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public static ResultError SaveEntitiesError(string visitorId)
        {
            return ResultError.Unknown("SAVE_ENTITIES_ERROR", $"Could not save entities when handling command {visitorId}.");
        }

        /// <summary>
        /// Conflict when assessment already exists.
        /// </summary>
        /// <param name="visitorId"></param>
        /// <returns></returns>
        public static ResultError AlreadyExists(string visitorId)
        {
            return ResultError.Conflict("ALREADY_EXISTS", $"An assessment was already found for visitor {visitorId}.");
        }
    }
}
