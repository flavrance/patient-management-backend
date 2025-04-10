using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.Patient;
using FinX.Application.Interfaces;
using FinX.Application.Validators.Patient;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IValidator<CreatePatientDto> _createValidator;
        private readonly IValidator<UpdatePatientDto> _updateValidator;
        private readonly ILogger<PatientsController> _logger;

        public PatientsController(
            IPatientService patientService,
            IValidator<CreatePatientDto> createValidator,
            IValidator<UpdatePatientDto> updateValidator,
            ILogger<PatientsController> logger)
        {
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null)
        {
            try
            {
                var patients = await _patientService.GetAllAsync(page, pageSize, searchTerm);
                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patients");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            try
            {
                var patient = await _patientService.GetByIdAsync(id);
                if (patient == null)
                    return NotFound();

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by id");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto createPatientDto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(createPatientDto);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var patient = await _patientService.CreateAsync(createPatientDto);
                return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(updatePatientDto);
                if (!validationResult.IsValid)
                    return BadRequest(validationResult.Errors);

                var patient = await _patientService.UpdateAsync(id, updatePatientDto);
                if (patient == null)
                    return NotFound();

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _patientService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient");
                return StatusCode(500, "Internal server error");
            }
        }
    }
} 