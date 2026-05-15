using System;

namespace BookingCore.Entities
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
        public int? UserId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;

        public User? User { get; set; }
    }
}
