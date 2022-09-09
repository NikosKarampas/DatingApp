using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary; 
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var acc = new Account();

            if ( env == "Development")
            {
                acc.Cloud = config.Value.CloudName;
                acc.ApiKey = config.Value.ApiKey;
                acc.ApiSecret = config.Value.ApiSecret;
            }
            else
            {
                acc.Cloud = Environment.GetEnvironmentVariable("Cloudinary:CloudName");
                acc.ApiKey= Environment.GetEnvironmentVariable("Cloudinary:ApiKey");
                acc.ApiSecret = Environment.GetEnvironmentVariable("Cloudinary:ApiSecret");
            }

            _cloudinary = new Cloudinary(acc);
        }
        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsyc(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }
    }
}