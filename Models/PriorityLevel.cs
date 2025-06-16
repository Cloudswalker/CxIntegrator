using System.Text.Json.Serialization;

namespace CxIntegrator.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PriorityLevel
    {
        Low,
        Normal,
        High
    }
}
