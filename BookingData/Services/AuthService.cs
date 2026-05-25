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
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                await _audit.LogActionAsync(null, "Login_Failed", $"Failed login attempt for username: {username}");
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
            var dbPath = _context.Database.GetConnectionString();
            System.IO.File.AppendAllText(
                System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                $"{DateTime.Now:HH:mm:ss.fff}: SeedAdminAsync started. Connection: {dbPath}\n");

            try
            {
                // Ensure database is created with current model
                var created = await _context.Database.EnsureCreatedAsync();
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                    $"{DateTime.Now:HH:mm:ss.fff}: EnsureCreatedAsync returned {created}\n");

                if (!await UserExistsAsync("admin"))
                {
                    System.IO.File.AppendAllText(
                        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                        $"{DateTime.Now:HH:mm:ss.fff}: Admin user not found, creating...\n");
                    var admin = new User
                    {
                        Username = "admin",
                        Email = "admin@example.com",
                        Role = "Admin"
                    };
                    await RegisterAsync(admin, "admin123");
                    System.IO.File.AppendAllText(
                        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                        $"{DateTime.Now:HH:mm:ss.fff}: Admin user created\n");
                }
                else
                {
                    System.IO.File.AppendAllText(
                        System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                        $"{DateTime.Now:HH:mm:ss.fff}: Admin user already exists\n");
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

                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                    $"{DateTime.Now:HH:mm:ss.fff}: SeedAdminAsync completed successfully\n");
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "booking_debug.log"),
                    $"{DateTime.Now:HH:mm:ss.fff}: SeedAdminAsync EXCEPTION: {ex.GetType().Name}: {ex.Message}\n{ex.StackTrace}\n");
                throw;
            }
        }
    }
}
