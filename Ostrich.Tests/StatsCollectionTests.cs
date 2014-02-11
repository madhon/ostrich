using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Ostrich.Tests
{
    public class StatsCollectionTests : IDisposable
    {
        private readonly StatsCollection stats;

        public StatsCollectionTests()
        {
            stats = new StatsCollection();
        }

        public void Dispose()
        {
            stats.ClearAll();
        }

        [Fact]
        public void BasicCounters()
        {
            stats.Increment("widgets", 1);
            stats.Increment("wodgets", 12);
            stats.Increment("wodgets");


            stats.Counters.ToArray().Should().Contain(new List<KeyValuePair<string, AtomicLong>>
            {
                new KeyValuePair<string, AtomicLong>("widgets", new AtomicLong(1)),
                new KeyValuePair<string, AtomicLong>("wodgets", new AtomicLong(13))
            });
        }

        [Fact]
        public void NegativeCounters()
        {
            stats.Increment("widgets", 3);
            stats.Increment("widgets", -1);

            stats.Counters.ToArray().Should().Contain(new List<KeyValuePair<string, AtomicLong>>
            {
                new KeyValuePair<string, AtomicLong>("widgets", new AtomicLong(2)),
            });
        }

        [Fact]
        public void EmptyMetrics()
        {
            stats.RecordMetric("test", 0);
            var metric = stats.GetMetric("test");

            metric.Min.Should().Be(0);
            metric.Max.Should().Be(0);
            metric.Mean.Should().Be(0);
            metric.Count.Should().Be(1);
        }

        [Fact]
        public void MetricBasicMeanMinMax()
        {
            stats.RecordMetric("test", 1);
            stats.RecordMetric("test", 2);
            stats.RecordMetric("test", 3);

            var metric = stats.GetMetric("test");
            metric.Min.Should().Be(1);
            metric.Max.Should().Be(3);
            metric.Mean.Should().Be(2);
            metric.Count.Should().Be(3);
        }

        [Fact]
        public void BasicGauge()
        {
            stats.AddGauge("pi", new Gauge(() => Math.PI));
            stats.Gauges.Values.First().Value.Should().Be(Math.PI);
        }

        [Fact]
        public void ClearGauge()
        {
            stats.AddGauge("pi", new Gauge(() => Math.PI));
            stats.DeleteGauge("pi");
            stats.Gauges.Count.Should().Be(0);
        }

        [Fact]
        public void GaugesUpdate()
        {
            float seed = 0;
            stats.AddGauge("autoIncrement", new Gauge(() => seed++));
            stats.Gauges.First().Value.Value.Should().Be(0);
            stats.Gauges.First().Value.Value.Should().Be(1);
            stats.Gauges.First().Value.Value.Should().Be(2);
            stats.Gauges.First().Value.Value.Should().Be(3);
        }
    }
}