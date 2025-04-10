using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinX.Infrastructure.Data.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AppointmentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Appointment>> GetAllAsync(int page, int pageSize, Guid? patientId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.Appointments.AsQueryable();

            // Apply optional filters
            if (patientId.HasValue)
            {
                query = query.Where(a => a.PatientId == patientId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(a => a.AppointmentDate <= toDate.Value);
            }

            // Order by appointment date
            query = query.OrderBy(a => a.AppointmentDate);

            // Apply pagination
            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(a => a.Patient)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _context.Appointments
                .Where(a => a.AppointmentDate >= start && a.AppointmentDate <= end)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<Appointment> GetByIdAsync(Guid id)
        {
            return await _context.Appointments.FindAsync(id);
        }

        public async Task<Appointment> CreateAsync(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task<Appointment> UpdateAsync(Appointment appointment)
        {
            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return appointment;
        }

        public async Task DeleteAsync(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Appointments.AnyAsync(a => a.Id == id);
        }
    }
} 