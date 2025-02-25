using QRCoder;
using System.Threading.Tasks;

namespace Integrations.Services
{
    public interface IQRCodeRepository
    {
        Task<byte[]> GenerateQRCodeAsync(string text, int pixelsPerModule = 10);
        Task<bool> ValidateQRCodeAsync(int ticketId, string secret , bool isReject);

    }
}
