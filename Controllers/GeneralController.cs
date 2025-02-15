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
    [Authorize(Roles = "General")]
    [Route("api/[controller]")]
    [ApiController]
    public class GeneralController : ControllerBase
    {
        private readonly IGeneralRepository repository;

        public GeneralController(IGeneralRepository repository)
        {
            this.repository = repository;
        }


        [HttpGet]
        [Authorize(Roles = "General")]
        [Route("GetCountries")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Countries))]
        public async Task<IActionResult> GetCountries ()
        {
            var v_countries = await repository.GetCountries();
            if (v_countries == null) return NotFound();
            return Ok(v_countries);
        }

        [HttpGet]
        [Authorize(Roles = "General")]
        [Route("GetDistricts")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Districts))]
        public async Task<IActionResult> GetDistricts()
        {
            var v_districts = await repository.GetDistricts();
            if (v_districts == null) return NotFound();
            return Ok(v_districts);
        }

        [HttpGet]
        [Authorize(Roles = "General")]
        [Route("GetRegions")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Regions))]
        public async Task<IActionResult> GetRegions()
        {
            var v_regions = await repository.GetRegions();
            if (v_regions == null) return NotFound();
            return Ok(v_regions);
        }




    }
}
