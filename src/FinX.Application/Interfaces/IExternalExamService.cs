using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.ExternalExams;

namespace FinX.Application.Interfaces;

public interface IExternalExamService
{
    Task<IEnumerable<ExternalExamDto>> GetByPatientIdAsync(Guid patientId);
    Task<ExternalExamDto> GetByIdAsync(Guid id);
    Task<ExternalExamDto> CreateAsync(CreateExternalExamDto createExternalExamDto);
    Task<ExternalExamDto> UpdateAsync(Guid id, UpdateExternalExamDto updateExternalExamDto);
    Task DeleteAsync(Guid id);
} 