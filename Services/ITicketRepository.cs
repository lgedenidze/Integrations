using Integrations.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrations.Services
{
  
        public interface ITicketRepository
        {
            // ✅ Get all tickets for a user
            Task<IEnumerable<Ticket>> GetUserTicketsAsync(int userId);

            // ✅ Purchase a ticket for an event
            Task<Ticket> PurchaseTicketAsync(int eventId, int userId);

            // ✅ Confirm ticket payment & generate QR Code
            Task<bool> ConfirmTicketPaymentAsync(int ticketId);
        }
    

}
