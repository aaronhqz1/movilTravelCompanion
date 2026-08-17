using System.Net.Http.Json;
using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public class ClothingService : IClothingService
{
    private readonly HttpClient _httpClient;

    public ClothingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ClothingRecommendation> GetRecommendationAsync(
        string city,
        double temperature,
        int weatherCode,
        double humidity,
        double windSpeed,
        string clothingStyle,
        string? coldSensitivity)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/ai/clothing-recommendation", new
        {
            city,
            temperature,
            weatherCode,
            humidity,
            windSpeed,
            clothingStyle,
            coldSensitivity
        });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudo obtener la recomendación de vestimenta. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var recommendation = await response.Content.ReadFromJsonAsync<ClothingRecommendation>();
        return recommendation ?? throw new HttpRequestException(
            "La respuesta del servidor al pedir la recomendación de vestimenta vino vacía.");
    }
}
