using System;

namespace BookingCore.Entities
{
    public class Schedule
    {
        public int Id { get; set; }
        public int ServiceId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Service? Service { get; set; }
    }
}
