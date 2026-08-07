using System.Text.Json.Serialization;

namespace movilTravelCompanion.Core.Models;

public class HourlyForecast
{
    [JsonPropertyName("time")]
    public string Time { get; set; } = string.Empty;

    [JsonPropertyName("temperature")]
    public double Temperature { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}
