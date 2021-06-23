using ExchangeRates.Web.Tests.Helpers;

using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace ExchangeRates.Web.Tests
{
    /// <summary>
    /// These tests use the <see cref="CustomWebApplicationFactory"/>
    /// </summary>
    public class CustomWebApplicationFactoryTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _fixture;

        public CustomWebApplicationFactoryTests(CustomWebApplicationFactory fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task ConvertReturnsExpectedValue()
        {
            HttpClient client = _fixture.CreateClient();

            var response = await client.GetAsync("/api/currency");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("3", content);
        }

        [Fact]
        public async Task CanOverrideTestService()
        {
            var customFactory = _fixture.WithWebHostBuilder(hostBuilder =>
            {
                hostBuilder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICurrencyConverter>();
                    services.AddSingleton<ICurrencyConverter, OtherCurrencyConverter>();
                });
            });

            HttpClient client = customFactory.CreateClient();

            var response = await client.GetAsync("/api/currency");

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Equal("5", content);
        }

        public class OtherCurrencyConverter : ICurrencyConverter
        {
            public decimal ConvertToGbp(decimal value, decimal rate, int dps) => 5;
        }
    }
}
