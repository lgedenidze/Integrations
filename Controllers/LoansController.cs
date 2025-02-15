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
    [Authorize(Roles = "Loans")]
    [Route("api/[controller]")]
    [ApiController]
    public class LoansController : ControllerBase
    {
        private readonly ILoansRepository repository;             

        public LoansController(ILoansRepository repository)
        {
            this.repository = repository;
        }


        [HttpPost]
        [Authorize(Roles = "Loans")]
        [Route("RegisterLoan")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoanRegistrationResult))]
        public async Task<IActionResult> RegisterLoan(LoanRegistrationData loanRegistrationData)
        {
            var v_result = await repository.RegisterLoan(loanRegistrationData);
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Loans")]
        [Route("CloseLoan")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CloseLoanResult))]
        public async Task<IActionResult> CloseLoan(int loanID)
        {
            var v_result = await repository.CloseLoan(loanID);
            return Ok(v_result);
        }

        [HttpPost]
        [Authorize(Roles = "Loans")]
        [Route("GiveOutLoan")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GiveOutLoanResult))]
        public async Task<IActionResult> GiveOutLoan(GiveOutLoanParameters giveOutLoanParameters)
        {
            var v_result = await repository.GiveOutLoan(giveOutLoanParameters);
            return Ok(v_result);
        }

        [HttpGet]
        [Authorize(Roles = "Loans")]
        [Route("GetLoanDetails/{loanID}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLoanDetailsResult))]
        public async Task<IActionResult> GetLoanDetails(int loanID )
        {
            GetLoanDetailsResult v_result = await repository.GetLoanDetails(loanID);
            if (v_result == null) return NotFound();
            return Ok(v_result);
        }



        [HttpPost]
        [Authorize(Roles = "Loans")]
        [Route("LoanRepayment")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> LoanRepayment(LoanRepaymentParams loanRepaymentParams)
        {
            var v_result = await repository.LoanRepayment(loanRepaymentParams);
            return Ok(v_result);
        }



        [HttpGet]
        [Authorize(Roles = "Loans")]
        [Route("GetLoanSchedule/{loanId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetLoanScheduleResult))]

        public async Task<IActionResult> GetLoanSchedule(int loanId)
        {
            var result = await repository.GetLoanSchedule(loanId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [HttpGet]
        [Authorize(Roles = "Loans")]
        [Route("GetMereTrancheDetails/{loanId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMereTrancheDetailsResult))]

        public async Task<IActionResult> GetMereTrancheDetails(int loanId)
        {
            var result = await repository.GetMereTrancheDetails(loanId);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "Loans")]
        [Route("ProlongMere")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GeneralResult))]
        public async Task<IActionResult> ProlongMere(ProlongMereParams prolongMereParams)
        {
            var v_result = await repository.ProlongMere(prolongMereParams);
            return Ok(v_result);
        }


    }
}
