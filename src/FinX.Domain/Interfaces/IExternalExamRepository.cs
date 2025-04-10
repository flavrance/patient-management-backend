using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Domain.Entities;

namespace FinX.Domain.Interfaces;

public interface IExternalExamRepository
{
    Task<IEnumerable<ExternalExam>> GetByPatientIdAsync(Guid patientId);
    Task<ExternalExam> GetByIdAsync(Guid id);
    Task<ExternalExam> CreateAsync(ExternalExam externalExam);
    Task<ExternalExam> UpdateAsync(ExternalExam externalExam);
    Task DeleteAsync(ExternalExam externalExam);
    Task<bool> ExistsAsync(Guid id);
} 