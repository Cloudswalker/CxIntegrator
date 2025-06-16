using CxIntegrator.Models;
using CxIntegrator.Services.Interfaces;
using System.Text.Json;

namespace CxIntegrator.Services
{
    internal class CrmService : ICrmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _crmApiUrl;
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public CrmService(HttpClient httpClient, string crmApiUrl)
        {
            _httpClient = httpClient;
            _crmApiUrl = crmApiUrl;
        }

        public async Task<List<CrmTicket>> GetTicketsAsync()
        {
            var response = await _httpClient.GetAsync(_crmApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var crmTickets = JsonSerializer.Deserialize<List<CrmTicket>>(json, _jsonSerializerOptions) ?? [];

            crmTickets = [.. crmTickets.Select(ticket => new CrmTicket
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Body = ticket.Body,
                    // Next fields are set to default values if not provided in the API response
                    CreatedAt = DateTime.UtcNow,
                    Priority = PriorityLevel.Normal,
                    Tags = new List<string> { "sample", "api" }
                })];

            return crmTickets;
        }
    }
}
