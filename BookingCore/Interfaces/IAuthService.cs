using BookingCore.Entities;
using System.Threading.Tasks;

namespace BookingCore.Interfaces
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(User user, string password);
        Task<bool> UserExistsAsync(string username);
        Task SeedAdminAsync();
    }
}
