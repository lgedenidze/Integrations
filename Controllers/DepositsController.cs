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
    [Authorize(Roles = "Deposits")]
    [Route("api/[controller]")]
    [ApiController]
    public class DepositsController : ControllerBase
    {
        private readonly IDepositsRepository repository;             

        public DepositsController(IDepositsRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        [Authorize(Roles = "Deposits")]
        [Route("OpenDeposit")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> OpenDeposit(OpenDepositParams openDepositParams)
        {
            var v_result = await repository.OpenDeposit(openDepositParams);
            return Ok(v_result);
        }

        [HttpGet]
        [Authorize(Roles = "Deposits")]        
        [Route("GetDepositPreOpeningDetails/{customerId}/{depositType}/{currency}/{periodInMonths}/{depositSubType?}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetDepositPreOpeningDetailsResult))]
        public async Task<IActionResult> GetDepositPreOpeningDetails(int    customerId,
                                                                     string depositType,
                                                                     string currency,
                                                                     int    periodInMonths,
                                                                     string depositSubType = "")
        {
            GetDepositPreOpeningDetailsResult v_result = await repository.GetDepositPreOpeningDetails(customerId,
                                                                                          depositType,
                                                                                          currency,
                                                                                          periodInMonths,
                                                                                          depositSubType);
            if (v_result == null) return NotFound();
            return Ok(v_result);
        }


    }
}
