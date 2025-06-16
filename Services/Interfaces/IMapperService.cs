using CxIntegrator.Models;

namespace CxIntegrator.Services.Interfaces
{
    internal interface IMapperService
    {
        CxoneTicket Map(CrmTicket crmTicket);
    }
}
