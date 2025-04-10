using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FinX.Application.DTOs.ExternalExam;
using FinX.Application.Interfaces;
using FinX.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Xunit;

namespace FinX.UnitTests.Services
{
    public class ExternalExamServiceTests
    {
        private readonly Mock<ILogger<ExternalExamService>> _mockLogger;
        private readonly Mock<HttpMessageHandler> _mockHttpHandler;
        private readonly IExternalExamService _service;

        public ExternalExamServiceTests()
        {
            _mockLogger = new Mock<ILogger<ExternalExamService>>();
            _mockHttpHandler = new Mock<HttpMessageHandler>();

            var client = new HttpClient(_mockHttpHandler.Object)
            {
                BaseAddress = new Uri("http://external-exam-api.com")
            };

            _service = new ExternalExamService(client, _mockLogger.Object);
        }

        [Fact]
        public async Task GetExamResults_WhenApiSucceeds_ReturnsResults()
        {
            // Arrange
            var examId = Guid.NewGuid();
            var expectedResponse = new ExternalExamResultDto
            {
                Id = examId,
                PatientName = "John Doe",
                ExamType = "Blood Test",
                Results = "Normal levels",
                ExamDate = DateTime.UtcNow.AddDays(-1)
            };

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(expectedResponse))
                });

            // Act
            var result = await _service.GetExamResultsAsync(examId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expectedResponse);
        }

        [Fact]
        public async Task GetExamResults_WhenApiTimeout_ReturnsFallbackResult()
        {
            // Arrange
            var examId = Guid.NewGuid();

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new TimeoutException());

            // Act
            var result = await _service.GetExamResultsAsync(examId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(examId);
            result.Status.Should().Be("Unavailable");
            result.ErrorMessage.Should().Contain("timeout");
        }

        [Fact]
        public async Task GetExamResults_WhenApiReturns404_ReturnsNull()
        {
            // Arrange
            var examId = Guid.NewGuid();

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.NotFound
                });

            // Act
            var result = await _service.GetExamResultsAsync(examId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetExamResults_WhenApiReturns500_ReturnsFallbackResult()
        {
            // Arrange
            var examId = Guid.NewGuid();

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Internal Server Error")
                });

            // Act
            var result = await _service.GetExamResultsAsync(examId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(examId);
            result.Status.Should().Be("Error");
            result.ErrorMessage.Should().Contain("500");
        }

        [Fact]
        public async Task GetExamResults_WhenNetworkError_ReturnsFallbackResult()
        {
            // Arrange
            var examId = Guid.NewGuid();

            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ThrowsAsync(new HttpRequestException("Network error"));

            // Act
            var result = await _service.GetExamResultsAsync(examId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(examId);
            result.Status.Should().Be("Error");
            result.ErrorMessage.Should().Contain("Network error");
        }
    }
} 