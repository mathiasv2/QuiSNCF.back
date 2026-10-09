using System.Globalization;
using QuiSNCF.DTO;
using QuiSNCF.Models;
using Departure = QuiSNCF.Models.Departure;

namespace QuiSNCF.Mappers;

public class SNCFApiMapperToDepartureView
{
    public static Departure? Mapping(DTO.Departure d)
    {
        var scheduled= ParseSncfDateTime(d.StopDateTime?.BaseDepartureDateTime);
        var real= ParseSncfDateTime(d.StopDateTime?.DepartureDateTime);

        if (scheduled is null && real is null)
            return null;

        var sched = scheduled ?? real!.Value;
        var actual = real ?? sched;
        var delayMinutes = (int)(actual - sched).TotalMinutes;

        var info = d.DisplayInformations;
        var train = string.IsNullOrEmpty(info.CommercialMode)
            ? info.Headsign
            : $"{info.CommercialMode} {info.Headsign}";

        return new Departure(
            ScheduledTime: sched.ToString("HH:mm"),
            Destination: info.Direction,
            Train: train,
            Mode: info.PhysicalMode,
            DelayMinutes: delayMinutes
        );
    }

    private static DateTime? ParseSncfDateTime(string? raw)
        => DateTime.TryParseExact(raw, "yyyyMMdd'T'HHmmss",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
            ? dt
            : null;

}