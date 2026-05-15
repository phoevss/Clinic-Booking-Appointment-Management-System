using BookingCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

using BookingCore.Interfaces;

namespace BookingData.Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly BookingDbContext _context;

        public ServiceManager(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            return await _context.Services.ToListAsync();
        }

        public async Task<bool> AddServiceAsync(Service service)
        {
            _context.Services.Add(service);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteServiceAsync(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return false;
            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
