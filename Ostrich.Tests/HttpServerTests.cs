namespace Ostrich.Tests
{
    using System;
    using System.Net;
    using Ostrich.Service;
    using Shouldly;
    using Xunit;
    using Xunit.Extensions;

    public class HttpServerTests
    {
        public HttpServerTests()
        {
            using (var service = new HttpDiagnosticsService())
            {
                service.Start();
            }
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
        public void HttpServerReturnsValidPage(string page)
        {
            using (var service = new HttpDiagnosticsService())
            {
                service.Start();
                StatusOfPage(page, service.Port).ShouldBe(WebExceptionStatus.Success);
            }
        }

        private WebExceptionStatus StatusOfPage(string url, int port)
        {
            using (var client = new WebClient())
            {
                try
                {
                    using (client.OpenRead(new Uri(string.Format("http://localhost:{0}{1}", port, url))))
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
