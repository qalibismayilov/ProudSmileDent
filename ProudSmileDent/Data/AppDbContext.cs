using Microsoft.EntityFrameworkCore;
using ProudSmileDent.Models;

namespace ProudSmileDent.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
    }
}