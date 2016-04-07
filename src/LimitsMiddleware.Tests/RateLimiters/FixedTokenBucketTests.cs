namespace LimitsMiddleware.RateLimiters
{
    using System;
    using System.Threading.Tasks;
    using Shouldly;
    using Xunit;

    public class FixedTokenBucketTests
    {
        private const int MaxTokens = 10;
        private const long RefillInterval = 10;
        private const int NLessThanMax = 2;
        private const int NGreaterThanMax = 12;
        private const int Cumulative = 2;
        private readonly FixedTokenBucket _bucket;
        private GetUtcNow _getUtcNow = () => SystemClock.GetUtcNow();

        public FixedTokenBucketTests()
        {
            _bucket = new FixedTokenBucket(() => MaxTokens, () => _getUtcNow());
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithNTokensLessThanMax_ReturnsFalse()
        {
            TimeSpan waitTime;
            var shouldThrottle = _bucket.ShouldThrottle(NLessThanMax, out waitTime);

            shouldThrottle.ShouldBeFalse();
            _bucket.CurrentTokenCount.ShouldBe(MaxTokens - NLessThanMax);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledCumulativeNTimesIsLessThanMaxTokens_ReturnsFalse()
        {
            for (var i = 0; i < Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax).ShouldBeFalse();
            }

            var tokens = _bucket.CurrentTokenCount;

            tokens.ShouldBe(MaxTokens - (Cumulative*NLessThanMax));
        }

        [Fact]
        public void ShouldThrottle_WhenCalledCumulativeNTimesIsGreaterThanMaxTokens_ReturnsTrue()
        {
            for (var i = 0; i < Cumulative; i++)
            {
                _bucket.ShouldThrottle(NGreaterThanMax).ShouldBeTrue();
            }

            var tokens = _bucket.CurrentTokenCount;

            tokens.ShouldBe(MaxTokens);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithNLessThanMaxSleepNLessThanMax_ReturnsFalse()
        {
            _getUtcNow = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = _getUtcNow();

            var before = _bucket.ShouldThrottle(NLessThanMax);
            var tokensBefore = _bucket.CurrentTokenCount;
            before.ShouldBeFalse();
            tokensBefore.ShouldBe(MaxTokens - NLessThanMax);

            _getUtcNow = () => virtualNow.AddSeconds(RefillInterval);

            var after = _bucket.ShouldThrottle(NLessThanMax);
            var tokensAfter = _bucket.CurrentTokenCount;

            after.ShouldBeFalse();
            tokensAfter.ShouldBe(MaxTokens - NLessThanMax);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithNGreaterThanMaxSleepNGreaterThanMax_ReturnsTrue()
        {
            _getUtcNow = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = _getUtcNow();

            var before = _bucket.ShouldThrottle(NGreaterThanMax);
            var tokensBefore = _bucket.CurrentTokenCount;

            before.ShouldBeTrue();
            tokensBefore.ShouldBe(MaxTokens);

            _getUtcNow = () => virtualNow.AddSeconds(RefillInterval);

            var after = _bucket.ShouldThrottle(NGreaterThanMax);
            var tokensAfter = _bucket.CurrentTokenCount;
            after.ShouldBeTrue();
            tokensAfter.ShouldBe(MaxTokens);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithNLessThanMaxSleepCumulativeNLessThanMax()
        {
            _getUtcNow = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = _getUtcNow();

            long sum = 0;
            for (var i = 0; i < Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax).ShouldBeFalse();
                sum += NLessThanMax;
            }
            var tokensBefore = _bucket.CurrentTokenCount;
            tokensBefore.ShouldBe(MaxTokens - sum);

            _getUtcNow = () => virtualNow.AddSeconds(RefillInterval);

            for (var i = 0; i < Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax).ShouldBeFalse();
            }
            var tokensAfter = _bucket.CurrentTokenCount;
            tokensAfter.ShouldBe(MaxTokens - sum);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithCumulativeNLessThanMaxSleepCumulativeNGreaterThanMax()
        {
            _getUtcNow = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = _getUtcNow();

            long sum = 0;
            for (var i = 0; i < Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax).ShouldBeFalse();
                sum += NLessThanMax;
            }
            var tokensBefore = _bucket.CurrentTokenCount;
            tokensBefore.ShouldBe(MaxTokens - sum);

            _getUtcNow = () => virtualNow.AddSeconds(RefillInterval);

            for (var i = 0; i < 3*Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax);
            }

            var after = _bucket.ShouldThrottle(NLessThanMax);
            var tokensAfter = _bucket.CurrentTokenCount;

            after.ShouldBeTrue();
            tokensAfter.ShouldBeLessThan(NLessThanMax);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithCumulativeNGreaterThanMaxSleepCumulativeNLessThanMax()
        {
            _getUtcNow = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = _getUtcNow();

            for (var i = 0; i < 3*Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax);
            }

            var before = _bucket.ShouldThrottle(NLessThanMax);
            var tokensBefore = _bucket.CurrentTokenCount;

            before.ShouldBeTrue();
            tokensBefore.ShouldBeLessThan(NLessThanMax);

            _getUtcNow = () => virtualNow.AddSeconds(RefillInterval);

            long sum = 0;
            for (var i = 0; i < Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax).ShouldBeFalse();
                sum += NLessThanMax;
            }

            var tokensAfter = _bucket.CurrentTokenCount;
            tokensAfter.ShouldBe(MaxTokens - sum);
        }

        [Fact]
        public void ShouldThrottle_WhenCalledWithCumulativeNGreaterThanMaxSleepCumulativeNGreaterThanMax()
        {
            _getUtcNow = () => new DateTime(2014, 2, 27, 0, 0, 0, DateTimeKind.Utc);
            var virtualNow = _getUtcNow();

            for (var i = 0; i < 3*Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax);
            }

            var before = _bucket.ShouldThrottle(NLessThanMax);
            var tokensBefore = _bucket.CurrentTokenCount;

            before.ShouldBeTrue();
            tokensBefore.ShouldBeLessThan(NLessThanMax);

            _getUtcNow = () => virtualNow.AddSeconds(RefillInterval);

            for (var i = 0; i < 3*Cumulative; i++)
            {
                _bucket.ShouldThrottle(NLessThanMax);
            }
            var after = _bucket.ShouldThrottle(NLessThanMax);
            var tokensAfter = _bucket.CurrentTokenCount;

            after.ShouldBeTrue();
            tokensAfter.ShouldBeLessThan(NLessThanMax);
        }

        [Fact]
        public async Task ShouldThrottle_WhenThread1NLessThanMaxAndThread2NLessThanMax()
        {
            var task1 = Task.Run(() => 
            {
                var throttle = _bucket.ShouldThrottle(NLessThanMax);
                throttle.ShouldBeFalse();
            });

            var task2 = Task.Run(() =>
            {
                var throttle = _bucket.ShouldThrottle(NLessThanMax);
                throttle.ShouldBeFalse();
            });

            await Task.WhenAll(task1, task2);

            _bucket.CurrentTokenCount.ShouldBe(MaxTokens - 2*NLessThanMax);
        }

        [Fact]
        public async Task ShouldThrottle_Thread1NGreaterThanMaxAndThread2NGreaterThanMax()
        {
            var shouldThrottle = _bucket.ShouldThrottle(NGreaterThanMax);
            shouldThrottle.ShouldBeTrue();

            var task1 = Task.Run(() =>
            {
                var throttle = _bucket.ShouldThrottle(NGreaterThanMax);
                throttle.ShouldBeTrue();
            });

            var task2 = Task.Run(() =>
            {
                var throttle = _bucket.ShouldThrottle(NGreaterThanMax);
                throttle.ShouldBeTrue();
            });

            await Task.WhenAll(task1, task2);

            _bucket.CurrentTokenCount.ShouldBe(MaxTokens);
        }
    }
}