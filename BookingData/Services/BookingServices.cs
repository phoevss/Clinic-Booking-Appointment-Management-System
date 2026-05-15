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
            return await _context.Schedules
                .Where(s => s.ServiceId == serviceId && s.Date.Date == date.Date && s.IsAvailable)
                .Include(s => s.Service)
                .ToListAsync();
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Update(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null) return false;
            _context.Schedules.Remove(schedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Schedule>> GetAllSchedulesAsync()
        {
            return await _context.Schedules.Include(s => s.Service).ToListAsync();
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
            return await _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Service)
                .Include(a => a.Schedule)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.User)
                .Include(a => a.Service)
                .Include(a => a.Schedule)
                .ToListAsync();
        }

        public async Task<bool> BookAppointmentAsync(Appointment appointment)
        {
            // Check if schedule is still available
            var schedule = await _context.Schedules.FindAsync(appointment.ScheduleId);
            if (schedule == null || !schedule.IsAvailable) return false;

            schedule.IsAvailable = false;
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null) return false;

            appointment.Status = status;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAppointmentAsync(int id)
        {
            var appointment = await _context.Appointments.Include(a => a.Schedule).FirstOrDefaultAsync(a => a.Id == id);
            if (appointment == null) return false;

            appointment.Status = "Cancelled";
            if (appointment.Schedule != null)
            {
                appointment.Schedule.IsAvailable = true;
            }
            await _context.SaveChangesAsync();
            return true;
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
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, string role)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            user.Role = role;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
