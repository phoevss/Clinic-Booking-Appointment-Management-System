using BookingCore.Entities;
using System;
using System.Threading.Tasks;

namespace BookingData.Services
{
    public interface IAuditService
    {
        Task LogActionAsync(int? userId, string action, string details);
    }

    public class AuditService : IAuditService
    {
        private readonly BookingDbContext _context;

        public AuditService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task LogActionAsync(int? userId, string action, string details)
        {
            var log = new AuditLog
            {
                UserId = userId,
                Action = action,
                Details = details,
                Timestamp = DateTime.Now
            };
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
