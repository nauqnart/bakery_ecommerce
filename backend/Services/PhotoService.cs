using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace StitchArtisan.Backend.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IConfiguration config)
        {
            var cloudName = config["CloudinarySettings:CloudName"];
            if (!string.IsNullOrEmpty(cloudName))
            {
                var acc = new Account(
                    cloudName,
                    config["CloudinarySettings:ApiKey"],
                    config["CloudinarySettings:ApiSecret"]
                );
                _cloudinary = new Cloudinary(acc);
            }
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
                    Folder = "hearth-and-harvest"
                };

                if (_cloudinary != null)
                {
                    uploadResult = await _cloudinary.UploadAsync(uploadParams);
                }
                else
                {
                    throw new Exception("Cloudinary settings are not configured. Cannot upload image.");
                }
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            if (_cloudinary == null) return new DeletionResult { Error = new Error { Message = "Cloudinary not configured" } };
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
