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
    [Authorize(Roles = "Cards")]
    [Route("api/[controller]")]
    [ApiController]
    public class CardsController : ControllerBase
    {
        private readonly ICardsRepository repository;             

        public CardsController(ICardsRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("AddCard")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> AddCard(AddCardParams addCardParams)
        {
            var v_result = await repository.AddCard(addCardParams);
            return Ok(v_result);
        }
 




    }
}
