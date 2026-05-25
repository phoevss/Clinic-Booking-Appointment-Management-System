using Microsoft.EntityFrameworkCore;
using BookingCore.Entities;

namespace BookingData
{
    public class BookingDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var assemblyPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var solutionDir = System.IO.Path.GetFullPath(System.IO.Path.Combine(assemblyPath, "..\\..\\..\\..\\..\\"));
            var dbPath = System.IO.Path.Combine(solutionDir, "booking.db");
            System.IO.File.AppendAllText(
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                $"{DateTime.Now:HH:mm:ss.fff}: assemblyPath={assemblyPath}, solutionDir={solutionDir}, dbPath={dbPath}\n");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed initial data if needed, or define constraints
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        }
    }
}
