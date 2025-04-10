using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.MedicalHistory;

namespace FinX.Application.Interfaces;

public interface IMedicalHistoryService
{
    Task<IEnumerable<MedicalHistoryDto>> GetByPatientIdAsync(Guid patientId);
    Task<MedicalHistoryDto> GetByIdAsync(Guid id);
    Task<MedicalHistoryDto> CreateAsync(CreateMedicalHistoryDto createMedicalHistoryDto);
    Task<MedicalHistoryDto> UpdateAsync(Guid id, UpdateMedicalHistoryDto updateMedicalHistoryDto);
    Task DeleteAsync(Guid id);
} 