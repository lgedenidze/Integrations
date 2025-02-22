using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IPhotoRepository
    {
        Task<string> UploadPhotoAsync(IFormFile file);
        Task<bool> DeletePhotoAsync(string fileName);
        public Task<string> UploadQRCodeAsync(byte[] qrBytes, string fileName);
    }
}
 