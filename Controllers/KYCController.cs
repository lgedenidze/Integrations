using Integrations.Model;
using Integrations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Integrations.Controllers
{
    [Authorize(Roles = "KYC")]
    [Route("api/[controller]")]
    [ApiController]
    public class KYCController : ControllerBase
    {
        private readonly IKYCRepository repository;             

        public KYCController(IKYCRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        [Authorize(Roles = "KYC")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(KYCResult))]
        public async Task<IActionResult> UpdateKYC(KYC kyc)
        {
            var v_result = await repository.UpdateKyc(kyc);
            return Ok(v_result);
        }


    }
}
