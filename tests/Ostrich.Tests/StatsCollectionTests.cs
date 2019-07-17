﻿namespace Ostrich.Tests
{
    using System;
    using System.Linq;
    using Shouldly;
    using NUnit.Framework;

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

        [Test]
        public void BasicCounters()
        {
            stats.Increment("widgets", 1);
            stats.Increment("wodgets", 12);
            stats.Increment("wodgets");

            //// TODO Fix this
            ////stats.Counters.ToList().ShouldContain(new List<KeyValuePair<string, AtomicLong>>
            ////{
            ////    new KeyValuePair<string, AtomicLong>("widgets", new AtomicLong(1)),
            ////    new KeyValuePair<string, AtomicLong>("wodgets", new AtomicLong(13))
            ////});
        }

        [Test]
        public void NegativeCounters()
        {
            stats.Increment("widgets", 3);
            stats.Increment("widgets", -1);

            //// TODO Fix this
            ////stats.Counters.ToArray().ShouldContain(new List<KeyValuePair<string, AtomicLong>>
            ////{
            ////    new KeyValuePair<string, AtomicLong>("widgets", new AtomicLong(2)),
            ////});
        }

        [Test]
        public void EmptyMetrics()
        {
            stats.RecordMetric("test", 0);
            var metric = stats.GetMetric("test");

            metric.Min.ShouldBe(0);
            metric.Max.ShouldBe(0);
            metric.Mean.ShouldBe(0);
            metric.Count.ShouldBe(1);
        }

        [Test]
        public void MetricBasicMeanMinMax()
        {
            stats.RecordMetric("test", 1);
            stats.RecordMetric("test", 2);
            stats.RecordMetric("test", 3);

            var metric = stats.GetMetric("test");
            metric.Min.ShouldBe(1);
            metric.Max.ShouldBe(3);
            metric.Mean.ShouldBe(2);
            metric.Count.ShouldBe(3);
        }

        [Test]
        public void BasicGauge()
        {
            stats.AddGauge("pi", new Gauge(() => Math.PI));
            stats.Gauges.Values.First().Value.ShouldBe(Math.PI);
        }

        [Test]
        public void ClearGauge()
        {
            stats.AddGauge("pi", new Gauge(() => Math.PI));
            stats.DeleteGauge("pi");
            stats.Gauges.Count.ShouldBe(0);
        }

        [Test]
        public void GaugesUpdate()
        {
            float seed = 0;
            stats.AddGauge("autoIncrement", new Gauge(() => seed++));
            stats.Gauges.First().Value.Value.ShouldBe(0);
            stats.Gauges.First().Value.Value.ShouldBe(1);
            stats.Gauges.First().Value.Value.ShouldBe(2);
            stats.Gauges.First().Value.Value.ShouldBe(3);
        }
    }
}