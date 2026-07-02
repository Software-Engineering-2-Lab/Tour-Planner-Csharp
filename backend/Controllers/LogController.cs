using Microsoft.AspNetCore.Mvc;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace TourPlanner.backend.Controllers
{
   
    [ApiController]
    [Route("api/tours")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;
        private readonly ITourService _tourService;

        public LogController(ILogService logService, ITourService tourService) {
            _logService = logService ;
            _tourService=tourService;
        }

        [HttpGet("{tourId}/logs")]
        public async Task<IActionResult> GetLogsByTour([FromRoute] long tourId)
        {
            return Ok(await _logService.FindByTourIdAsync(tourId));
        }

        [HttpPost("{tourId}/logs")]
        public async Task<IActionResult> CreateLog([FromRoute] long tourId, [FromBody] LogDto dto)
        {
            var createdLog = await _logService.CreateAsync(tourId,dto);
            await _tourService.CalculatesPopularityAsync(tourId);
            await _tourService.CalculatesChildFriendlinessAsync(tourId);

            return Ok(createdLog);
        }

        [HttpPut("{tourId}/logs/{logId}")]
        public async Task<IActionResult> UpdateLog([FromRoute] long tourId, [FromRoute] long logId, [FromBody] LogDto dto)
        {
            var updatedLog=await _logService.UpdateAsync(tourId, logId, dto);
            await _tourService.CalculatesChildFriendlinessAsync(tourId);
            return Ok(updatedLog);

        }

        [HttpDelete("{tourId}/logs/{logId}")]
        public async Task<IActionResult> DeleteLog([FromRoute] long tourId, [FromRoute] long logId)
        {
            await _logService.DeleteAsync(logId);
            await _tourService.CalculatesPopularityAsync(tourId);
            await _tourService.CalculatesChildFriendlinessAsync(tourId);
            return NoContent(); // 204 No Content
        }
    }
}