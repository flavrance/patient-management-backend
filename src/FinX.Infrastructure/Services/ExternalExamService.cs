using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FinX.Application.DTOs.ExternalExams;
using Microsoft.Extensions.Configuration;
using Polly;
using Polly.Retry;

namespace FinX.Infrastructure.Services
{
    public class ExternalExamService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;

        public ExternalExamService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            _retryPolicy = Policy<HttpResponseMessage>
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => 
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<IEnumerable<ExternalExamDto>> GetExamsByCpf(string cpf)
        {
            try
            {
                var response = await _retryPolicy.ExecuteAsync(async () =>
                {
                    // Mock response for testing
                    return new HttpResponseMessage
                    {
                        StatusCode = System.Net.HttpStatusCode.OK,
                        Content = new StringContent(JsonSerializer.Serialize(new List<ExternalExamDto>
                        {
                            new ExternalExamDto
                            {
                                Name = "Hemograma Completo",
                                Date = DateTime.UtcNow.AddDays(-30),
                                Laboratory = "Laboratório Central",
                                Result = "Normal"
                            },
                            new ExternalExamDto
                            {
                                Name = "Glicemia",
                                Date = DateTime.UtcNow.AddDays(-15),
                                Laboratory = "Laboratório Central",
                                Result = "95 mg/dL"
                            }
                        }))
                    };
                });

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<IEnumerable<ExternalExamDto>>(content);
                }

                // Fallback to empty list
                return Enumerable.Empty<ExternalExamDto>();
            }
            catch (Exception)
            {
                // Log error and return empty list
                return Enumerable.Empty<ExternalExamDto>();
            }
        }
    }
} 