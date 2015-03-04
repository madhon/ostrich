using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Shouldly;
using Ostrich.Service;
using Xunit;
using Xunit.Extensions;

namespace Ostrich.Tests
{
    public class HttpServerTests
    {
        public HttpServerTests()
        {
            using (var service = new HttpDiagnosticsService())
                service.Start();
        }

        [Theory]
        [InlineData("/stats.txt")]
        [InlineData("/server_info.txt")]
        [InlineData("/graph_data")]
        [InlineData("/stats")]
        [InlineData("/server_info")]
        [InlineData("/ping")]
        [InlineData("/static/index.html")]
        [InlineData("/graph")]
        [InlineData("/report")]
        public void HttpServerReturnsValidPage(String page)
        {
            using (var service = new HttpDiagnosticsService())
            {
                service.Start();
                StatusOfPage(page, service.Port).ShouldBe(WebExceptionStatus.Success);
            }
        }

        private WebExceptionStatus StatusOfPage(String url, int port)
        {
            using (var client = new WebClient())
            {
                try
                {
                    using (client.OpenRead(new Uri(String.Format("http://localhost:{0}{1}", port, url))))
                    {
                        return WebExceptionStatus.Success;
                    }
                }
                catch (WebException ex)
                {
                    return ex.Status;
                }
            }
        }
    }
}
