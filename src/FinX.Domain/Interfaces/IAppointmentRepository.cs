using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Domain.Entities;

namespace FinX.Domain.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<IEnumerable<Appointment>> GetAllAsync(int page, int pageSize, Guid? patientId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<Appointment> GetByIdAsync(Guid id);
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<Appointment> UpdateAsync(Appointment appointment);
        Task DeleteAsync(Appointment appointment);
        Task<bool> ExistsAsync(Guid id);
    }
} 