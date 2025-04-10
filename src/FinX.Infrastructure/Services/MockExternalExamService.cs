using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinX.Application.DTOs.ExternalExams;
using FinX.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinX.Infrastructure.Services;

public class MockExternalExamService : IExternalExamService
{
    private readonly List<ExternalExamDto> _mockExams;
    private readonly ILogger<MockExternalExamService> _logger;

    public MockExternalExamService(ILogger<MockExternalExamService> logger)
    {
        _logger = logger;
        _mockExams = GenerateMockData();
    }

    public Task<IEnumerable<ExternalExamDto>> GetByPatientIdAsync(Guid patientId)
    {
        _logger.LogInformation("Buscando exames externos para o paciente: {PatientId}", patientId);
        var exams = _mockExams.Where(e => e.PatientId == patientId).ToList();
        return Task.FromResult<IEnumerable<ExternalExamDto>>(exams);
    }

    public Task<ExternalExamDto> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Buscando exame externo por ID: {Id}", id);
        var exam = _mockExams.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(exam);
    }

    public Task<ExternalExamDto> CreateAsync(CreateExternalExamDto createExternalExamDto)
    {
        _logger.LogInformation("Mock - Criando exame externo (somente leitura)");
        
        // Simulamos a criação, mas não alteramos os dados mock
        var newExam = new ExternalExamDto
        {
            Id = Guid.NewGuid(),
            PatientId = createExternalExamDto.PatientId,
            Name = createExternalExamDto.Name,
            Date = createExternalExamDto.Date,
            Laboratory = createExternalExamDto.Laboratory,
            Result = createExternalExamDto.Result,
            CreatedAt = DateTime.UtcNow
        };
        
        return Task.FromResult(newExam);
    }

    public Task<ExternalExamDto> UpdateAsync(Guid id, UpdateExternalExamDto updateExternalExamDto)
    {
        _logger.LogInformation("Mock - Atualizando exame externo (somente leitura): {Id}", id);
        
        // Simulamos a atualização, mas não alteramos os dados mock
        var existingExam = _mockExams.FirstOrDefault(e => e.Id == id);
        if (existingExam == null)
        {
            return Task.FromResult<ExternalExamDto>(null);
        }
        
        var updatedExam = new ExternalExamDto
        {
            Id = existingExam.Id,
            PatientId = existingExam.PatientId,
            Name = updateExternalExamDto.Name,
            Date = updateExternalExamDto.Date,
            Laboratory = updateExternalExamDto.Laboratory,
            Result = updateExternalExamDto.Result,
            CreatedAt = existingExam.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };
        
        return Task.FromResult(updatedExam);
    }

    public Task DeleteAsync(Guid id)
    {
        _logger.LogInformation("Mock - Deletando exame externo (somente leitura): {Id}", id);
        // Não fazemos nada, apenas simulamos a operação
        return Task.CompletedTask;
    }

    private List<ExternalExamDto> GenerateMockData()
    {
        var patientId1 = Guid.Parse("d78a5f43-8a09-4cca-a954-ff9579b87e6a");
        var patientId2 = Guid.Parse("3d3a52d9-0c3d-4cb0-96a3-87d5867f1a5b");
        
        return new List<ExternalExamDto>
        {
            new ExternalExamDto
            {
                Id = Guid.Parse("f5b77c9d-8a0b-4c6a-9f5a-7bca2e8987e4"),
                PatientId = patientId1,
                Name = "Hemograma Completo",
                Date = DateTime.UtcNow.AddDays(-30),
                Laboratory = "Laboratório São Luiz",
                Result = "Hemoglobina: 14.5 g/dL, Leucócitos: 7.500/mm³, Plaquetas: 230.000/mm³",
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            },
            new ExternalExamDto
            {
                Id = Guid.Parse("b2f7c0a1-9d5e-4c8b-8f6d-9e7a3b2c1d0e"),
                PatientId = patientId1,
                Name = "Glicemia em Jejum",
                Date = DateTime.UtcNow.AddDays(-15),
                Laboratory = "Laboratório São Luiz",
                Result = "85 mg/dL (Valor de referência: 70-99 mg/dL)",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new ExternalExamDto
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7a8b-9c0d-e1f2a3b4c5d6"),
                PatientId = patientId1,
                Name = "Perfil Lipídico",
                Date = DateTime.UtcNow.AddDays(-15),
                Laboratory = "Laboratório São Luiz",
                Result = "Colesterol Total: 180 mg/dL, HDL: 55 mg/dL, LDL: 110 mg/dL, Triglicérides: 75 mg/dL",
                CreatedAt = DateTime.UtcNow.AddDays(-15)
            },
            new ExternalExamDto
            {
                Id = Guid.Parse("1a2b3c4d-5e6f-7a8b-9c0d-1e2f3a4b5c6d"),
                PatientId = patientId2,
                Name = "Eletrocardiograma",
                Date = DateTime.UtcNow.AddDays(-45),
                Laboratory = "Clínica Cardio",
                Result = "Ritmo sinusal normal. Sem alterações significativas.",
                CreatedAt = DateTime.UtcNow.AddDays(-45)
            },
            new ExternalExamDto
            {
                Id = Guid.Parse("c5d6e7f8-a9b0-c1d2-e3f4-a5b6c7d8e9f0"),
                PatientId = patientId2,
                Name = "Ultrassonografia Abdominal",
                Date = DateTime.UtcNow.AddDays(-20),
                Laboratory = "Centro de Diagnóstico por Imagem",
                Result = "Fígado, vesícula biliar, pâncreas, baço e rins sem alterações. Sem líquido livre na cavidade.",
                CreatedAt = DateTime.UtcNow.AddDays(-20)
            }
        };
    }
} 