using System;

namespace BookingCore.Entities
{
    public class Appointment
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ServiceId { get; set; }
        public int ScheduleId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Notes { get; set; } = string.Empty;

        public User? User { get; set; }
        public Service? Service { get; set; }
        public Schedule? Schedule { get; set; }
    }
}
