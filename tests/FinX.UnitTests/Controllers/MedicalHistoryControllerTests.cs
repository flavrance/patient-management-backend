using System;
using System.Threading.Tasks;
using FinX.API.Controllers;
using FinX.Application.DTOs.MedicalHistory;
using FinX.Application.Interfaces;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FinX.UnitTests.Controllers
{
    public class MedicalHistoryControllerTests
    {
        private readonly Mock<IMedicalHistoryService> _mockMedicalHistoryService;
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IValidator<CreateMedicalHistoryDto>> _mockCreateValidator;
        private readonly MedicalHistoryController _controller;

        public MedicalHistoryControllerTests()
        {
            _mockMedicalHistoryService = new Mock<IMedicalHistoryService>();
            _mockPatientService = new Mock<IPatientService>();
            _mockCreateValidator = new Mock<IValidator<CreateMedicalHistoryDto>>();

            _controller = new MedicalHistoryController(
                _mockMedicalHistoryService.Object,
                _mockPatientService.Object,
                _mockCreateValidator.Object
            );
        }

        [Fact]
        public async Task Create_WithNonExistentPatient_ReturnsBadRequest()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var createDto = new CreateMedicalHistoryDto
            {
                PatientId = patientId,
                Description = "Regular checkup",
                Diagnosis = "Healthy",
                Prescription = "None"
            };

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            _mockPatientService
                .Setup(service => service.GetByIdAsync(patientId))
                .ReturnsAsync((PatientDto)null);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            badRequestResult.Value.Should().Be("Patient not found");
        }

        [Fact]
        public async Task Create_WithMissingRequiredFields_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateMedicalHistoryDto
            {
                PatientId = Guid.NewGuid(),
                Description = "", // Required field missing
                Diagnosis = "",   // Required field missing
                Prescription = "None"
            };

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Description", "Description is required"),
                new ValidationFailure("Diagnosis", "Diagnosis is required")
            });

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);
            errors.Should().HaveCount(2);
            errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage == "Description is required");
            errors.Should().Contain(e => e.PropertyName == "Diagnosis" && e.ErrorMessage == "Diagnosis is required");
        }

        [Fact]
        public async Task Create_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var createDto = new CreateMedicalHistoryDto
            {
                PatientId = patientId,
                Description = "Regular checkup",
                Diagnosis = "Healthy",
                Prescription = "None"
            };

            var createdHistory = new MedicalHistoryDto
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                Description = createDto.Description,
                Diagnosis = createDto.Diagnosis,
                Prescription = createDto.Prescription,
                CreatedAt = DateTime.UtcNow
            };

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            _mockPatientService
                .Setup(service => service.GetByIdAsync(patientId))
                .ReturnsAsync(new PatientDto { Id = patientId, Name = "John Doe" });

            _mockMedicalHistoryService
                .Setup(service => service.CreateAsync(createDto))
                .ReturnsAsync(createdHistory);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedHistory = Assert.IsType<MedicalHistoryDto>(createdAtActionResult.Value);
            returnedHistory.Should().BeEquivalentTo(createdHistory);
        }

        [Fact]
        public async Task GetByPatientId_WithNonExistentPatient_ReturnsNotFound()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _mockPatientService
                .Setup(service => service.GetByIdAsync(patientId))
                .ReturnsAsync((PatientDto)null);

            // Act
            var result = await _controller.GetByPatientId(patientId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetByPatientId_WithExistingPatient_ReturnsHistory()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var histories = new List<MedicalHistoryDto>
            {
                new MedicalHistoryDto
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    Description = "Regular checkup",
                    Diagnosis = "Healthy",
                    Prescription = "None",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new MedicalHistoryDto
                {
                    Id = Guid.NewGuid(),
                    PatientId = patientId,
                    Description = "Follow-up",
                    Diagnosis = "Recovering",
                    Prescription = "Continue medication",
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockPatientService
                .Setup(service => service.GetByIdAsync(patientId))
                .ReturnsAsync(new PatientDto { Id = patientId, Name = "John Doe" });

            _mockMedicalHistoryService
                .Setup(service => service.GetByPatientIdAsync(patientId))
                .ReturnsAsync(histories);

            // Act
            var result = await _controller.GetByPatientId(patientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistories = Assert.IsAssignableFrom<IEnumerable<MedicalHistoryDto>>(okResult.Value);
            returnedHistories.Should().BeEquivalentTo(histories);
        }
    }
} 