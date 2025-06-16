using CxIntegrator.Models;
using CxIntegrator.Services.Interfaces;

namespace CxIntegrator.Services
{
    internal class MapperService : IMapperService
    {
        public CxoneTicket Map(CrmTicket crm)
        {
            return new CxoneTicket
            {
                TicketId = crm.Id.ToString(),
                Subject = crm.Title,
                Description = crm.Body,
                Created = crm.CreatedAt,
                Priority = crm.Priority,
                Tags = crm.Tags
            };
        }
    }
}
