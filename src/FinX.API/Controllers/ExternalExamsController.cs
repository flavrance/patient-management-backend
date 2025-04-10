using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.Application.DTOs.ExternalExams;
using FinX.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace FinX.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExternalExamsController : ControllerBase
    {
        private readonly IExternalExamService _externalExamService;
        private readonly IPatientService _patientService;
        private readonly ILogger<ExternalExamsController> _logger;

        public ExternalExamsController(
            IExternalExamService externalExamService,
            IPatientService patientService,
            ILogger<ExternalExamsController> logger)
        {
            _externalExamService = externalExamService ?? throw new ArgumentNullException(nameof(externalExamService));
            _patientService = patientService ?? throw new ArgumentNullException(nameof(patientService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Get external exams for a specific patient
        /// </summary>
        /// <param name="patientId">Patient ID</param>
        /// <returns>List of external exams for the patient</returns>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<ExternalExamDto>>> GetByPatientId(Guid patientId)
        {
            try
            {
                var patient = await _patientService.GetByIdAsync(patientId);
                if (patient == null)
                    return NotFound($"Patient with ID {patientId} not found");

                // Use the mock service to get exams by patient ID
                var exams = await _externalExamService.GetByPatientIdAsync(patientId);
                return Ok(exams);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving external exams for patient: {PatientId}", patientId);
                return StatusCode(500, "Internal server error while retrieving external exams");
            }
        }
    }
} 