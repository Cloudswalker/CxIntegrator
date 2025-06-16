namespace CxIntegrator.Models
{
    public record CrmTicket
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Body { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public PriorityLevel Priority { get; init; } = PriorityLevel.Normal;
        public List<string> Tags { get; init; } = new();
    }
}
