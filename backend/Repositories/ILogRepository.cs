using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.backend.Entities;

namespace TourPlanner.backend.Repositories
{
    public interface ILogRepository
    {
        Task<List<Log>> FindByTourIdAsync(long tourId);
        Task<List<Log>> FindByTourAsync(Tour tour);
        Task<Log> AddAsync(Log log);
        Task<Log?> GetByIdAsync (long id);
        Task<Log> UpdateAsync (Log log);
        Task DeleteAsync (Log log);
        
        


    }
}