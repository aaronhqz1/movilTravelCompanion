using System.Net.Http.Json;
using System.Text.Json.Serialization;
using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<int> RegisterAsync(string username, string password, string? homeCity)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/auth/register", new { username, password, homeCity });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudo registrar el usuario '{username}'. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        return result?.UserId ?? throw new HttpRequestException(
            "La respuesta del servidor al registrar el usuario vino vacía.");
    }

    public async Task<User> LoginAsync(string username, string password)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "/api/auth/login", new { username, password });

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
                $"No se pudo iniciar sesión con el usuario '{username}'. Código HTTP: {(int)response.StatusCode} {response.StatusCode}. {errorBody}");
        }

        var user = await response.Content.ReadFromJsonAsync<User>();
        return user ?? throw new HttpRequestException(
            "La respuesta del servidor al iniciar sesión vino vacía.");
    }

    private sealed class RegisterResponse
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
    }
}
