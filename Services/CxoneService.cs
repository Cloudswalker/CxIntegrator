using CxIntegrator.Models;
using CxIntegrator.Services.Interfaces;
using System.Net;
using System.Text;
using System.Text.Json;

namespace CxIntegrator.Services
{
    internal class CxoneService : ICxoneService
    {
        private readonly HttpClient _httpClient;
        private readonly string _cxoneApiUrl;

        public CxoneService(HttpClient httpClient, string cxoneApiUrl)
        {
            _httpClient = httpClient;
            _cxoneApiUrl = cxoneApiUrl;
        }

        public async Task<(HttpStatusCode ResponseCode, string? ErrorMessage)> SendTicketAsync(CxoneTicket ticket)
        {
            try
            {
                var json = JsonSerializer.Serialize(ticket);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_cxoneApiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return (response.StatusCode, null);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return (response.StatusCode, errorContent);
                }
            }
            catch (Exception ex)
            {                
                return (HttpStatusCode.InternalServerError, $"CxIntegrator internal error: {ex.Message}");
            }
        }
    }
}
