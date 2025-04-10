using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FinX.Application.DTOs.Patient;
using FinX.Application.Interfaces;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinX.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IMapper mapper,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<(IEnumerable<PatientDto> Patients, int TotalCount)> GetAllAsync(int page, int pageSize, string searchTerm = null)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(page, pageSize, searchTerm);
            var totalCount = await _patientRepository.GetTotalCountAsync(searchTerm);
            var patientDtos = _mapper.Map<IEnumerable<PatientDto>>(patients);
            return (patientDtos, totalCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar todos os pacientes");
            throw;
        }
    }

    public async Task<PatientDto> GetByIdAsync(Guid id)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Paciente não encontrado com o ID: {Id}", id);
                return null;
            }

            return _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar paciente com ID: {Id}", id);
            throw;
        }
    }

    public async Task<PatientDto> CreateAsync(CreatePatientDto createPatientDto)
    {
        try
        {
            var patient = new Patient(
                createPatientDto.FirstName,
                createPatientDto.LastName,
                createPatientDto.Cpf,
                createPatientDto.DateOfBirth,
                createPatientDto.Gender,
                createPatientDto.Email,
                createPatientDto.PhoneNumber,
                createPatientDto.Address,
                createPatientDto.MedicalHistory);

            var createdPatient = await _patientRepository.CreateAsync(patient);
            return _mapper.Map<PatientDto>(createdPatient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar novo paciente");
            throw;
        }
    }

    public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientDto updatePatientDto)
    {
        try
        {
            var existingPatient = await _patientRepository.GetByIdAsync(id);
            if (existingPatient == null)
            {
                _logger.LogWarning("Paciente não encontrado para atualização com o ID: {Id}", id);
                return null;
            }

            existingPatient.Update(
                updatePatientDto.FirstName,
                updatePatientDto.LastName,
                updatePatientDto.Cpf,
                updatePatientDto.DateOfBirth,
                updatePatientDto.Gender,
                updatePatientDto.Email,
                updatePatientDto.PhoneNumber,
                updatePatientDto.Address,
                updatePatientDto.MedicalHistory);

            var updatedPatient = await _patientRepository.UpdateAsync(existingPatient);
            return _mapper.Map<PatientDto>(updatedPatient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar paciente com ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                _logger.LogWarning("Paciente não encontrado para exclusão com o ID: {Id}", id);
                return;
            }

            await _patientRepository.DeleteAsync(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao excluir paciente com ID: {Id}", id);
            throw;
        }
    }
} 