using QuiSNCF.DTO;
using QuiSNCF.Models;

public static class StationMappingExtensions
{
    public static GetStationDTO ToDto(this Station station) => new()
    {
        PictureUrl = station.PictureUrl,
        Hint = station.Hint,
        LastTimePlayed = station.LastTimePlayed,
        RandomX = station.RandomX,
        RandomY = station.RandomY
    };
}