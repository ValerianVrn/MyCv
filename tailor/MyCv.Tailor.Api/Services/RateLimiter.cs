using System.Collections.Concurrent;

namespace MyCv.Tailor.Api.Services
{
    /// <summary>
    /// Best-effort, in-memory sliding-window rate limiter keyed by client (IP).
    /// State is per Function instance: it protects against bursts, not against a distributed attack.
    /// </summary>
    public sealed class RateLimiter(TimeProvider timeProvider, int permitLimit = RateLimiter.DefaultPermitLimit, TimeSpan? window = null) : IRateLimiter
    {
        public const int DefaultPermitLimit = 10;
        public static readonly TimeSpan DefaultWindow = TimeSpan.FromMinutes(10);

        private const int CleanupThreshold = 10_000;

        private readonly TimeSpan _window = window ?? DefaultWindow;
        private readonly ConcurrentDictionary<string, Queue<DateTimeOffset>> _hits = new();

        /// <inheritdoc/>
        public bool TryAcquire(string clientKey)
        {
            var now = timeProvider.GetUtcNow();
            var queue = _hits.GetOrAdd(clientKey, _ => new Queue<DateTimeOffset>());

            bool acquired;
            lock (queue)
            {
                while (queue.Count > 0 && now - queue.Peek() >= _window)
                {
                    _ = queue.Dequeue();
                }

                acquired = queue.Count < permitLimit;
                if (acquired)
                {
                    queue.Enqueue(now);
                }
            }

            if (_hits.Count > CleanupThreshold)
            {
                Cleanup(now);
            }

            return acquired;
        }

        private void Cleanup(DateTimeOffset now)
        {
            foreach (var (key, queue) in _hits)
            {
                lock (queue)
                {
                    if (queue.Count == 0 || now - queue.Last() >= _window)
                    {
                        _ = _hits.TryRemove(key, out _);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Rate limiter abstraction.
    /// </summary>
    public interface IRateLimiter
    {
        /// <summary>
        /// Try to consume one permit for the client.
        /// </summary>
        /// <returns><c>false</c> if the client exceeded its quota.</returns>
        bool TryAcquire(string clientKey);
    }
}
