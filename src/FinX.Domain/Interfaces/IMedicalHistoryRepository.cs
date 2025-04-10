using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Domain.Entities;

namespace FinX.Domain.Interfaces;

public interface IMedicalHistoryRepository
{
    Task<IEnumerable<MedicalHistory>> GetByPatientIdAsync(Guid patientId);
    Task<MedicalHistory> GetByIdAsync(Guid id);
    Task<MedicalHistory> CreateAsync(MedicalHistory medicalHistory);
    Task<MedicalHistory> UpdateAsync(MedicalHistory medicalHistory);
    Task DeleteAsync(MedicalHistory medicalHistory);
    Task<bool> ExistsAsync(Guid id);
} 