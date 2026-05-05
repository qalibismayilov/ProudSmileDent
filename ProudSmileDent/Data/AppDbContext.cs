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

        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Service> Services { get; set; }
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

            modelBuilder.Entity<ChatMessage>()
                .Property(x => x.Message)
                .IsRequired();

            modelBuilder.Entity<ChatMessage>()
                .Property(x => x.Sender)
                .IsRequired();
        }
    }
}