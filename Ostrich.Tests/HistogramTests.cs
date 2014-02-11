using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Ostrich.Tests
{
    public class HistogramTests
    {
        Histogram histogram = new Histogram();
        Histogram histogram2 = new Histogram();

        public HistogramTests()
        {
            histogram.Clear();
            histogram2.Clear();
        }

        [Fact]
        public void CanFindTheRightBucketForTimings()
        {
            histogram.Add(0);
            histogram.Get(true)[0].Should().Be(1);
            histogram.Add(9999999);
            var h = histogram.Get(true);
            h[h.Length - 1].Should().Be(1);
            histogram.Add(1);
            histogram.Get(true)[1].Should().Be(1);
            histogram.Add(2);
            histogram.Get(true)[2].Should().Be(1);
            histogram.Add(11);
            histogram.Add(12);
            histogram.Add(13);
            histogram.Get(true)[8].Should().Be(3);
        }

        [Fact]
        public void FindHistogramCutoffsForVariousPercentages()
        {
            for (int i = 0; i < 1000; i++) histogram.Add(i);

            Histogram.BinarySearch(histogram.GetPercentile(0.0d)).Should().Be(0);
            Histogram.BinarySearch(histogram.GetPercentile(0.5d)).Should().Be(22);
            Histogram.BinarySearch(histogram.GetPercentile(0.9d)).Should().Be(24);
            Histogram.BinarySearch(histogram.GetPercentile(0.99d)).Should().Be(25);
            Histogram.BinarySearch(histogram.GetPercentile(1.0d)).Should().Be(25);
        }

        [Fact]
        public void Merge()
        {
            for (int i = 0; i < 50; i++)
            {
                histogram.Add(i * 10);
                histogram2.Add(i * 10);
            }

            var origTotal = histogram.Total;
            histogram.Merge(histogram2);
            histogram.Total.Should().Be(origTotal + histogram2.Total);
            var stats = histogram.Get(true);
            var stats2 = histogram2.Get(true);
            for (int i = 0; i < 50; i++)
            {
                var bucket = Histogram.BinarySearch(i * 10);
                stats[bucket].Should().Be(2*stats2[bucket]);
            }
        }

        [Fact]
        public void HandleAVeryLargeTiming()
        {
            histogram.Add(100000000);
            histogram.GetPercentile(1.0d).Should().Be(Int32.MaxValue);
        }
    }
}
