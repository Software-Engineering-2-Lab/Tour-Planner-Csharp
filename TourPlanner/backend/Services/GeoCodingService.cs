using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using TourPlanner.backend.Enums;
using Serilog;
namespace TourPlanner.backend.Services;

public class GeocodingService : IGeocodingService
{
  private readonly HttpClient _httpClient;
  private readonly string _apiKey;

  public GeocodingService (HttpClient httpClient , IConfiguration configuration)
  {
    _httpClient=httpClient;
    _apiKey=configuration["OpenRouteService:ApiKey"] ?? throw new ArgumentNullException("ORS ApiKey is missing");
  }

  public async Task<(double lon , double lat)> GetCoordinatesAsync (String address)
  {
    var url = $"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={Uri.EscapeDataString(address)}&size=1";

    Log.Information("[DEBUG ORS] URL-ul generat este: {Url}", url);
        
    var response = await _httpClient.GetAsync(url);
    response.EnsureSuccessStatusCode();

    var json = await response.Content.ReadAsStringAsync();
    using var doc = JsonDocument.Parse(json);
    
    var features = doc.RootElement.GetProperty("features");
    if (features.GetArrayLength() == 0) throw new Exception($"Location '{address}' not found.");

    var coordinates = features[0].GetProperty("geometry").GetProperty("coordinates");
    
    return (coordinates[0].GetDouble(), coordinates[1].GetDouble());
}

public async Task<(double distance , double duration , string geometry)> GetRouteAsync ((double lon , double lat)start , (double lon , double lat)end , TransportType transportType)
      {
        
          string profile = transportType.ToString().ToLower() switch
          {
              "bike" => "cycling-regular",
              "walk" => "foot-walking",
              "hike" => "foot-hiking",
              _ => "driving-car"
          };

          var url = $"https://api.openrouteservice.org/v2/directions/{profile}?api_key={_apiKey}&start={start.lon},{start.lat}&end={end.lon},{end.lat}";

          Log.Information("[DEBUG ORS] URL-ul generat este: {Url}", url);

          var response = await _httpClient.GetAsync(url);
          response.EnsureSuccessStatusCode();

          var json = await response.Content.ReadAsStringAsync();
          using var doc = JsonDocument.Parse(json);

          var route = doc.RootElement.GetProperty("features")[0].GetProperty("properties").GetProperty("summary");
          var geometry = doc.RootElement.GetProperty("features")[0].GetProperty("geometry").GetProperty("coordinates").ToString();

          double distance = route.GetProperty("distance").GetDouble(); 
          distance=Math.Round(distance/1000.0,1);
          double duration = route.GetProperty("duration").GetDouble(); 
          duration=Math.Round(duration/60.0,1);

          return (distance, duration, geometry); 
      }
  }

