using Microsoft.EntityFrameworkCore;
using TourPlanner.backend.Entities;

namespace TourPlanner.backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Tour> Tours { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configurare relație One-to-Many (Un Tour are mai multe Logs)
            modelBuilder.Entity<Log>()
                .HasOne(l => l.Tour)
                .WithMany(t => t.Logs)
                .HasForeignKey(l => l.TourId);
        }
    }
}