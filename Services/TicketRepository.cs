
using System;
using System.Linq;
 using System.Threading.Tasks;
using global::Integrations.Data;
using global::Integrations.Model;
using Microsoft.EntityFrameworkCore;
 using System.Collections.Generic;

namespace Integrations.Services
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;
        private readonly IQRCodeRepository _qrCodeGenerator;
        private readonly IPhotoRepository _photoRepository;
        private readonly IEmailService _emailService;

        public TicketRepository(AppDbContext context, IQRCodeRepository qrCodeGenerator, IPhotoRepository photoRepository, IEmailService emailService)
        {
            _context = context;
            _qrCodeGenerator = qrCodeGenerator;
            _photoRepository = photoRepository;
            _emailService = emailService;

        }

        // ✅ Get all tickets for a user
        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(int userId)
        {
            return await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.Basket)
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        // ✅ Purchase a ticket for an event
        public async Task<Ticket> PurchaseTicketAsync(int eventId, int userId)
        {
            var eventEntity = await _context.Events
                .Include(e => e.Baskets)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (eventEntity == null)
                throw new Exception("Event not found");

            var availableBasket = eventEntity.Baskets
                .OrderBy(b => b.Price)
                .FirstOrDefault(b => b.SoldTickets < b.TotalTickets);

            if (availableBasket == null)
                throw new Exception("No tickets available");

            var ticket = new Ticket
            {
                EventId = eventId,
                BasketId = availableBasket.Id,
                UserId = userId,
                IsPaid = false
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return ticket;
        }

        // ✅ Confirm ticket payment & generate QR code
        public async Task<bool> ConfirmTicketPaymentAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Event)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || ticket.IsPaid)
                return false;

            ticket.IsPaid = true;

            // Generate QR code
            string qrText = $"Ticket ID: {ticket.Id}, Event: {ticket.Event.Name}, User ID: {ticket.UserId}";
            byte[] qrBytes = await _qrCodeGenerator.GenerateQRCodeAsync(qrText);

            // Upload QR code to Azure
            string qrUrl = await _photoRepository.UploadQRCodeAsync(qrBytes, $"qr_{ticket.Id}");

            // Save QR code URL in DB
            ticket.QRCodeUrl = qrUrl;
            await _context.SaveChangesAsync();
            await _emailService.SendTicketEmailAsync(ticket.User.Email, ticket.Event.Name, ticket.Id.ToString(), qrUrl);

            return true;
        }


    }
}
