using CxIntegrator.Models;
using System.Net;

namespace CxIntegrator.Services.Interfaces
{
    internal interface ICxoneService
    {
        Task<(HttpStatusCode ResponseCode, string? ErrorMessage)> SendTicketAsync(CxoneTicket ticket);
    }
}
