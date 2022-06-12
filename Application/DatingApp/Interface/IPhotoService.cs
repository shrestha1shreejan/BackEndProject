using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Application.DatingApp.Interface
{
    public  interface IPhotoService
    {
        /// <summary>
        /// IFormFile represents a file sent along with the http request
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}
