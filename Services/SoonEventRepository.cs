using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Integrations.Data;
using Integrations.Model;
 using Integrations.Services;
using Integrations.Utils;
using Microsoft.EntityFrameworkCore;

namespace Integrations.Repositories
{
    public class SoonEventRepository : ISoonEventRepository
    {
        private readonly AppDbContext _context;

        public SoonEventRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<SoonEvent>> GetSoonEventsAsync()
        {
            return await _context.SoonEvents
           .Include(se => se.Event)
           .ThenInclude(e => e.LineUps) // ✅ Fetch associated LineUps
           .OrderBy(se => se.Position)
           .Take(4)
           .ToListAsync();
        }

        public async Task<string> AddSoonEventAsync(int eventId, int position)
        {
            if (position < 1 || position > 4)
                return AppResource.SoonEvent_InvalidPosition; // ✅ Invalid position

            // ✅ Check if event is already a soon event
            var existingEntry = await _context.SoonEvents.FirstOrDefaultAsync(se => se.EventId == eventId);
            if (existingEntry != null)
                return AppResource.SoonEvent_AlreadyExists;

            // ✅ Ensure we only allow 4 soon events
            var soonEvents = await GetSoonEventsAsync();
            if (soonEvents.Count >= 4)
            {
                return AppResource.SoonEvent_LimitReached;
            }

            // ✅ Create new soon event
            var soonEvent = new SoonEvent
            {
                EventId = eventId,
                Position = position
            };

            _context.SoonEvents.Add(soonEvent);
            await _context.SaveChangesAsync();
            return AppResource.SoonEvent_Added;
        }

        public async Task<string> RemoveSoonEventAsync(int soonEventId)
        {
            var soonEvent = await _context.SoonEvents.FindAsync(soonEventId);
            if (soonEvent == null)
                return AppResource.SoonEvent_NotFound;

            _context.SoonEvents.Remove(soonEvent);
            await _context.SaveChangesAsync();
            return AppResource.SoonEvent_Removed;
        }
    }
}
