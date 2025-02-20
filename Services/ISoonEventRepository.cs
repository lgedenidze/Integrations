using Integrations.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface ISoonEventRepository
    {
        Task<List<SoonEvent>> GetSoonEventsAsync();
        Task<string> AddSoonEventAsync(int eventId, int position);
        Task<string> RemoveSoonEventAsync(int soonEventId);
    }
}

