using BookingCore.Entities;
using BookingCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookingData.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly BookingDbContext _context;

        public ScheduleService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Schedule>> GetAvailableSchedulesAsync(int serviceId, DateTime date)
        {
            try
            {
                var dateStart = date.Date;
                var dateEnd = dateStart.AddDays(1);
                return await _context.Schedules
                    .Where(s => s.ServiceId == serviceId && s.Date >= dateStart && s.Date < dateEnd && s.IsAvailable)
                    .Include(s => s.Service)
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Schedule>();
            }
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            try
            {
                _context.Schedules.Add(schedule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            var existing = await _context.Schedules.FindAsync(schedule.Id);
            if (existing == null) return false;

            existing.ServiceId = schedule.ServiceId;
            existing.Date = schedule.Date;
            existing.StartTime = schedule.StartTime;
            existing.EndTime = schedule.EndTime;
            existing.IsAvailable = schedule.IsAvailable;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;
            _context.Schedules.Remove(schedule);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            try
            {
                return await _context.Schedules.Include(s => s.Service).ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Schedule>();
            }
        }
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly BookingDbContext _context;

        public AppointmentService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(int userId)
        {
            try
            {
                return await _context.Appointments
                    .AsNoTracking()
                    .Where(a => a.UserId == userId)
                    .Include(a => a.Service)
                    .Include(a => a.Schedule)
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Appointment>();
            }
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _context.Appointments
                    .Include(a => a.User)
                    .Include(a => a.Service)
                    .Include(a => a.Schedule)
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Appointment>();
            }
        }

        public async Task<bool> BookAppointmentAsync(Appointment appointment)
        {
            try
            {
                var rows = await _context.Schedules
                    .Where(s => s.Id == appointment.ScheduleId && s.IsAvailable)
                    .ExecuteUpdateAsync(setters => setters.SetProperty(s => s.IsAvailable, false));
                if (rows == 0) return false;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            var allowed = new[] { "Approved", "Rejected" };
            if (!allowed.Contains(status)) return false;

            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null) return false;

            appointment.Status = status;
            if (status == "Rejected" && appointment.Schedule != null)
                appointment.Schedule.IsAvailable = true;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Schedule)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null) return false;

            if (appointment.Status == "Cancelled" || appointment.Status == "Rejected" || appointment.Status == "Approved")
                return false;

            appointment.Status = "Cancelled";
            if (appointment.Schedule != null)
                appointment.Schedule.IsAvailable = true;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public class UserService : IUserService
    {
        private readonly BookingDbContext _context;

        public UserService(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                var users = await _context.Users.AsNoTracking().ToListAsync();
                foreach (var u in users)
                    u.PasswordHash = string.Empty;
                return users;
            }
            catch
            {
                return Enumerable.Empty<User>();
            }
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string role)
        {
            var allowed = new[] { "User", "Admin" };
            if (!allowed.Contains(role)) return false;

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.Role = role;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                var appointments = await _context.Appointments
                    .Where(a => a.UserId == userId)
                    .ToListAsync();
                _context.Appointments.RemoveRange(appointments);
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
