using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using QuiSNCF.DTO;
using QuiSNCF.Mappers;
using QuiSNCF.Models;
using QuiSNCF.Repository;
using Departure = QuiSNCF.Models.Departure;

namespace QuiSNCF.Service;

public class SNCFApiRequest(IConfiguration config, ICityRepository repo)
{
    string? key = config.GetSection("SNCFApiKey").Value;
    HttpClient _client = new HttpClient();
    public async Task<string> GetCityId()
    {
        var city = await repo.GetOrPickRandomCity();
        string cityName = city.CityName;
        
        var authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{key}:"));
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);

        var response = await _client.GetAsync(
            $"https://api.sncf.com/v1/coverage/sncf/places?q={cityName}&type[]=stop_area");

        var json = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(json);
        var places = doc.RootElement.GetProperty("places");

        var result = places[0].GetProperty("id").GetString();
        return result;
    }
    
    public async Task<List<Departure>> GetNextDepartures()
    {
        var stopAreaId = await GetCityId();
        Console.WriteLine(stopAreaId);
        
        var response = await _client.GetAsync(
            $"https://api.sncf.com/v1/coverage/sncf/stop_areas/{stopAreaId}/departures?count=10");
        
        var json = await response.Content.ReadAsStringAsync();

        var data = JsonSerializer.Deserialize<DeparturesResponse>(json);

        return data.Departures.Select(SNCFApiMapperToDepartureView.Mapping).ToList();
    }
    

}