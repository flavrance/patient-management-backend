using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FinX.Application.DTOs.MedicalHistory;
using FinX.Application.Interfaces;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinX.Application.Services;

public class MedicalHistoryService : IMedicalHistoryService
{
    private readonly IMedicalHistoryRepository _medicalHistoryRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<MedicalHistoryService> _logger;

    public MedicalHistoryService(
        IMedicalHistoryRepository medicalHistoryRepository,
        IPatientRepository patientRepository,
        IMapper mapper,
        ILogger<MedicalHistoryService> logger)
    {
        _medicalHistoryRepository = medicalHistoryRepository ?? throw new ArgumentNullException(nameof(medicalHistoryRepository));
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<IEnumerable<MedicalHistoryDto>> GetByPatientIdAsync(Guid patientId)
    {
        try
        {
            var patientExists = await _patientRepository.ExistsAsync(patientId);
            if (!patientExists)
            {
                _logger.LogWarning("Paciente não encontrado com o ID: {Id}", patientId);
                return null;
            }

            var medicalHistories = await _medicalHistoryRepository.GetByPatientIdAsync(patientId);
            return _mapper.Map<IEnumerable<MedicalHistoryDto>>(medicalHistories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar histórico médico por ID de paciente: {Id}", patientId);
            throw;
        }
    }

    public async Task<MedicalHistoryDto> GetByIdAsync(Guid id)
    {
        try
        {
            var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(id);
            if (medicalHistory == null)
            {
                _logger.LogWarning("Histórico médico não encontrado com o ID: {Id}", id);
                return null;
            }

            return _mapper.Map<MedicalHistoryDto>(medicalHistory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar histórico médico por ID: {Id}", id);
            throw;
        }
    }

    public async Task<MedicalHistoryDto> CreateAsync(CreateMedicalHistoryDto createMedicalHistoryDto)
    {
        try
        {
            var patientExists = await _patientRepository.ExistsAsync(createMedicalHistoryDto.PatientId);
            if (!patientExists)
            {
                _logger.LogWarning("Paciente não encontrado com o ID: {Id}", createMedicalHistoryDto.PatientId);
                return null;
            }

            var medicalHistory = new MedicalHistory(
                createMedicalHistoryDto.PatientId,
                createMedicalHistoryDto.Diagnosis,
                createMedicalHistoryDto.Exams,
                createMedicalHistoryDto.Prescriptions);

            var createdMedicalHistory = await _medicalHistoryRepository.CreateAsync(medicalHistory);
            return _mapper.Map<MedicalHistoryDto>(createdMedicalHistory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar histórico médico");
            throw;
        }
    }

    public async Task<MedicalHistoryDto> UpdateAsync(Guid id, UpdateMedicalHistoryDto updateMedicalHistoryDto)
    {
        try
        {
            var existingMedicalHistory = await _medicalHistoryRepository.GetByIdAsync(id);
            if (existingMedicalHistory == null)
            {
                _logger.LogWarning("Histórico médico não encontrado para atualização com o ID: {Id}", id);
                return null;
            }

            existingMedicalHistory.Update(
                updateMedicalHistoryDto.Diagnosis,
                updateMedicalHistoryDto.Exams,
                updateMedicalHistoryDto.Prescriptions);

            var updatedMedicalHistory = await _medicalHistoryRepository.UpdateAsync(existingMedicalHistory);
            return _mapper.Map<MedicalHistoryDto>(updatedMedicalHistory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar histórico médico com ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(id);
            if (medicalHistory == null)
            {
                _logger.LogWarning("Histórico médico não encontrado para exclusão com o ID: {Id}", id);
                return;
            }

            await _medicalHistoryRepository.DeleteAsync(medicalHistory);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir histórico médico com ID: {Id}", id);
            throw;
        }
    }
} 