using Microsoft.AspNetCore.Http;

namespace TourPlanner.backend.Services
{
    public interface IImageStorageService
    {
        Task<string> SaveImageAsync(long userId, long tourId, IFormFile file);
        void DeleteImage(string filePath);
    }
}