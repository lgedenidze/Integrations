using Integrations.Model;
using Integrations.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Integrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;

        public EventsController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        // ✅ GET /api/events → Get all events
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<IActionResult> GetActiveEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("AllEvents")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]

        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }


        // ✅ GET /api/events/{id} → Get event by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _eventRepository.GetEventByIdAsync(id);
            if (eventItem == null)
            {
                return NotFound(new { message = "Event not found" });
            }
            return Ok(eventItem);
        }

        // ✅ POST /api/events → Create a new event
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
        {
            if (newEvent == null || newEvent.LineUps == null)
            {
                return BadRequest(new { message = "Invalid event data" });
            }

            await _eventRepository.AddEventAsync(newEvent);
            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
        }

        // ✅ PUT /api/events/{id} → Update an event
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] Event updatedEvent)
        {
            if (id != updatedEvent.Id)
            {
                return BadRequest(new { message = "Event ID mismatch" });
            }

            var existingEvent = await _eventRepository.GetEventByIdAsync(id);
            if (existingEvent == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            await _eventRepository.UpdateEventAsync(updatedEvent);
            return Ok(updatedEvent);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _eventRepository.DeleteEventAsync(id);
            return Ok("Deleted!");
        }

    }
}