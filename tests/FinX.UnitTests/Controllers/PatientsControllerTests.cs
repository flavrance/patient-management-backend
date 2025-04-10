using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinX.API.Controllers;
using FinX.Application.DTOs.Patient;
using FinX.Application.Interfaces;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using System.Linq;

namespace FinX.UnitTests.Controllers
{
    public class PatientsControllerTests
    {
        private readonly Mock<IPatientService> _mockPatientService;
        private readonly Mock<IValidator<CreatePatientDto>> _mockCreateValidator;
        private readonly Mock<IValidator<UpdatePatientDto>> _mockUpdateValidator;
        private readonly PatientsController _controller;

        public PatientsControllerTests()
        {
            _mockPatientService = new Mock<IPatientService>();
            _mockCreateValidator = new Mock<IValidator<CreatePatientDto>>();
            _mockUpdateValidator = new Mock<IValidator<UpdatePatientDto>>();
            
            _controller = new PatientsController(
                _mockPatientService.Object,
                _mockCreateValidator.Object,
                _mockUpdateValidator.Object
            );
        }

        [Fact]
        public async Task GetAll_ReturnsOkResult_WithPatients()
        {
            // Arrange
            var expectedPatients = new List<PatientDto>
            {
                new PatientDto { Id = Guid.NewGuid(), Name = "John Doe", Cpf = "12345678901", BirthDate = DateTime.Now.AddYears(-30) },
                new PatientDto { Id = Guid.NewGuid(), Name = "Jane Doe", Cpf = "98765432101", BirthDate = DateTime.Now.AddYears(-25) }
            };

            _mockPatientService
                .Setup(service => service.GetAllAsync(1, 10, null))
                .ReturnsAsync(expectedPatients);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPatients = Assert.IsAssignableFrom<IEnumerable<PatientDto>>(okResult.Value);
            Assert.Equal(expectedPatients, returnedPatients);
        }

        [Fact]
        public async Task GetById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var expectedPatient = new PatientDto { Id = patientId, Name = "John Doe" };

            _mockPatientService
                .Setup(service => service.GetByIdAsync(patientId))
                .ReturnsAsync(expectedPatient);

            // Act
            var result = await _controller.GetById(patientId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientDto>(okResult.Value);
            Assert.Equal(expectedPatient, returnedPatient);
        }

        [Fact]
        public async Task Create_WithValidPatient_ReturnsCreatedAtAction()
        {
            // Arrange
            var createDto = new CreatePatientDto { Name = "John Doe" };
            var createdPatient = new PatientDto { Id = Guid.NewGuid(), Name = "John Doe" };

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            _mockPatientService
                .Setup(service => service.CreateAsync(createDto))
                .ReturnsAsync(createdPatient);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientDto>(createdAtActionResult.Value);
            Assert.Equal(createdPatient, returnedPatient);
        }

        [Fact]
        public async Task Update_WithValidPatient_ReturnsOkResult()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var updateDto = new UpdatePatientDto { Name = "John Doe Updated" };
            var updatedPatient = new PatientDto { Id = patientId, Name = "John Doe Updated" };

            _mockUpdateValidator
                .Setup(validator => validator.ValidateAsync(updateDto, default))
                .ReturnsAsync(new ValidationResult());

            _mockPatientService
                .Setup(service => service.UpdateAsync(patientId, updateDto))
                .ReturnsAsync(updatedPatient);

            // Act
            var result = await _controller.Update(patientId, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPatient = Assert.IsType<PatientDto>(okResult.Value);
            Assert.Equal(updatedPatient, returnedPatient);
        }

        [Fact]
        public async Task Delete_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var patientId = Guid.NewGuid();

            _mockPatientService
                .Setup(service => service.DeleteAsync(patientId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(patientId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Create_WithInvalidCpf_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreatePatientDto 
            { 
                Name = "John Doe",
                Cpf = "invalid_cpf",
                BirthDate = DateTime.Now.AddYears(-30)
            };

            var validationResult = new ValidationResult(new[] 
            {
                new ValidationFailure("Cpf", "Invalid CPF format")
            });

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);
            errors.Should().ContainSingle(e => e.PropertyName == "Cpf" && e.ErrorMessage == "Invalid CPF format");
        }

        [Fact]
        public async Task Create_WithDuplicateCpf_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreatePatientDto 
            { 
                Name = "John Doe",
                Cpf = "12345678901",
                BirthDate = DateTime.Now.AddYears(-30)
            };

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(new ValidationResult());

            _mockPatientService
                .Setup(service => service.CreateAsync(createDto))
                .ThrowsAsync(new InvalidOperationException("CPF already exists"));

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("Internal server error", statusCodeResult.Value);
        }

        [Fact]
        public async Task Create_WithShortName_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreatePatientDto 
            { 
                Name = "Jo",
                Cpf = "12345678901",
                BirthDate = DateTime.Now.AddYears(-30)
            };

            var validationResult = new ValidationResult(new[] 
            {
                new ValidationFailure("Name", "Name must be at least 3 characters long")
            });

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);
            errors.Should().ContainSingle(e => e.PropertyName == "Name" && e.ErrorMessage == "Name must be at least 3 characters long");
        }

        [Fact]
        public async Task Create_WithFutureBirthDate_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreatePatientDto 
            { 
                Name = "John Doe",
                Cpf = "12345678901",
                BirthDate = DateTime.Now.AddYears(1)
            };

            var validationResult = new ValidationResult(new[] 
            {
                new ValidationFailure("BirthDate", "Birth date cannot be in the future")
            });

            _mockCreateValidator
                .Setup(validator => validator.ValidateAsync(createDto, default))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var errors = Assert.IsAssignableFrom<IEnumerable<ValidationFailure>>(badRequestResult.Value);
            errors.Should().ContainSingle(e => e.PropertyName == "BirthDate" && e.ErrorMessage == "Birth date cannot be in the future");
        }
    }
} 