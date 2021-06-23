using ExchangeRates.Web.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

using Xunit;

namespace ExchangeRates.Web.Tests
{
    /// <summary>
    /// Unit tests for the <see cref="CurrencyController"/>
    /// </summary>
    public class CurrencyControllerTests
    {
        [Fact]
        public void Convert_ReturnsValue()
        {
            var converter = new CurrencyConverter();
            var controller = new CurrencyController(converter);
            var model = new InputModel
            {
                Value = 1,
                ExchangeRate = 2,
                DecimalPlaces = 2
            };

            var result = controller.Convert(model);

            Assert.NotNull(result);

            var convertedResult = ((IConvertToActionResult)result).Convert();
            var objectResult = Assert.IsType<ObjectResult>(convertedResult);
            var statusCode = objectResult.StatusCode ?? 200;

            Assert.Equal(200, statusCode);
        }

        [Fact]
        public void Convert_ReturnsBadRequestWhenInvalid()
        {
            var converter = new CurrencyConverter();
            var controller = new CurrencyController(converter);
            var model = new InputModel
            {
                Value = 1,
                ExchangeRate = -2,
                DecimalPlaces = 2
            };

            controller.ModelState.AddModelError(
                nameof(model.ExchangeRate),
                "Exchange rate must be greater than zero");

            var result = controller.Convert(model);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);

            var statusCode = badRequest.StatusCode ?? 200;

            Assert.Equal(400, statusCode);
        }
    }
}
