using Microsoft.AspNetCore.Mvc;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace TourPlanner.backend.Controllers
{
    using TourPlanner.backend.Enums;
    using TourPlanner.backend.DTOs;
    
    [ApiController]
    [Route("api/tours")]
    public class TourController : ControllerBase
    {
        private readonly ITourService _tourService;
        private readonly IGeocodingService _geocodingService;

        public TourController(ITourService tourService, IGeocodingService geocodingService) 
        {
            _tourService = tourService;
            _geocodingService = geocodingService;
        }



        [HttpGet("{id:long}")] // accepts only numbers
        public async Task<IActionResult> GetTourById([FromRoute] long id)
        {
            return Ok(await _tourService.FindByIdAsync(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateTour([FromBody] TourDto tourDTO)
        {
            var created = await _tourService.CreateAsync(tourDTO);
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

        [HttpGet("preview")]
        public async Task<IActionResult> GetRoutePreview([FromQuery] string from, [FromQuery] string to, [FromQuery] string transportType)
        {
            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
            {
                return BadRequest("Locațiile 'From' și 'To' sunt obligatorii.");
            }

            try
            {
                if (!Enum.TryParse<TransportType>(transportType, true, out var tType))
                {
                    tType = TransportType.DRIVE; 
                }

                var startData = await _geocodingService.GetCoordinatesAsync(from);
                var endData = await _geocodingService.GetCoordinatesAsync(to);

                var route = await _geocodingService.GetRouteAsync(
                    (startData.lon, startData.lat), 
                    (endData.lon, endData.lat), 
                    tType
                );

                return Ok(new RoutePreviewResponseDto
                {
                    DistanceKm = route.distance,
                    DurationMin = route.duration,
                    ResolvedFrom = startData.officialName, 
                    ResolvedTo = endData.officialName,
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}