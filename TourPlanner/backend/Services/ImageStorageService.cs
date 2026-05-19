using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace TourPlanner.backend.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private readonly string _baseStoragePath;

        public ImageStorageService()
        {
            _baseStoragePath = Path.Combine(Directory.GetCurrentDirectory(), "Storage");
        }

        public async Task<string> SaveImageAsync(long userId, long tourId, IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file.");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (Array.IndexOf(allowedExtensions, extension) < 0)
            {
                throw new ArgumentException("Invalid file extension. Only JPG, JPEG, and PNG are allowed.");
            }

            var targetFolder = Path.Combine(_baseStoragePath, "users", userId.ToString(), "tours", tourId.ToString(), "images");
            
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{extension}";
            var fullPath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fullPath;
        }

        public void DeleteImage(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}