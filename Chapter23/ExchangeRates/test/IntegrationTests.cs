using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System.Net.Http;
using System.Threading.Tasks;
using ExchangeRates.Web.Tests.Helpers;
using Xunit;

namespace ExchangeRates.Web.Tests
{
    public class IntegrationTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _fixture;

        public IntegrationTests(WebApplicationFactory<Startup> fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task StatusMiddlewareReturnsPong()
        {
            HttpClient client = _fixture.CreateClient();

            var response = await client.GetAsync("/ping");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("pong", content);
        }

        /// <summary>
        /// Just for razor pages, I leave here just for consultation 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task HomePageReturnsHtml()
        {
            HttpClient client = _fixture.CreateClient();

            // Act
            var response = await client.GetAsync("/");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html", response.Content.Headers.ContentType.MediaType);

            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("Enter values and click convert", content);
        }

        [Fact]
        public async Task ConvertReturnsExpectedValue()
        {
            var customFactory = _fixture.WithWebHostBuilder(hostBuilder =>
            {
                hostBuilder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICurrencyConverter>();
                    services.AddSingleton<ICurrencyConverter, StubCurrencyConverter>();
                });
            });

            HttpClient client = customFactory.CreateClient();

            var response = await client.GetAsync("/api/currency");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("3", content);
        }
    }
}
