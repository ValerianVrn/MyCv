using MyCv.Tailor.Api.Services;

namespace MyCv.Tailor.Api.Tests
{
    [TestClass]
    public sealed class RateLimiterTest
    {
        private sealed class ManualTimeProvider : TimeProvider
        {
            public DateTimeOffset Now { get; set; } = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

            public override DateTimeOffset GetUtcNow() => Now;
        }

        [TestMethod]
        public void TryAcquireBlocksAfterPermitLimit()
        {
            var limiter = new RateLimiter(new ManualTimeProvider(), permitLimit: 3, window: TimeSpan.FromMinutes(1));

            Assert.IsTrue(limiter.TryAcquire("a"));
            Assert.IsTrue(limiter.TryAcquire("a"));
            Assert.IsTrue(limiter.TryAcquire("a"));
            Assert.IsFalse(limiter.TryAcquire("a"));
        }

        [TestMethod]
        public void TryAcquireIsPerClient()
        {
            var limiter = new RateLimiter(new ManualTimeProvider(), permitLimit: 1, window: TimeSpan.FromMinutes(1));

            Assert.IsTrue(limiter.TryAcquire("a"));
            Assert.IsFalse(limiter.TryAcquire("a"));
            Assert.IsTrue(limiter.TryAcquire("b"));
        }

        [TestMethod]
        public void TryAcquireAllowsAgainAfterWindow()
        {
            var time = new ManualTimeProvider();
            var limiter = new RateLimiter(time, permitLimit: 1, window: TimeSpan.FromMinutes(1));

            Assert.IsTrue(limiter.TryAcquire("a"));
            Assert.IsFalse(limiter.TryAcquire("a"));

            time.Now = time.Now.AddMinutes(1);

            Assert.IsTrue(limiter.TryAcquire("a"));
        }
    }
}
