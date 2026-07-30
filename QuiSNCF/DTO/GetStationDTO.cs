namespace QuiSNCF.DTO;

public class GetStationDTO
{
    public string PictureUrl { get; set; }
    public string Hint { get; set; }
    public DateOnly? LastTimePlayed { get; set; }
    public double RandomX { get; set; }
    public double RandomY { get; set; }
}