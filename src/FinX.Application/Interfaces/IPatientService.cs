using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.Patient;

namespace FinX.Application.Interfaces;

public interface IPatientService
{
    Task<(IEnumerable<PatientDto> Patients, int TotalCount)> GetAllAsync(int page, int pageSize, string searchTerm = null);
    Task<PatientDto> GetByIdAsync(Guid id);
    Task<PatientDto> CreateAsync(CreatePatientDto createPatientDto);
    Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto updatePatientDto);
    Task DeleteAsync(Guid id);
} 