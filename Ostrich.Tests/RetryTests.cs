using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
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
                e.InnerExceptions.Cast<InvalidOperationException>().Count().Should().Be(3);
            }
            retries.Should().Be(3);
        }
    }
}
