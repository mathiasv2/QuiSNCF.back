using QuiSNCF.Repository;

namespace QuiSNCF.Models;

public class City : IPlayable
{
    public int CityId { get; set; }
    public string CityName { get; set; }
    public DateOnly? LastTimePlayed { get; set; }
    public string DisplayName => CityName;
}