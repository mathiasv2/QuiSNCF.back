using System.Globalization;
using QuiSNCF.DTO;
using QuiSNCF.Models;
using Departure = QuiSNCF.Models.Departure;

namespace QuiSNCF.Mappers;

public class SNCFApiMapperToDepartureView
{
    public static Departure Mapping(DTO.Departure d)
    {
        var scheduled = ParseSncfDateTime(d.StopDateTime.BaseDepartureDateTime);
        var real = ParseSncfDateTime(d.StopDateTime.DepartureDateTime);
        var delayMinutes = (int)(real - scheduled).TotalMinutes;

        var train = string.IsNullOrEmpty(d.DisplayInformations.CommercialMode)
            ? d.DisplayInformations.Headsign
            : $"{d.DisplayInformations.CommercialMode} {d.DisplayInformations.Headsign}";

        return new Departure(
            ScheduledTime: scheduled.ToString("HH:mm"),
            Destination: d.DisplayInformations.Direction,
            Train: train,
            Mode: d.DisplayInformations.PhysicalMode,
            DelayMinutes: delayMinutes
        );
    }

        
    private static DateTime ParseSncfDateTime(string raw)
        => DateTime.ParseExact(raw, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);
}