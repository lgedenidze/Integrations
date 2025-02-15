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
 


       

    }
}
