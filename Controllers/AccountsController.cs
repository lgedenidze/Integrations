using Integrations.Model;
 using Integrations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Integrations.Model.Enums.BalanceType;

namespace Integrations.Controllers
{
    [Authorize(Roles = "Accounts, UMTS, Balance")]
    // [Authorize(Roles = "Accounts, aaa")]  // კონტროლერში უნდა მიეთითოს ყველა შესაძლო როლი
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsRepository repository;

        public AccountsController(IAccountsRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        //[Authorize(Roles = "Accounts")] თუ გვინდა მეთოდების მიხედვით გავყოთ უფლებებ, მეთოდში უნდა დავადოთ ცალკე 
        [Authorize(Roles = "Accounts,UMTS")]
        [Route("GetAccounts/{customerId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Accounts))]
        public async Task<IActionResult> GetAccounts(int customerId)
        {
            var v_accounts = await repository.GetAccounts(customerId);
            if (v_accounts == null) return NotFound();
            return Ok(v_accounts);
        }

        [HttpGet]
        [Authorize(Roles = "Accounts")]
        [Route("GetAccountsForCrediting/")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AccountsForCrediting))]
        public async Task<IActionResult> GetAccountsForCrediting([FromQuery] string personalNo, [FromQuery] string phoneNumber, [FromQuery] string iban)
        {
            if (phoneNumber == null && personalNo == null && iban == null)
            {
                return BadRequest("Missing required parameters: phoneNumber, personalNo, or iban.");
            }

            if ((phoneNumber != null ? 1 : 0) + (personalNo != null ? 1 : 0) + (iban != null ? 1 : 0) > 1)
            {
                return BadRequest("More than one parameter has a value. Please pass only one.");
            }


            var v_accounts = await repository.GetAccountsForCrediting(phoneNumber, iban, personalNo);
            if (v_accounts == null) return NotFound();
            return Ok(v_accounts);

        }

        [HttpGet]
        [Authorize(Roles = "UMTS")]
        [Route("GetAccountsForUMTS/{customerId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Accounts))]
        public async Task<IActionResult> GetAccountsForUMTS(int customerId)
        {
            var v_accounts = await repository.GetAccounts(customerId);
            if (v_accounts == null) return NotFound();
            v_accounts.accounts = v_accounts.accounts.Where(x => x.debitEnabled == true && x.creditEnabled == true).ToList();
            return Ok(v_accounts);
        }



        [HttpPost]
        [Authorize(Roles = "Accounts")]
        [Route("GetBalance/{accountId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetBalanceResult))]
        public async Task<IActionResult> GetBalance(string accountId )
        {
            var v_getBalanceResult = await repository.GetBalance(accountId );
            if (v_getBalanceResult == null) return NotFound();
            return Ok(v_getBalanceResult);
        }

        [HttpPost]
        [Authorize(Roles = "Balance")]
        [Route("SendBalanceValidationToRabbitMQ/{accountId}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> SendBalanceValidationToRabbitMQ(string accountId)
        {
            var v_result = await repository.SendBalanceValidationToRabbitMQ(accountId);
            return Ok(v_result);
        }


        [HttpPost]
        [Authorize(Roles = "Accounts")]
        [Route("OpenAccount")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OpenAccountResult))]
        public async Task<IActionResult> OpenAccount(OpenAccountParams openAccountParams)
        {
            var v_result = await repository.OpenAccount(openAccountParams);
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Accounts")]
        [Route("AddCurrency")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddCurrencyResult))]
        public async Task<IActionResult> AddCurrency(AddCurrencyParams addCurrencyParams)
        {
            var v_result = await repository.AddCurrency(addCurrencyParams);
            return Ok(v_result);
        }

    }
}
