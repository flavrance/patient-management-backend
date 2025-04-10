using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FinX.Application.DTOs.Appointment;
using FinX.Application.Interfaces;
using FinX.Domain.Entities;
using FinX.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace FinX.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IMapper mapper,
            ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository ?? throw new ArgumentNullException(nameof(appointmentRepository));
            _patientRepository = patientRepository ?? throw new ArgumentNullException(nameof(patientRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAsync(int page, int pageSize, Guid? patientId, DateTime? fromDate, DateTime? toDate)
        {
            try
            {
                // Use the repository method that now handles filtering and pagination
                var appointments = await _appointmentRepository.GetAllAsync(
                    page,
                    pageSize,
                    patientId,
                    fromDate,
                    toDate);
                
                return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all appointments");
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(Guid patientId)
        {
            try
            {
                var patientExists = await _patientRepository.ExistsAsync(patientId);
                if (!patientExists)
                {
                    _logger.LogWarning("Patient not found: {Id}", patientId);
                    return null;
                }

                var appointments = await _appointmentRepository.GetByPatientIdAsync(patientId);
                return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments for patient: {PatientId}", patientId);
                throw;
            }
        }

        public async Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            try
            {
                if (start > end)
                {
                    _logger.LogWarning("Invalid date range: start {Start} is later than end {End}", start, end);
                    throw new ArgumentException("Start date must be before end date");
                }

                var appointments = await _appointmentRepository.GetByDateRangeAsync(start, end);
                return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointments between {Start} and {End}", start, end);
                throw;
            }
        }

        public async Task<AppointmentDto> GetByIdAsync(Guid id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment not found: {Id}", id);
                    return null;
                }

                return _mapper.Map<AppointmentDto>(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting appointment: {Id}", id);
                throw;
            }
        }

        public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto createAppointmentDto)
        {
            try
            {
                var patientExists = await _patientRepository.ExistsAsync(createAppointmentDto.PatientId);
                if (!patientExists)
                {
                    _logger.LogWarning("Patient not found: {Id}", createAppointmentDto.PatientId);
                    throw new ArgumentException($"Patient with ID {createAppointmentDto.PatientId} not found");
                }

                var appointment = new Appointment(
                    createAppointmentDto.PatientId,
                    createAppointmentDto.AppointmentDate,
                    createAppointmentDto.Duration,
                    createAppointmentDto.Type,
                    createAppointmentDto.Notes);

                var createdAppointment = await _appointmentRepository.CreateAsync(appointment);
                return _mapper.Map<AppointmentDto>(createdAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                throw;
            }
        }

        public async Task<AppointmentDto> UpdateAsync(Guid id, UpdateAppointmentDto updateAppointmentDto)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment not found for update: {Id}", id);
                    return null;
                }

                var patientExists = await _patientRepository.ExistsAsync(updateAppointmentDto.PatientId);
                if (!patientExists)
                {
                    _logger.LogWarning("Patient not found: {Id}", updateAppointmentDto.PatientId);
                    throw new ArgumentException($"Patient with ID {updateAppointmentDto.PatientId} not found");
                }

                appointment.Update(
                    updateAppointmentDto.AppointmentDate,
                    updateAppointmentDto.Duration,
                    updateAppointmentDto.Status,
                    updateAppointmentDto.Type,
                    updateAppointmentDto.Notes);

                var updatedAppointment = await _appointmentRepository.UpdateAsync(appointment);
                return _mapper.Map<AppointmentDto>(updatedAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    _logger.LogWarning("Appointment not found for deletion: {Id}", id);
                    return false;
                }

                await _appointmentRepository.DeleteAsync(appointment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment: {Id}", id);
                throw;
            }
        }
    }
} 