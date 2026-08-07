using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public interface IWeatherApiService
{
    Task<WeatherData> GetRandomWeatherAsync();

    Task<WeatherData> SearchWeatherAsync(string city);

    Task<WeatherData> GetWeatherByCoordinatesAsync(double latitude, double longitude);
}
