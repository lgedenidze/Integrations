using QRCoder;
using SkiaSharp;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Integrations.Data;
using Microsoft.EntityFrameworkCore;

namespace Integrations.Services
{
    public class QRCodeRepository : IQRCodeRepository
    {
        private readonly AppDbContext _context;

        public QRCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateQRCodeAsync(string content, int pixelsPerModule = 10)
        {
            try
            {
                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                {
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                    return await Task.FromResult(RenderQRCode(qrCodeData, pixelsPerModule));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"QR Code generation failed: {ex.Message}");
            }
        }

        private byte[] RenderQRCode(QRCodeData qrCodeData, int pixelsPerModule)
        {
            int matrixSize = qrCodeData.ModuleMatrix.Count;
            int imageSize = matrixSize * pixelsPerModule;

            using (var surface = SKSurface.Create(new SKImageInfo(imageSize, imageSize)))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
                {
                    for (int x = 0; x < matrixSize; x++)
                    {
                        for (int y = 0; y < matrixSize; y++)
                        {
                            if (qrCodeData.ModuleMatrix[x][y])
                            {
                                canvas.DrawRect(x * pixelsPerModule, y * pixelsPerModule, pixelsPerModule, pixelsPerModule, paint);
                            }
                        }
                    }
                }

                using (var image = surface.Snapshot())
                {
                    using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }
            }
        }

        // ✅ Validate QR Code by checking Ticket ID & Secret
        public async Task<bool> ValidateQRCodeAsync(int ticketId, string secret, bool isReject)
        {
            var ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null || !ticket.IsPaid ||ticket.IsUsedTicket)
                return false;

         
            if (ticket.Secret == secret)
            {
                ticket.IsUsedTicket = true;
                ticket.IsReject = isReject;
                await _context.SaveChangesAsync();
                return true;

            }

            return false;
        }


        private string GenerateSecureToken(int ticketId, int userId)
        {
            using (var sha256 = SHA256.Create())
            {
                string rawData = $"{ticketId}-{userId}-MySecretKey";
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                return Convert.ToBase64String(bytes);
            }
        }
    }
}
