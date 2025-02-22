namespace Integrations.Controllers
{
    using Integrations.Services;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    [Route("api/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoRepository _photoRepository;

        public PhotoController(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            try
            {
                string fileUrl = await _photoRepository.UploadPhotoAsync(file);
                return Ok(new { FileUrl = fileUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeletePhoto(string fileName)
        {
            try
            {
                bool isDeleted = await _photoRepository.DeletePhotoAsync(fileName);
                if (!isDeleted) return NotFound("File not found");

                return Ok(new { Message = "File deleted successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }

}
