namespace Ostrich.Tests
{
    using System;
    using System.Net;
    using Ostrich.Service;
    using Shouldly;

    public class HttpServerTests
    {
        public HttpServerTests()
        {
            using (var service = new HttpDiagnosticsService())
            {
                service.Start();
            }
        }

        [Input("/stats.txt")]
        [Input("/server_info.txt")]
        [Input("/graph_data")]
        [Input("/stats")]
        [Input("/server_info")]
        [Input("/ping")]
        [Input("/static/index.html")]
        [Input("/graph")]
        [Input("/report")]
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
