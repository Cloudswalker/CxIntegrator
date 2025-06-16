using CxIntegrator.Models;

namespace CxIntegrator.Services.Interfaces
{
    internal interface ICrmService
    {
        Task<List<CrmTicket>> GetTicketsAsync();
    }
}
