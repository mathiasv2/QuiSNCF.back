using QuiSNCF.DTO;
using QuiSNCF.Models;

namespace QuiSNCF.Mappers;

public static class WordToDto
{
    public static GetWordDTO ToDto(this Word station) => new()
    {
        Definition = station.Definition,
        LastTimePlayed = station.LastTimePlayed,
    };
}