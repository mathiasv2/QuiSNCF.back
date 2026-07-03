using QuiSNCF.Repository;

namespace QuiSNCF.Models;

public class Station : IPlayable
{
    public int StationId { get; set; }
    public string Name { get; set; }
    public string City { get; set; }
    public string PictureUrl { get; set; }
    public string Hint { get; set; }
    public DateOnly? LastTimePlayed { get; set; }
    public double RandomX { get; set; }
    public double RandomY { get; set; }
    public string DisplayName => Name;

}