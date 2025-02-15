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
    [Authorize(Roles = "Customers, UMTS")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomersRepository repository;             

        public CustomersController(ICustomersRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        [Authorize(Roles = "Customers")]
        [Route("CheckCustomer/{personalNumber}/{financialNumber}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckCustomerResult))]
        public async Task<IActionResult> CheckCustomer(string personalNumber, string financialNumber)
        {
            var v_result = await repository.CustomerExists(personalNumber, financialNumber);
            if (v_result == null) return NotFound();
            return Ok(v_result);
        }


        [HttpPost]
        [Authorize(Roles = "Customers")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CreateCustomerResult))]
        public async Task<IActionResult> RegisterCustomer(Customer customer)
        {
            var v_result = await repository.CreateCustomer(customer);
            return Ok(v_result);
        }

        [HttpPatch("{customerId}")]
        [Authorize(Roles = "Customers")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        public async Task<IActionResult> UpdateCustomer(int customerId, [FromBody] JsonPatchDocument<Customer> customer)
        {
            Customer v_existing_customer =  await repository.GetCustomer(customerId,null);
            if (v_existing_customer == null) return NotFound();

            customer.ApplyTo(v_existing_customer);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var v_result = await repository.UpdateCustomer(v_existing_customer);

            return Ok(v_result);
        }

        [HttpGet]
        [Authorize(Roles = "Customers, UMTS")]
        [Route("GetCustomer/")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Customer))]
        public async Task<IActionResult> GetCustomer([FromQuery]  int? customerId, [FromQuery] string personalNo)
        {
            var v_customer = await repository.GetCustomer(customerId, personalNo);
            if (v_customer == null) return NotFound();
            return Ok(v_customer);
        }



    }
}
