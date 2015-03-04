using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Ostrich.Util;
using Xunit;

namespace Ostrich.Tests
{
    public class RetryTests
    {
        [Fact]
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
