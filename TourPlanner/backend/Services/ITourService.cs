using TourPlanner.backend.DTOs;

namespace TourPlanner.backend.Services
{
    public interface ITourService
    {
        Task<TourDto?> FindByIdAsync(long id);
        Task<List<TourDto>> FindByUserIdAsync(long userId);
        Task<TourDto> CreateAsync(TourDto tourDTO);
        Task<TourDto> UpdateAsync(long id, TourDto tourDTO);
        Task DeleteAsync(long id);
        Task<SearchResultDto> SearchAsync(string query);
    }
}