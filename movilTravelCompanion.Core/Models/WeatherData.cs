using System.Text.Json.Serialization;

namespace movilTravelCompanion.Core.Models;

public class WeatherData
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("humidity")]
    public double Humidity { get; set; }

    [JsonPropertyName("wind_speed")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("hourly_forecast")]
    public List<HourlyForecast> HourlyForecast { get; set; } = [];
}
