using BookingCore.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookingCore.Interfaces
{
    public interface IScheduleService
    {
        Task<IEnumerable<Schedule>> GetAvailableSchedulesAsync(int serviceId, DateTime date);
        Task<bool> AddScheduleAsync(Schedule schedule);
        Task<bool> UpdateScheduleAsync(Schedule schedule);
        Task<bool> DeleteScheduleAsync(int id);
        Task<IEnumerable<Schedule>> GetAllSchedulesAsync();
    }

    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetUserAppointmentsAsync(int userId);
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<bool> BookAppointmentAsync(Appointment appointment);
        Task<bool> UpdateAppointmentStatusAsync(int id, string status);
        Task<bool> CancelAppointmentAsync(int id);
    }

    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<bool> UpdateUserRoleAsync(int userId, string role);
        Task<bool> DeleteUserAsync(int userId);
    }

    public interface IServiceManager
    {
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<bool> AddServiceAsync(Service service);
        Task<bool> DeleteServiceAsync(int id);
    }
}
