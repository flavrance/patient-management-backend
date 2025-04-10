using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FinX.Infrastructure.Data.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly ApplicationDbContext _context;

    public PatientRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<Patient>> GetAllAsync(int page, int pageSize, string searchTerm = null)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm) ||
                p.Email.ToLower().Contains(searchTerm) ||
                p.PhoneNumber.Contains(searchTerm));
        }

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Patient> GetByIdAsync(Guid id)
    {
        return await _context.Patients.FindAsync(id);
    }

    public async Task<Patient> CreateAsync(Patient patient)
    {
        await _context.Patients.AddAsync(patient);
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task<Patient> UpdateAsync(Patient patient)
    {
        _context.Entry(patient).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return patient;
    }

    public async Task DeleteAsync(Patient patient)
    {
        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Patients.AnyAsync(p => p.Id == id);
    }

    public async Task<int> GetTotalCountAsync(string searchTerm = null)
    {
        var query = _context.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(p =>
                p.FirstName.ToLower().Contains(searchTerm) ||
                p.LastName.ToLower().Contains(searchTerm) ||
                p.Email.ToLower().Contains(searchTerm) ||
                p.PhoneNumber.Contains(searchTerm));
        }

        return await query.CountAsync();
    }
} 