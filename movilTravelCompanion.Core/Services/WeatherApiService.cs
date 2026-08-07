using System.Globalization;
using System.Net.Http.Json;
using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public class WeatherApiService : IWeatherApiService
{
    private readonly HttpClient _httpClient;

    public WeatherApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData> GetRandomWeatherAsync()
    {
        var response = await _httpClient.GetAsync("/api/weather/random");
        return await ReadWeatherDataAsync(response, "obtener el clima aleatorio");
    }

    public async Task<WeatherData> SearchWeatherAsync(string city)
    {
        var url = $"/api/weather/search?city={Uri.EscapeDataString(city)}";
        var response = await _httpClient.GetAsync(url);
        return await ReadWeatherDataAsync(response, $"buscar el clima de '{city}'");
    }

    public async Task<WeatherData> GetWeatherByCoordinatesAsync(double latitude, double longitude)
    {
        var lat = latitude.ToString(CultureInfo.InvariantCulture);
        var lon = longitude.ToString(CultureInfo.InvariantCulture);
        var url = $"/api/weather/coordinates?lat={lat}&lon={lon}";
        var response = await _httpClient.GetAsync(url);
        return await ReadWeatherDataAsync(response, $"obtener el clima para ({lat}, {lon})");
    }

    private static async Task<WeatherData> ReadWeatherDataAsync(HttpResponseMessage response, string operationDescription)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudo {operationDescription}. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var weatherData = await response.Content.ReadFromJsonAsync<WeatherData>();
        return weatherData ?? throw new HttpRequestException(
            $"La respuesta del servidor al intentar {operationDescription} vino vacía.");
    }
}
