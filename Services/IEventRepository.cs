using Integrations.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task AddEventAsync(Event newEvent);
        Task UpdateEventAsync(Event updatedEvent);
        Task DeleteEventAsync(int id);
        Task<IEnumerable<Event>> GetAllActiveEventsAsync();

    }
}
