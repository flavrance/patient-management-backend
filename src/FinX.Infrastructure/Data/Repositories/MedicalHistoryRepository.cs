using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinX.Infrastructure.Data.Repositories;

public class MedicalHistoryRepository : IMedicalHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public MedicalHistoryRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MedicalHistory>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.MedicalHistories
            .Where(m => m.PatientId == patientId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
    }

    public async Task<MedicalHistory> GetByIdAsync(Guid id)
    {
        return await _context.MedicalHistories.FindAsync(id);
    }

    public async Task<MedicalHistory> CreateAsync(MedicalHistory medicalHistory)
    {
        await _context.MedicalHistories.AddAsync(medicalHistory);
        await _context.SaveChangesAsync();
        return medicalHistory;
    }

    public async Task<MedicalHistory> UpdateAsync(MedicalHistory medicalHistory)
    {
        _context.Entry(medicalHistory).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return medicalHistory;
    }

    public async Task DeleteAsync(MedicalHistory medicalHistory)
    {
        _context.MedicalHistories.Remove(medicalHistory);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.MedicalHistories.AnyAsync(m => m.Id == id);
    }
} 