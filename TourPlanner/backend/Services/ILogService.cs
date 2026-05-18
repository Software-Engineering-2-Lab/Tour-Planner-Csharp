using TourPlanner.backend.DTOs;

namespace TourPlanner.backend.Services
{
    public interface ILogService
    {
        Task<IEnumerable<LogDto>> FindByTourIdAsync(long tourId);
        Task<LogDto> CreateAsync(long tourId, LogDto dto);
        Task<LogDto> UpdateAsync(long tourId, long logId, LogDto dto);
        Task DeleteAsync(long logId);
    }
}