namespace Integrations.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Integrations.Repositories;
     using Integrations.Services;
    using Integrations.Utils;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    [Route("api/[controller]")]
    [ApiController]
    public class SoonEventsController : ControllerBase
    {
        private readonly ISoonEventRepository _soonEventRepository;

        public SoonEventsController(ISoonEventRepository soonEventRepository)
        {
            _soonEventRepository = soonEventRepository;
        }

        [HttpGet] 
        public async Task<IActionResult> GetSoonEvents()
        {
            var events = await _soonEventRepository.GetSoonEventsAsync();
            return Ok(events);
        }

        [HttpPost("add/{eventId}")]
     //   [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddSoonEvent(int eventId, [FromQuery] int position)
        {
            var message = await _soonEventRepository.AddSoonEventAsync(eventId, position);
            if (message == AppResource.SoonEvent_Added)
                return Ok(new { message });

            return BadRequest(new { message });
        }

        [HttpDelete("remove/{soonEventId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveSoonEvent(int soonEventId)
        {
            var message = await _soonEventRepository.RemoveSoonEventAsync(soonEventId);
            if (message == AppResource.SoonEvent_Removed)
                return Ok(new { message });

            return NotFound(new { message });
        }
    }

}
