using System.Text.Json.Serialization;

namespace QuiSNCF.DTO;

public class Departure
{
    [JsonPropertyName("stop_date_time")]
    public StopDateTime StopDateTime { get; set; } = default!;

    [JsonPropertyName("display_informations")]
    public DisplayInformations DisplayInformations { get; set; } = default!;
}