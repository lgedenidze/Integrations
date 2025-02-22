using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;
using System;

namespace Integrations.Services
{

    public class PhotoRepository : IPhotoRepository
    {
        private readonly BlobContainerClient _blobContainerClient;

        public PhotoRepository(IConfiguration configuration)
        {
            string connectionString = configuration["AzureBlobStorage:ConnectionString"];
            string containerName = configuration["AzureBlobStorage:ContainerName"];
            _blobContainerClient = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<string> UploadPhotoAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty!");

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);

                using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

                return blobClient.Uri.ToString(); // Return the public image URL
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading image: {ex.Message}");
            }
        }

        public async Task<bool> DeletePhotoAsync(string fileName)
        {
            try
            {
                BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
                return await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting image: {ex.Message}");
            }
        }
    }
}
