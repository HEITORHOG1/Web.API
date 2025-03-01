using Microsoft.AspNetCore.Http;

namespace Web.API.Services
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file);

        Task DeleteImageAsync(string imagePath);
    }
}