using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TourPlanner.backend.Data; // Unde ai definit DbContext-ul
using TourPlanner.backend.Entities;

namespace TourPlanner.backend.Repositories
{
    public class TourRepository : ITourRepository
    {
        private readonly ApplicationDbContext _context;

        public TourRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Tour>> FindByNameContainingIgnoreCaseAsync(string name)
        {
            return await _context.Tours
                .Where(t => t.Name.ToLower().Contains(name.ToLower()))
                .ToListAsync();
        }

        public async Task<List<Tour>> FindByUserAsync(User user)
        {
            return await _context.Tours
                .Where(t => t.User == user)
                .ToListAsync();
        }

        public async Task<List<Tour>> FindByUserIdAsync(long userId)
        {
            return await _context.Tours
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task<List<Tour>> FullTextSearchAsync(string q)
        {
            string searchPattern = q.ToLower();

            return await _context.Tours
                .Include(t => t.Logs) // LEFT JOIN în EF Core
                .Where(t => t.Name.ToLower().Contains(searchPattern) ||
                            (t.Description != null && t.Description.ToLower().Contains(searchPattern)) ||
                            t.Logs.Any(l => l.Comment.ToLower().Contains(searchPattern)))
                .Distinct()
                .ToListAsync();
        }

        public async Task<Tour?> GetByIdAsync(long id) 
        {
            return await _context.Tours
                .Include(t => t.Logs)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Tour> AddAsync(Tour tour)
        {
            _context.Tours.Add(tour);
            await _context.SaveChangesAsync();
            return tour;
        }

        public async Task<Tour> UpdateAsync(Tour tour)
        {
            _context.Tours.Update(tour); 
            await _context.SaveChangesAsync();
            return tour;
        }

 
        public async Task DeleteAsync(Tour tour)
        {
                _context.Tours.Remove(tour); 
                await _context.SaveChangesAsync();
            }
    }
}
    