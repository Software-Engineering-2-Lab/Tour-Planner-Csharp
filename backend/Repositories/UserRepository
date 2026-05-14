using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.backend.Data;
using TourPlanner.backend.Entities;

namespace TourPlanner.backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Implementare căutări specifice ---

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> ExistsByUsernameAsync(string username)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == username);
        }

        public async Task<User?> FindByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // --- Implementare CRUD ---

        public async Task<User?> GetByIdAsync(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        public void Update(User user)
        {
            // EF Core marchează automat entitatea ca 'Modified'
            _context.Users.Update(user);
        }

        public void Delete(User user)
        {
            // Marchează entitatea ca 'Deleted'
            _context.Users.Remove(user);
        }

        public async Task SaveAsync()
        {
            // Trimite toate modificările (Add, Update, Delete) către DB
            await _context.SaveChangesAsync();
        }
    }
}