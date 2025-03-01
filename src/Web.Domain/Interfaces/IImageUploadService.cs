using Microsoft.AspNetCore.Http;

namespace Web.Domain.Interfaces
{
    public interface IImageUploadService
    {
        Task<string> UploadImageAsync(IFormFile file);

        Task DeleteImageAsync(string imagePath);
    }
}