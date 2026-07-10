using System.Text.Json.Serialization;

namespace QuiSNCF.DTO;

public class StopDateTime
{
    [JsonPropertyName("departure_date_time")]
    public string DepartureDateTime { get; set; } = "";

    [JsonPropertyName("base_departure_date_time")]
    public string BaseDepartureDateTime { get; set; } = "";

    [JsonPropertyName("data_freshness")]
    public string DataFreshness { get; set; } = "";
}