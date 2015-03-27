namespace Ostrich.Tests
{
    using System;
    using Shouldly;

    public class HistogramTests
    {
        private Histogram histogram = new Histogram();
        private Histogram histogram2 = new Histogram();

        public HistogramTests()
        {
            histogram.Clear();
            histogram2.Clear();
        }

        public void CanFindTheRightBucketForTimings()
        {
            histogram.Add(0);
            histogram.Get(true)[0].ShouldBe(1);
            histogram.Add(9999999);
            var h = histogram.Get(true);
            h[h.Length - 1].ShouldBe(1);
            histogram.Add(1);
            histogram.Get(true)[1].ShouldBe(1);
            histogram.Add(2);
            histogram.Get(true)[2].ShouldBe(1);
            histogram.Add(11);
            histogram.Add(12);
            histogram.Add(13);
            histogram.Get(true)[8].ShouldBe(3);
        }

        public void FindHistogramCutoffsForVariousPercentages()
        {
            for (int i = 0; i < 1000; i++)
            {
                histogram.Add(i);
            }

            Histogram.BinarySearch(histogram.GetPercentile(0.0d)).ShouldBe(0);
            Histogram.BinarySearch(histogram.GetPercentile(0.5d)).ShouldBe(22);
            Histogram.BinarySearch(histogram.GetPercentile(0.9d)).ShouldBe(24);
            Histogram.BinarySearch(histogram.GetPercentile(0.99d)).ShouldBe(25);
            Histogram.BinarySearch(histogram.GetPercentile(1.0d)).ShouldBe(25);
        }

        public void Merge()
        {
            for (int i = 0; i < 50; i++)
            {
                histogram.Add(i * 10);
                histogram2.Add(i * 10);
            }

            var origTotal = histogram.Total;
            histogram.Merge(histogram2);
            histogram.Total.ShouldBe(origTotal + histogram2.Total);
            var stats = histogram.Get(true);
            var stats2 = histogram2.Get(true);
            for (int i = 0; i < 50; i++)
            {
                var bucket = Histogram.BinarySearch(i * 10);
                stats[bucket].ShouldBe(2 * stats2[bucket]);
            }
        }

        public void HandleAVeryLargeTiming()
        {
            histogram.Add(100000000);
            histogram.GetPercentile(1.0d).ShouldBe(int.MaxValue);
        }
    }
}
