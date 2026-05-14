using System.Collections.Generic;
using System.Threading.Tasks;
using TourPlanner.backend.Entities;

namespace TourPlanner.backend.Repositories
{
    public interface IUserRepository
    {
        Task<User?> FindByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<User?> FindByEmailAsync(string email);
        Task<User?> GetByIdAsync(long id);       
        Task<List<User>> GetAllAsync();          
        Task<User> AddAsync(User user);          
        void Update(User user);                  
        void Delete(User user);                  
        Task SaveAsync();                        
    }
}