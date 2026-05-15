using BookingCore.Entities;
using BookingCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BCrypt.Net;

namespace BookingData.Services
{
    public class AuthService : IAuthService
    {
        private readonly BookingDbContext _context;
        private readonly IAuditService _audit;

        public AuthService(BookingDbContext context)
        {
            _context = context;
            _audit = new AuditService(context);
        }

        public async Task<User?> LoginAsync(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }
            await _audit.LogActionAsync(user.Id, "Login", "Successful login");
            return user;
        }

        public async Task<bool> RegisterAsync(User user, string password)
        {
            if (await UserExistsAsync(user.Username)) return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            await _audit.LogActionAsync(user.Id, "Registration", $"User {user.Username} registered.");
            return true;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task SeedAdminAsync()
        {
            // Ensure database is created and migrated
            await _context.Database.MigrateAsync();

            if (!await UserExistsAsync("admin"))
            {
                var admin = new User
                {
                    Username = "admin",
                    Email = "admin@example.com",
                    Role = "Admin"
                };
                await RegisterAsync(admin, "admin123");
            }

            if (!await _context.Services.AnyAsync())
            {
                _context.Services.AddRange(
                    new Service { Name = "General Consultation", Description = "Standard checkup", DurationMinutes = 30, Price = 50 },
                    new Service { Name = "Specialist Therapy", Description = "Advanced treatment", DurationMinutes = 60, Price = 150 },
                    new Service { Name = "Follow-up Session", Description = "Quick review", DurationMinutes = 15, Price = 30 }
                );
                await _context.SaveChangesAsync();
            }
        }
    }
}
