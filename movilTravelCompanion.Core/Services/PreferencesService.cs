using System.Net.Http.Json;
using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public class PreferencesService : IPreferencesService
{
    private readonly HttpClient _httpClient;

    public PreferencesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserPreferences> GetPreferencesAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"/api/user/{userId}/preferences");

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudieron obtener las preferencias. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var preferences = await response.Content.ReadFromJsonAsync<UserPreferences>();
        return preferences ?? new UserPreferences();
    }

    public async Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences)
    {
        var response = await _httpClient.PutAsJsonAsync($"/api/user/{userId}/preferences", preferences);

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudieron guardar las preferencias. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var updated = await response.Content.ReadFromJsonAsync<UserPreferences>();
        return updated ?? preferences;
    }
}
