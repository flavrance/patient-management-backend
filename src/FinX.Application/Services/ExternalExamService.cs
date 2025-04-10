using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FinX.Application.DTOs.ExternalExams;
using FinX.Application.Interfaces;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinX.Application.Services;

public class ExternalExamService : IExternalExamService
{
    private readonly IExternalExamRepository _externalExamRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<ExternalExamService> _logger;

    public ExternalExamService(
        IExternalExamRepository externalExamRepository,
        IPatientRepository patientRepository,
        IMapper mapper,
        ILogger<ExternalExamService> logger)
    {
        _externalExamRepository = externalExamRepository ?? throw new ArgumentNullException(nameof(externalExamRepository));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<ExternalExamDto>> GetByPatientIdAsync(Guid patientId)
    {
        try
        {
            var patientExists = await _patientRepository.ExistsAsync(patientId);
            if (!patientExists)
            {
                _logger.LogWarning("Paciente não encontrado com o ID: {Id}", patientId);
                return null;
            }

            var externalExams = await _externalExamRepository.GetByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<ExternalExamDto>>(externalExams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar exames externos por ID de paciente: {Id}", patientId);
            throw;
        }
    }

    public async Task<ExternalExamDto> GetByIdAsync(Guid id)
    {
        try
        {
            var externalExam = await _externalExamRepository.GetByIdAsync(id);
            if (externalExam == null)
            {
                _logger.LogWarning("Exame externo não encontrado com o ID: {Id}", id);
                return null;
            }

            return _mapper.Map<ExternalExamDto>(externalExam);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar exame externo por ID: {Id}", id);
            throw;
        }
    }

    public async Task<ExternalExamDto> CreateAsync(CreateExternalExamDto createExternalExamDto)
    {
        try
        {
            var patientExists = await _patientRepository.ExistsAsync(createExternalExamDto.PatientId);
            if (!patientExists)
            {
                _logger.LogWarning("Paciente não encontrado com o ID: {Id}", createExternalExamDto.PatientId);
                return null;
            }

            var externalExam = new ExternalExam(
                createExternalExamDto.PatientId,
                createExternalExamDto.Name,
                createExternalExamDto.Date,
                createExternalExamDto.Laboratory,
                createExternalExamDto.Result);

            var createdExam = await _externalExamRepository.CreateAsync(externalExam);
            return _mapper.Map<ExternalExamDto>(createdExam);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar exame externo");
            throw;
        }
    }

    public async Task<ExternalExamDto> UpdateAsync(Guid id, UpdateExternalExamDto updateExternalExamDto)
    {
        try
        {
            var existingExam = await _externalExamRepository.GetByIdAsync(id);
            if (existingExam == null)
            {
                _logger.LogWarning("Exame externo não encontrado para atualização com o ID: {Id}", id);
                return null;
            }

            existingExam.Update(
                updateExternalExamDto.Name,
                updateExternalExamDto.Date,
                updateExternalExamDto.Laboratory,
                updateExternalExamDto.Result);

            var updatedExam = await _externalExamRepository.UpdateAsync(existingExam);
            return _mapper.Map<ExternalExamDto>(updatedExam);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar exame externo com ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var externalExam = await _externalExamRepository.GetByIdAsync(id);
            if (externalExam == null)
            {
                _logger.LogWarning("Exame externo não encontrado para exclusão com o ID: {Id}", id);
                return;
            }

            await _externalExamRepository.DeleteAsync(externalExam);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir exame externo com ID: {Id}", id);
            throw;
        }
    }
} 