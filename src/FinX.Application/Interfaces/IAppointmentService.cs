using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.Appointment;

namespace FinX.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<IEnumerable<AppointmentDto>> GetAllAsync(int page, int pageSize, Guid? patientId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(Guid patientId);
        Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(DateTime start, DateTime end);
        Task<AppointmentDto> GetByIdAsync(Guid id);
        Task<AppointmentDto> CreateAsync(CreateAppointmentDto createAppointmentDto);
        Task<AppointmentDto> UpdateAsync(Guid id, UpdateAppointmentDto updateAppointmentDto);
        Task<bool> DeleteAsync(Guid id);
    }
} 