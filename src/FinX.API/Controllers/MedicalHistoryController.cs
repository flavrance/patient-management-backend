using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.MedicalHistory;
using FinX.Application.DTOs.Patient;
using FinX.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MedicalHistoryController : ControllerBase
    {
        private readonly IMedicalHistoryService _medicalHistoryService;
        private readonly IPatientService _patientService;
        private readonly IValidator<CreateMedicalHistoryDto> _createValidator;
        private readonly ILogger<MedicalHistoryController> _logger;

        public MedicalHistoryController(
            IMedicalHistoryService medicalHistoryService,
            IPatientService patientService,
            IValidator<CreateMedicalHistoryDto> createValidator,
            ILogger<MedicalHistoryController> logger)
        {
            _medicalHistoryService = medicalHistoryService ?? throw new ArgumentNullException(nameof(medicalHistoryService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicalHistoryDto>>> GetByPatientId(Guid patientId)
        {
            try
            {
                var patient = await _patientService.GetByIdAsync(patientId);
                if (patient == null)
                    return NotFound();

                var medicalHistories = await _medicalHistoryService.GetByPatientIdAsync(patientId);
                return Ok(medicalHistories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical history for patient");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalHistoryDto>> GetById(Guid id)
        {
            try
            {
                var medicalHistory = await _medicalHistoryService.GetByIdAsync(id);
                if (medicalHistory == null)
                    return NotFound();

                return Ok(medicalHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting medical history by id");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<MedicalHistoryDto>> Create([FromBody] CreateMedicalHistoryDto createMedicalHistoryDto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(createMedicalHistoryDto);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var patient = await _patientService.GetByIdAsync(createMedicalHistoryDto.PatientId);
                if (patient == null)
                    return BadRequest("Patient not found");

                var medicalHistory = await _medicalHistoryService.CreateAsync(createMedicalHistoryDto);
                return CreatedAtAction(nameof(GetById), new { id = medicalHistory.Id }, medicalHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating medical history");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MedicalHistoryDto>> Update(Guid id, [FromBody] UpdateMedicalHistoryDto updateMedicalHistoryDto)
        {
            try
            {
                var medicalHistory = await _medicalHistoryService.UpdateAsync(id, updateMedicalHistoryDto);
                if (medicalHistory == null)
                    return NotFound();

                return Ok(medicalHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating medical history");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var medicalHistory = await _medicalHistoryService.GetByIdAsync(id);
                if (medicalHistory == null)
                    return NotFound();

                await _medicalHistoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting medical history");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 