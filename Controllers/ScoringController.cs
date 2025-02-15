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
    [Authorize(Roles = "Scoring")]
    [Route("api/[controller]")]
    [ApiController]
    public class ScoringController : ControllerBase
    {
        private readonly IScoringRepository repository;             

        public ScoringController(IScoringRepository repository)
        {
            this.repository = repository;//new ScoringRepository();
        }

        [HttpGet]
        [Authorize(Roles = "Scoring")]
        [Route("{personalNo}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ScoringData>))]
        public async Task<IActionResult> GetScoringInternalData(string personalNo)
        {
            
            var v_scoringData =  await repository.GetScoringData(personalNo);
            if (v_scoringData == null) return NotFound();
            return Ok(v_scoringData);
        }
    }
}
