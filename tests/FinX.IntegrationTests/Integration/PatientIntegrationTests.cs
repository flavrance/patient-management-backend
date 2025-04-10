using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FinX.API;
using FinX.Application.DTOs.Patient;
using FinX.Application.DTOs.MedicalHistory;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using FluentAssertions;

namespace FinX.UnitTests.Integration
{
    public class PatientIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly string _validToken;

        public PatientIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
            
            // In a real scenario, this would be obtained through authentication
            _validToken = "your_valid_jwt_token";
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _validToken);
        }

        [Fact]
        public async Task CompletePatientJourney_Success()
        {
            // Step 1: Create a new patient
            var createPatientDto = new CreatePatientDto
            {
                Name = "John Doe",
                Cpf = "12345678901",
                BirthDate = DateTime.Now.AddYears(-30)
            };

            var createPatientResponse = await _client.PostAsync("/api/patients",
                new StringContent(JsonSerializer.Serialize(createPatientDto), Encoding.UTF8, "application/json"));

            createPatientResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdPatient = await JsonSerializer.DeserializeAsync<PatientDto>(
                await createPatientResponse.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            // Step 2: Get the created patient
            var getPatientResponse = await _client.GetAsync($"/api/patients/{createdPatient.Id}");
            getPatientResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var retrievedPatient = await JsonSerializer.DeserializeAsync<PatientDto>(
                await getPatientResponse.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            retrievedPatient.Should().BeEquivalentTo(createdPatient);

            // Step 3: Add medical history
            var createHistoryDto = new CreateMedicalHistoryDto
            {
                PatientId = createdPatient.Id,
                Description = "Initial checkup",
                Diagnosis = "Healthy",
                Prescription = "Regular exercise"
            };

            var createHistoryResponse = await _client.PostAsync("/api/medical-history",
                new StringContent(JsonSerializer.Serialize(createHistoryDto), Encoding.UTF8, "application/json"));

            createHistoryResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var createdHistory = await JsonSerializer.DeserializeAsync<MedicalHistoryDto>(
                await createHistoryResponse.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            // Step 4: Get patient's medical history
            var getHistoryResponse = await _client.GetAsync($"/api/medical-history/patient/{createdPatient.Id}");
            getHistoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var histories = await JsonSerializer.DeserializeAsync<MedicalHistoryDto[]>(
                await getHistoryResponse.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            histories.Should().ContainSingle(h => h.Id == createdHistory.Id);

            // Step 5: Update patient
            var updatePatientDto = new UpdatePatientDto
            {
                Name = "John Doe Jr.",
                Cpf = createdPatient.Cpf,
                BirthDate = createdPatient.BirthDate
            };

            var updatePatientResponse = await _client.PutAsync($"/api/patients/{createdPatient.Id}",
                new StringContent(JsonSerializer.Serialize(updatePatientDto), Encoding.UTF8, "application/json"));

            updatePatientResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedPatient = await JsonSerializer.DeserializeAsync<PatientDto>(
                await updatePatientResponse.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            updatedPatient.Name.Should().Be("John Doe Jr.");

            // Step 6: Delete patient
            var deleteResponse = await _client.DeleteAsync($"/api/patients/{createdPatient.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // Step 7: Verify patient is deleted
            var getDeletedPatientResponse = await _client.GetAsync($"/api/patients/{createdPatient.Id}");
            getDeletedPatientResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task UnauthorizedAccess_ReturnsUnauthorized()
        {
            // Remove authorization header
            _client.DefaultRequestHeaders.Authorization = null;

            var response = await _client.GetAsync("/api/patients");
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task CreatePatient_WithInvalidData_ReturnsBadRequest()
        {
            var invalidPatient = new CreatePatientDto
            {
                Name = "Jo", // Too short
                Cpf = "invalid_cpf",
                BirthDate = DateTime.Now.AddYears(1) // Future date
            };

            var response = await _client.PostAsync("/api/patients",
                new StringContent(JsonSerializer.Serialize(invalidPatient), Encoding.UTF8, "application/json"));

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var errors = await JsonSerializer.DeserializeAsync<ValidationProblemDetails>(
                await response.Content.ReadAsStreamAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            errors.Errors.Should().ContainKeys("Name", "Cpf", "BirthDate");
        }

        [Fact]
        public async Task GetPatient_WithInvalidId_ReturnsNotFound()
        {
            var invalidId = Guid.NewGuid();
            var response = await _client.GetAsync($"/api/patients/{invalidId}");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
} 