using System.Text.Json.Serialization;

namespace CxIntegrator.Models
{
    public record CxoneTicket
    {
        public string TicketId { get; init; } = "";
        public string Subject { get; init; } = "";
        public string Description { get; init; } = "";
        public DateTime Created { get; init; }
        public PriorityLevel Priority { get; init; } = PriorityLevel.Normal;
        public List<string> Tags { get; init; } = new();
    }
}
