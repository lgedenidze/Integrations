using Integrations.Data;
using Integrations.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public class EventRepository : IEventRepository
    {
        private readonly AppDbContext _context;

        public EventRepository(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all events
        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await _context.Events.Include(e => e.LineUps).ToListAsync();
        }

        // ✅ Get a single event by ID
        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events.Include(e => e.LineUps)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ✅ Create an event with auto-generated LineUp EventId
        public async Task AddEventAsync(Event newEvent)
        {
            newEvent.StartDate = DateTime.SpecifyKind(newEvent.StartDate, DateTimeKind.Utc);
            newEvent.EndDate = DateTime.SpecifyKind(newEvent.EndDate, DateTimeKind.Utc);
            // Ensure LineUps are linked to the event
            foreach (var lineup in newEvent.LineUps)
            {
                lineup.Event = newEvent;
            }

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
        }

        // ✅ Update an event with new LineUps
        public async Task UpdateEventAsync(Event updatedEvent)
        {
            var existingEvent = await _context.Events.Include(e => e.LineUps)
                .FirstOrDefaultAsync(e => e.Id == updatedEvent.Id);

            if (existingEvent != null)
            {
                // Update event details
                existingEvent.Name = updatedEvent.Name;
                existingEvent.Description = updatedEvent.Description;
                existingEvent.EventPhotoUrl = updatedEvent.EventPhotoUrl;
                existingEvent.StartDate = updatedEvent.StartDate;
                existingEvent.EndDate = updatedEvent.EndDate;

                // Remove old LineUps and replace with new ones
                existingEvent.LineUps.Clear();
                foreach (var lineup in updatedEvent.LineUps)
                {
                    lineup.Event = existingEvent; // Associate each LineUp with the event
                    existingEvent.LineUps.Add(lineup);
                }

                await _context.SaveChangesAsync();
            }
        }
    }
}
