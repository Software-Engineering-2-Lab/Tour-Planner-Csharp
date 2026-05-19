using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TourPlanner.backend.Data;
using TourPlanner.backend.Entities;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Services;

namespace TourPlanner.backend.Controllers
{
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageStorageService _storageService;

        public PhotosController(ApplicationDbContext context, IImageStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        [Authorize]
        [HttpPost("api/tours/{tourId}/photos")]
        public async Task<IActionResult> UploadPhoto(long tourId, IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == tourId);
            if (tour == null)
            {
                return NotFound("Tour not found.");
            }

            if (tour.UserId != userId)
            {
                return Forbid();
            }

            try
            {
                var absolutePath = await _storageService.SaveImageAsync(userId, tourId, file);
                
                var tourImage = new TourImage
                {
                    FilePath = absolutePath,
                    FileName = file.FileName,
                    TourId = tourId
                };

                _context.TourImages.Add(tourImage);
                await _context.SaveChangesAsync();

                var dto = new TourImageDto
                {
                    Id = tourImage.Id,
                    FileName = tourImage.FileName,
                    Url = $"/api/tours/{tourId}/photos/{tourImage.Id}",
                    TourId = tourId,
                    CreatedAt = tourImage.CreatedAt
                };

                return Ok(dto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the photo.");
            }
        }

        [AllowAnonymous]
        [HttpGet("api/tours/{tourId}/photos/{photoId}")]
        public async Task<IActionResult> GetPhoto(long tourId, long photoId)
        {
            var photo = await _context.TourImages
                .FirstOrDefaultAsync(ti => ti.Id == photoId && ti.TourId == tourId);

            if (photo == null)
            {
                return NotFound("Photo not found.");
            }

            if (!System.IO.File.Exists(photo.FilePath))
            {
                return NotFound("Physical file missing from server storage.");
            }

            var contentType = "image/jpeg";
            if (photo.FilePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
            {
                contentType = "image/png";
            }

            return PhysicalFile(photo.FilePath, contentType);
        }

        [Authorize]
        [HttpDelete("api/tours/{tourId}/photos/{photoId}")]
        public async Task<IActionResult> DeletePhoto(long tourId, long photoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !long.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var photo = await _context.TourImages
                .Include(ti => ti.Tour)
                .FirstOrDefaultAsync(ti => ti.Id == photoId && ti.TourId == tourId);

            if (photo == null)
            {
                return NotFound("Photo not found.");
            }

            if (photo.Tour!.UserId != userId)
            {
                return Forbid();
            }

            try
            {
                if (System.IO.File.Exists(photo.FilePath))
                {
                    System.IO.File.Delete(photo.FilePath);
                }

                _context.TourImages.Remove(photo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the photo file from storage.");
            }
        }
    }
}