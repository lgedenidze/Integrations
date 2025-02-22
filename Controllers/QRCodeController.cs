using Integrations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Integrations.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeController : ControllerBase
    {
        private readonly IQRCodeRepository _qrCodeRepository;

        public QRCodeController(IQRCodeRepository qrCodeRepository)
        {
            _qrCodeRepository = qrCodeRepository;
        }

        // ✅ Generate QR Code (For Testing)
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQRCode([FromBody] QRGenerateRequest request)
        {
            try
            {
                string qrContent = JsonSerializer.Serialize(new
                {
                    ticketId = request.TicketId,
                    userId = request.UserId,
                    eventName = request.EventName,
                    secret = request.Secret
                });

                byte[] qrBytes = await _qrCodeRepository.GenerateQRCodeAsync(qrContent);
                return File(qrBytes, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ✅ Scan QR Code (Validation)
        [Authorize(Roles = "EventStaff, Admin")]
        [HttpPost("scan")]
        public async Task<IActionResult> ScanQRCode([FromBody] QRScanRequest request)
        {
            try
            {
                bool isValid = await _qrCodeRepository.ValidateQRCodeAsync(request.TicketId, request.Secret);

                if (!isValid)
                    return BadRequest(new { message = "Invalid or tampered QR code!" });

                return Ok(new { message = "QR Code is valid!", ticketId = request.TicketId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class QRGenerateRequest
    {
        public int TicketId { get; set; }
        public int UserId { get; set; }
        public string EventName { get; set; }
        public string Secret { get; set; }
    }

    public class QRScanRequest
    {
        public int TicketId { get; set; }
        public string Secret { get; set; }
    }
}
