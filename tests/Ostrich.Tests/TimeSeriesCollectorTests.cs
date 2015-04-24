namespace Ostrich.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Ostrich.Service;
    using Ostrich.Util;
    using Shouldly;
    using Xunit;

    public class TimeSeriesCollectorTests : IDisposable
    {
        private StatsCollection stats;
        private StatsListener listener;
        private TimeSeriesCollector collector;
        private IDisposable timeFreeze;

        public TimeSeriesCollectorTests()
        {
            stats = new StatsCollection();
            listener = new StatsListener(stats);
            collector = new TimeSeriesCollector(listener, false);
            timeFreeze = SystemClock.Freeze();
        }

        public void Dispose()
        {
            timeFreeze.Dispose();
            collector.Dispose();
        }

        [Fact]
        public void StatsIncr()
        {
            stats.Increment("cats");
            stats.Increment("dogs", 3);

            collector.Periodic(null);

            stats.Increment("dogs", 60000);
            SystemClock.Advance(TimeSpan.FromMinutes(1));

            collector.Periodic(null);

            var series = collector.Get("counter:dogs", null);
            series.ShouldNotBe(null);
            series.Count().ShouldBe(60);
           
            var seconds = SystemClock.Minus(TimeSpan.FromMinutes(2d)).MillisFromEpoch() / 1000;
            AssertClose(new List<long>(new[] { seconds, 0 }), series.ElementAt(57));
            seconds = SystemClock.Minus(TimeSpan.FromMinutes(1d)).MillisFromEpoch() / 1000;
            AssertClose(new List<long>(new[] { seconds, 3 }), series.ElementAt(58));
            AssertClose(new List<long>(new[] { SystemClock.Seconds(), 60000 }), series.ElementAt(59));
        }

        [Fact]
        public void StatsWithCounterUpdate()
        {
            stats.Increment("tps", 10);

            collector.Periodic(null);
            SystemClock.Advance(TimeSpan.FromMinutes(1));
            stats.Increment("tps", 5);
            collector.Periodic(null);

            var series = collector.Get("counter:tps", null);
            series.ShouldNotBe(null);
            series.ShouldNotBe(null);
            series.Count().ShouldBe(60);
            var seconds = SystemClock.Minus(TimeSpan.FromMinutes(2d)).MillisFromEpoch() / 1000;
            AssertClose(new List<long>(new[] { seconds, 0 }), series.ElementAt(57));
            seconds = SystemClock.Minus(TimeSpan.FromMinutes(1d)).MillisFromEpoch() / 1000;
            AssertClose(new List<long>(new[] { seconds, 10 }), series.ElementAt(58));
            AssertClose(new List<long>(new[] { SystemClock.Seconds(), 5 }), series.ElementAt(59));
        }

        [Fact]
        public void SpecificTimingProfiles()
        {
            stats.RecordMetric("run", 5);
            stats.RecordMetric("run", 10);
            stats.RecordMetric("run", 15);
            stats.RecordMetric("run", 20);

            collector.Periodic(null);

            var series = collector.Get("timing:run", null);
            series.ShouldNotBe(null);
            series.Count().ShouldBe(60);
            AssertClose(new List<long>(new[] { SystemClock.Seconds(), 6, 10, 17, 23, 23, 23, 23, 23 }), series.ElementAt(59).ToArray());

            series = collector.Get("timing:run", new[] { 0, 2 });
            series.ShouldNotBe(null);
            series.Count().ShouldBe(60);
            AssertClose(new List<long>(new[] { SystemClock.Seconds(), 6, 17 }), series.ElementAt(59).ToArray());

            series = collector.Get("timing:run", new[] { 1, 7 });
            series.ShouldNotBe(null);
            series.Count().ShouldBe(60);
            AssertClose(new List<long>(new[] { SystemClock.Seconds(), 10, 23 }), series.ElementAt(59).ToArray());
        }

        // punt timestamp division rounding temporarily
        private void AssertClose(IEnumerable<long> a, IEnumerable<long> b)
        {
            a.Count().ShouldBe(b.Count());
            for (int i = 0; i < a.Count(); i++)
            {
                var itemA = a.ElementAt(i);
                var itemB = b.ElementAt(i);

                if (itemA + 1 == itemB)
                {
                    itemA = itemA + 1;
                }
                else if (itemB + 1 == itemA)
                {
                    itemB = itemB + 1;
                }

                itemA.ShouldBe(itemB);
            }
        }
    }
}
