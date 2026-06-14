using Microsoft.AspNetCore.Mvc;
using TourPlanner.backend.DTOs;
using TourPlanner.backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace TourPlanner.backend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tours")]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService) => _logService = logService;

        [HttpGet("{tourId}/logs")]
        public async Task<IActionResult> GetLogsByTour([FromRoute] long tourId)
        {
            return Ok(await _logService.FindByTourIdAsync(tourId));
        }

        [HttpPost("{tourId}/logs")]
        public async Task<IActionResult> CreateLog([FromRoute] long tourId, [FromBody] LogDto dto)
        {
            return Ok(await _logService.CreateAsync(tourId, dto));
        }

        [HttpPut("{tourId}/logs/{logId}")]
        public async Task<IActionResult> UpdateLog([FromRoute] long tourId, [FromRoute] long logId, [FromBody] LogDto dto)
        {
            return Ok(await _logService.UpdateAsync(tourId, logId, dto));
        }

        [HttpDelete("{tourId}/logs/{logId}")]
        public async Task<IActionResult> DeleteLog([FromRoute] long tourId, [FromRoute] long logId)
        {
            await _logService.DeleteAsync(logId);
            return NoContent(); // 204 No Content
        }
    }
}