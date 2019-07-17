namespace Ostrich.Tests
{
    using System;
    using System.Linq;
    using Ostrich.Util;
    using Shouldly;
    using NUnit.Framework;

    [TestFixture]
    public class RetryTests
    {
        [Test]
        public void WillRetryOnly3Times()
        {
            int retries = 0;
            try
            {
                Retry.AtMost(3).Try(() =>
                {
                    retries++;
                    throw new InvalidOperationException();
                });
            }
            catch (AggregateException e)
            {
                e.InnerExceptions.Cast<InvalidOperationException>().Count().ShouldBe(3);
            }

            retries.ShouldBe(3);
        }
    }
}
