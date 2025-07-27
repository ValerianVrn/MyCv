using MediatR;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using MyCv.Rating.Infrastructure;

namespace MyCv.Rating.Application.UnitTests
{
    public class Helper
    {
        /// <summary>
        /// Create context for unit tests
        /// </summary>
        /// <returns></returns>
        internal static RatingContext CreateRatingContext()
        {
            // In memory connection
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<RatingContext>().UseSqlite(connection).Options;
            var mediator = new Mock<IMediator>();

            // Create context and add types
            var context = new RatingContext(options, mediator.Object);
            _ = context.Database.EnsureCreated();
            return context;
        }
    }
}
