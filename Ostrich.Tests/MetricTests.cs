using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
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
            metric.Count.Should().Be(2);
            metric.Min.Should().Be(10);
            metric.Max.Should().Be(20);
            metric.Mean.Should().Be(15.0d);

            metric.Add(60);
            metric.Count.Should().Be(3);
            metric.Min.Should().Be(10);
            metric.Max.Should().Be(60);
            metric.Mean.Should().Be(30.0d);

            Histogram other = new Histogram {10, 20, 60};
            metric.Histogram.Should().Contain(other.Get());
        }
    }
}
