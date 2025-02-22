using Integrations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Integrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // ✅ Requires authentication for all ticket operations
    public class TicketsController : ControllerBase
    {
        private readonly ITicketRepository _ticketRepository;

        public TicketsController(ITicketRepository ticketRepository)
        {
            _ticketRepository = ticketRepository;
        }

        // ✅ Get all tickets for the authenticated user
        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetUserTickets()
        {
            try
            {
                int userId = GetUserIdFromToken();
                var tickets = await _ticketRepository.GetUserTicketsAsync(userId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Purchase a ticket for an event
        [HttpPost("purchase/{eventId}")]
        public async Task<IActionResult> PurchaseTicket(int eventId)
        {
            try
            {
                int userId = GetUserIdFromToken();
                var ticket = await _ticketRepository.PurchaseTicketAsync(eventId, userId);
                return Ok(new { ticketId = ticket.Id, message = "Ticket purchased successfully. Awaiting payment." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Confirm payment for a ticket & generate QR Code
        [HttpPost("confirm-payment/{ticketId}")]
        public async Task<IActionResult> ConfirmTicketPayment(int ticketId)
        {
            try
            {
                bool success = await _ticketRepository.ConfirmTicketPaymentAsync(ticketId);
                if (!success)
                    return NotFound(new { message = "Ticket not found or already paid." });

                return Ok(new { message = "Payment confirmed. Your ticket is now valid with a QR code." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Extract User ID from JWT token
        private int GetUserIdFromToken()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                throw new UnauthorizedAccessException("User ID not found in token.");

            return int.Parse(userIdClaim.Value);
        }
    }
}
