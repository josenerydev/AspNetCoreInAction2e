using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace ExchangeRates.Web.Controllers
{
    [Route("api/[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly ICurrencyConverter _converter;

        public CurrencyController(ICurrencyConverter converter)
        {
            _converter = converter;
        }

        [HttpGet]
        public ActionResult<decimal> Convert(InputModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return _converter.ConvertToGbp(
                model.Value,
                model.ExchangeRate,
                model.DecimalPlaces);
        }
    }

    public class InputModel
    {
        public decimal Value { get; set; } = 0;

        [Range(0, double.MaxValue)]
        public decimal ExchangeRate { get; set; }

        [Range(0, int.MaxValue)]
        public int DecimalPlaces { get; set; }
    }
}
