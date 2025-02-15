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
    [Authorize(Roles = "Transactions, UMTS")]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsRepository repository;             

        public TransactionsController(ITransactionsRepository repository)
        {
            this.repository = repository;
        }

        [HttpPost]
        [Authorize(Roles = "Transactions, UMTS")]
        [Route("TransferMoney")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TransferMoneyResult))]
        public async Task<IActionResult> TransferMoney(TransferMoneyData transferMoneyData)
        {
            var v_result = await repository.TransferMoney(transferMoneyData);
            return Ok(v_result);
        }

        [HttpGet]
        [Authorize(Roles = "Transactions")]
        [Route("GetTransferFee/{transferType}/{debitAccountId}/{creditAccountId}/{isStp}/{amount}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTransferFeeResult))]
        public async Task<IActionResult> GetTransferFee(string transferType, string debitAccountId, string creditAccountId, bool isStp, decimal amount)
        {
            var v_transfer_fee = await repository.GetTransferFee(transferType, debitAccountId, creditAccountId, isStp, amount);
            if (v_transfer_fee == null) return NotFound();
            return Ok(v_transfer_fee);
        }


        [HttpGet]
        [Authorize(Roles = "Transactions")]
        [Route("GetTreasuryCodeDescr/{treasuryCode}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetTreasuryCodeDescrResult))]
        public async Task<IActionResult> GetTreasuryCodeDescr(string treasuryCode)
        {
            var v_result = await repository.GetTreasuryCodeDescr(treasuryCode);
             if (v_result == null) return NotFound();
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Transactions")]
        [Route("Authorize")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> Authorize(AuthorizeParams authorizeParams)
        {
            var v_result = await repository.Authorize(authorizeParams);
            return Ok(v_result);
        }


    }
}
