namespace Ostrich.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using NUnit.Framework;
    using Ostrich.Service;
    using Shouldly;

    [TestFixture]
    public class HttpServerTests
    {
        public HttpServerTests()
        {
            using (var service = new HttpDiagnosticsService())
            {
                service.Start();
            }
        }

        [TestCase("/stats.txt")]
        [TestCase("/server_info.txt")]
        [TestCase("/graph_data")]
        [TestCase("/stats")]
        [TestCase("/server_info")]
        [TestCase("/ping")]
        [TestCase("/static/index.html")]
        [TestCase("/graph")]
        [TestCase("/report")]
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
