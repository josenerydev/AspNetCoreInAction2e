using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

using System.Net.Http;
using System.Threading.Tasks;

using Xunit;

namespace ExchangeRates.Web.Tests
{
    /// <summary>
    /// Integration test for the <see cref="StatusMiddleware"/>
    /// </summary>
    public class StatusMiddlewareTestHostTests
    {
        [Fact]
        public async Task StatusMiddlewareReturnsPong()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                {
                    webHost.UseTestServer();
                    webHost.Configure(app =>
                        app.UseMiddleware<StatusMiddleware>()
                    );
                });

            IHost host = await hostBuilder.StartAsync();
            HttpClient client = host.GetTestClient();

            var response = await client.GetAsync("/ping");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("pong", content);
        }
    }
}
