using DentClinicApi.Models;
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

<<<<<<< HEAD
=======
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Service> Services { get; set; }
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(x => x.Username)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.PasswordHash)
                .IsRequired();
<<<<<<< HEAD
=======

            modelBuilder.Entity<ChatMessage>()
                .Property(x => x.Message)
                .IsRequired();

            modelBuilder.Entity<ChatMessage>()
                .Property(x => x.Sender)
                .IsRequired();
>>>>>>> 2386656b82fc8776b7038d584c6b493cf03b5b3d
        }
    }
}