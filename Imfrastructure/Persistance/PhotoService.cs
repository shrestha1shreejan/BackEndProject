using Application.DatingApp.Interface;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Domain.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistance
{
    internal class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;
        #region Constructor
        public PhotoService(IOptions<CloudinarySettings> config)
        {
            Account account = new Account(
                    config.Value.CloudName,
                    config.Value.ApiKey,
                    config.Value.ApiSecret
                );
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
        }
        #endregion


        #region Implementation

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);

            var result = await _cloudinary.DestroyAsync(deleteParams);

            return result;
        }

        #endregion

    }
}
