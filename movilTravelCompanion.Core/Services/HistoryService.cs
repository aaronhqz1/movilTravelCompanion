using System.Net.Http.Json;
using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public class HistoryService : IHistoryService
{
    private readonly HttpClient _httpClient;

    public HistoryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task AddAsync(int userId, WeatherData weather)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/history", new
        {
            userId,
            city = weather.City,
            latitude = weather.Latitude,
            longitude = weather.Longitude,
            temperature = weather.Temperature,
            humidity = weather.Humidity,
            wind_speed = weather.WindSpeed,
            weather_code = weather.WeatherCode
        });

        if (!response.IsSuccessStatusCode)
        {
            // El backend valida la regla de "misma ciudad dentro de 24hs" y devuelve
            // un error para ese caso; lo mostramos tal cual en vez de duplicar la regla aca.
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudo guardar '{weather.City}' en el historial. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }
    }

    public async Task<List<HistoryEntry>> GetRecentAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/history/{userId}/recent");

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudo obtener el historial reciente. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var history = await response.Content.ReadFromJsonAsync<List<HistoryEntry>>();
        return history ?? [];
    }
}
