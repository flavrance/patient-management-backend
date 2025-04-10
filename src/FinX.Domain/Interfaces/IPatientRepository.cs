using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Domain.Entities;

namespace FinX.Domain.Interfaces;

public interface IPatientRepository
{
    Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize, string searchTerm = null);
    Task<Patient> GetByIdAsync(Guid id);
    Task<Patient> CreateAsync(Patient patient);
    Task<Patient> UpdateAsync(Patient patient);
    Task DeleteAsync(Patient patient);
    Task<bool> ExistsAsync(Guid id);
    Task<int> GetTotalCountAsync(string searchTerm = null);
} 