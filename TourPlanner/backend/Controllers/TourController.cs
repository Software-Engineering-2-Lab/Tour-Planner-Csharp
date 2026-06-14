using Microsoft.AspNetCore.Mvc;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace TourPlanner.backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tours")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService) => _tourService = tourService;

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTourById([FromRoute] long id)
        {
            return Ok(await _tourService.FindByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTour([FromBody] TourDto tourDTO)
        {
            var created = await _tourService.CreateAsync(tourDTO);
            // CreatedAtAction returnează status 201 și locația resursei
            return CreatedAtAction(nameof(GetTourById), new { id = created.Id }, created);
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportTours([FromBody] List<TourDto> tourDtos)
        {
            var created = new List<object>();
            foreach (var dto in tourDtos)
            {
                var tour = await _tourService.CreateAsync(dto);
                created.Add(tour);
            }
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTour([FromRoute] long id, [FromBody] TourDto tourDTO)
        {
            return Ok(await _tourService.UpdateAsync(id, tourDTO));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTour([FromRoute] long id)
        {
            await _tourService.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchTours([FromQuery] string query)
        {
            return Ok(await _tourService.SearchAsync(query));
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetToursByUserId([FromRoute] long userId)
        {
            return Ok(await _tourService.FindByUserIdAsync(userId));
        }
    }
}