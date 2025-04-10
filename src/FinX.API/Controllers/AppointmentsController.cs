using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FinX.Application.DTOs.Appointment;
using FinX.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace FinX.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IValidator<CreateAppointmentDto> _createValidator;
        private readonly IValidator<UpdateAppointmentDto> _updateValidator;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IValidator<CreateAppointmentDto> createValidator,
            IValidator<UpdateAppointmentDto> updateValidator,
            ILogger<AppointmentsController> logger)
        {
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
            _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
            _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Retrieves a list of all appointments with optional filtering
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="patientId">Optional patient ID to filter appointments</param>
        /// <param name="fromDate">Optional start date to filter appointments</param>
        /// <param name="toDate">Optional end date to filter appointments</param>
        /// <returns>A list of appointments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? patientId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var appointments = await _appointmentService.GetAllAsync(page, pageSize, patientId, fromDate, toDate);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving appointments");
            }
        }

        /// <summary>
        /// Retrieves a specific appointment by ID
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <returns>The requested appointment</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {id} not found");
                }
                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment with ID {AppointmentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the appointment");
            }
        }

        /// <summary>
        /// Creates a new appointment
        /// </summary>
        /// <param name="createDto">Appointment data</param>
        /// <returns>The created appointment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto createDto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(createDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var appointment = await _appointmentService.CreateAsync(createDto);
                return CreatedAtAction(nameof(GetById), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the appointment");
            }
        }

        /// <summary>
        /// Updates an existing appointment
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <param name="updateDto">Updated appointment data</param>
        /// <returns>The updated appointment</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAppointmentDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return BadRequest("ID in URL does not match ID in request body");
                }

                var validationResult = await _updateValidator.ValidateAsync(updateDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(validationResult.Errors);
                }

                var appointment = await _appointmentService.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {id} not found");
                }

                var updatedAppointment = await _appointmentService.UpdateAsync(id, updateDto);
                return Ok(updatedAppointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating appointment with ID {AppointmentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating the appointment");
            }
        }

        /// <summary>
        /// Deletes an appointment
        /// </summary>
        /// <param name="id">Appointment ID</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetByIdAsync(id);
                if (appointment == null)
                {
                    return NotFound($"Appointment with ID {id} not found");
                }

                await _appointmentService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting appointment with ID {AppointmentId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while deleting the appointment");
            }
        }
    }
} 