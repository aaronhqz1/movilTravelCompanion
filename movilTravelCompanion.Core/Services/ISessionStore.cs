using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

// Abstraccion sobre "donde se guarda la sesion". El detalle de storage (Preferences,
// SecureStorage, lo que sea) es una API especifica de plataforma (Microsoft.Maui.Essentials),
// asi que vive en el proyecto movilTravelCompanion (cabeza de la app), no aca en Core.
// Core solo conoce esta interfaz, igual que conoce IAuthService sin saber que hay un
// HttpClient del otro lado.
public interface ISessionStore
{
    void SaveUser(User user);

    User? GetUser();

    void ClearUser();
}
