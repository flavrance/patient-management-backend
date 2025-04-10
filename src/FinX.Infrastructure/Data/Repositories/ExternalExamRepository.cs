using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinX.Infrastructure.Data.Repositories;

public class ExternalExamRepository : IExternalExamRepository
{
    private readonly ApplicationDbContext _context;

    public ExternalExamRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<ExternalExam>> GetByPatientIdAsync(Guid patientId)
    {
        return await _context.ExternalExams
            .Where(e => e.PatientId == patientId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<ExternalExam> GetByIdAsync(Guid id)
    {
        return await _context.ExternalExams.FindAsync(id);
    }

    public async Task<ExternalExam> CreateAsync(ExternalExam externalExam)
    {
        await _context.ExternalExams.AddAsync(externalExam);
        await _context.SaveChangesAsync();
        return externalExam;
    }

    public async Task<ExternalExam> UpdateAsync(ExternalExam externalExam)
    {
        _context.Entry(externalExam).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return externalExam;
    }

    public async Task DeleteAsync(ExternalExam externalExam)
    {
        _context.ExternalExams.Remove(externalExam);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.ExternalExams.AnyAsync(e => e.Id == id);
    }
} 