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

        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("ResetPin")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> ResetPin(ResetCardPinParams resetCardPinParams)
        {
            var v_result = await repository.ResetPin(resetCardPinParams);
            return Ok(v_result);
        }


        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("Block")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> Block(BlockCardParams blockCardParams)
        {
            var v_result = await repository.Block(blockCardParams);
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("UnBlock")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> Unblock(UnblockCardParams unblockCardParams)
        {
            var v_result = await repository.Unblock(unblockCardParams);
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("ChangeCurrencyPriority")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> ChangeCurrencyPriority(ChangeCardCurrencyPriorityParams changeCardCurrencyPriorityParams)
        {
            var v_result = await repository.ChangeCurrencyPriority(changeCardCurrencyPriorityParams);
            return Ok(v_result);
        }


        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("ChangeHighRiskTransactionStatus")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> ChangeHighRiskTransactionStatus(ChangeCardHighRiskTransactionStatusParams changeCardHighRiskTransactionStatusParams)
        {
            var v_result = await repository.ChangeHighRiskTransactionStatus(changeCardHighRiskTransactionStatusParams);
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Cards")]
        [Route("LinkInstantCardToAccount")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> LinkInstantCardToAccount(LinkInstantCardToAccountParams linkInstantCardToAccountParams)
        {
            var v_result = await repository.LinkInstantCardToAccount(linkInstantCardToAccountParams);
            return Ok(v_result);
        }





    }
}
