using System.Text.Json.Serialization;

namespace QuiSNCF.DTO;

public class DeparturesResponse
{
    [JsonPropertyName("departures")]
    public List<Departure> Departures { get; set; } = [];
}