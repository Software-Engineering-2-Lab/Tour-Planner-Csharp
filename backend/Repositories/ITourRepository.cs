using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.backend.Entities;

namespace TourPlanner.backend.Repositories
{
    public interface ITourRepository
    {
        Task<List<Tour>> FindByNameContainingIgnoreCaseAsync(string name);
        Task<List<Tour>> FindByUserAsync(User user);
        Task<List<Tour>> FindByUserIdAsync(long userId);
        Task<List<Tour>> FullTextSearchAsync(string query);
        Task<Tour> AddAsync(Tour tour);
        Task<Tour?> GetByIdAsync (long id);
        Task<Tour> UpdateAsync (Tour tour);
        Task DeleteAsync (Tour tour);
    }
}