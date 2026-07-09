using System.Text.Json.Serialization;

namespace QuiSNCF.DTO;

public class DisplayInformations
{
    [JsonPropertyName("direction")]
    public string Direction { get; set; } = "";

    [JsonPropertyName("headsign")]
    public string Headsign { get; set; } = "";

    [JsonPropertyName("commercial_mode")]
    public string CommercialMode { get; set; } = "";

    [JsonPropertyName("physical_mode")]
    public string PhysicalMode { get; set; } = "";
}