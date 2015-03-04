using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Ostrich.Tests
{
    public class MetricTests
    {
        [Fact]
        public void MinMeanMax()
        {
            var metric = new Metric();
            metric.Add(10);
            metric.Add(20);
            metric.Count.ShouldBe(2);
            metric.Min.ShouldBe(10);
            metric.Max.ShouldBe(20);
            metric.Mean.ShouldBe(15.0d);

            metric.Add(60);
            metric.Count.ShouldBe(3);
            metric.Min.ShouldBe(10);
            metric.Max.ShouldBe(60);
            metric.Mean.ShouldBe(30.0d);

            Histogram other = new Histogram {10, 20, 60};
            //metric.Histogram.ShouldContain(other.Get()); // TODO Fix
        }
    }
}
