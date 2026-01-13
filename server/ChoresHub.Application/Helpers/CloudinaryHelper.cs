using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ChoresHub.Application.Helpers
{
    public class CloudinaryHelper
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryHelper(IConfiguration configuration)
        {
            _cloudinary = new Cloudinary(
                new Account(
                    configuration["CloudinarySettings:CloudName"],
                    configuration["CloudinarySettings:ApiKey"],
                    configuration["CloudinarySettings:ApiSecret"]
                )
            );
        }

        public async Task<string> UploadProfilePictureAsync(IFormFile file)
        {
            if (file == null) return null!;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, file.OpenReadStream()),
                Folder = "chores_hub"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            return uploadResult.SecureUrl?.ToString()!;
        }
    }
}