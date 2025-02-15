using Integrations.Model;
using Integrations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Controllers
{
    [Authorize(Roles = "Currencies,Website")]
    [Route("api/[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrenciesRepository repository;             

        public CurrenciesController(ICurrenciesRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Currencies")]
        [Route("GetExchangeRate/{baseCurrency}/{targetCurrency}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExchangeRate))]
        public async Task<IActionResult> GetExchangeRate(string baseCurrency, string targetCurrency)
        {
            var v_exchange_rate = await repository.GetExchangeRate(baseCurrency, targetCurrency);
            if (v_exchange_rate == null) return NotFound();
            return Ok(v_exchange_rate);
        }



        [HttpGet]
        [Authorize(Roles = "Website")]
        [Route("GetExchangeRates")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CurrencyExchanges))]
        public async Task<IActionResult> GetExchangeRates()
        {
            var v_exchange_rate = await repository.GetExchangeRates();
            if (v_exchange_rate == null) return NotFound();
            return Ok(v_exchange_rate);
        }




    }
}
