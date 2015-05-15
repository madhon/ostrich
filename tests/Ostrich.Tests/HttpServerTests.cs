namespace Ostrich.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using Ostrich.Service;
    using Shouldly;
    using Xunit;

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
                StatusOfPage(page, service.Port).ShouldBe(HttpStatusCode.OK);
            }
        }

        private HttpStatusCode StatusOfPage(string url, int port)
        {
            var page = new Uri(string.Format("http://localhost:{0}{1}", port, url));

            using (var client = new HttpClient())
            {
                var response = client.GetAsync(page).Result;
                return response.StatusCode;
            }
        }
    }
}
