using movilTravelCompanion.Core.Models;
using movilTravelCompanion.Core.Services;

namespace movilTravelCompanion.Services;

// Implementacion concreta de ISessionStore usando Preferences (Microsoft.Maui.Storage),
// el equivalente en MAUI a localStorage en el navegador: pares clave/valor que
// persisten en disco entre aperturas de la app. Preferences solo guarda tipos
// primitivos (string, int, double, bool), por eso el User se descompone en
// varias claves en vez de serializarse entero.
public class PreferencesSessionStore : ISessionStore
{
    private const string UserIdKey = "session_userId";
    private const string UsernameKey = "session_username";
    private const string HomeCityKey = "session_homeCity";
    private const string HomeLatitudeKey = "session_homeLatitude";
    private const string HomeLongitudeKey = "session_homeLongitude";
    private const string TravelDestinationKey = "session_travelDestination";
    private const string DestinationLatitudeKey = "session_destinationLatitude";
    private const string DestinationLongitudeKey = "session_destinationLongitude";

    public void SaveUser(User user)
    {
        Preferences.Default.Set(UserIdKey, user.UserId);
        Preferences.Default.Set(UsernameKey, user.Username);
        SetOrRemove(HomeCityKey, user.HomeCity);
        SetOrRemove(HomeLatitudeKey, user.HomeLatitude);
        SetOrRemove(HomeLongitudeKey, user.HomeLongitude);
        SetOrRemove(TravelDestinationKey, user.TravelDestination);
        SetOrRemove(DestinationLatitudeKey, user.DestinationLatitude);
        SetOrRemove(DestinationLongitudeKey, user.DestinationLongitude);
    }

    public User? GetUser()
    {
        if (!Preferences.Default.ContainsKey(UserIdKey))
        {
            return null;
        }

        return new User
        {
            UserId = Preferences.Default.Get(UserIdKey, 0),
            Username = Preferences.Default.Get(UsernameKey, string.Empty),
            HomeCity = GetStringOrNull(HomeCityKey),
            HomeLatitude = GetDoubleOrNull(HomeLatitudeKey),
            HomeLongitude = GetDoubleOrNull(HomeLongitudeKey),
            TravelDestination = GetStringOrNull(TravelDestinationKey),
            DestinationLatitude = GetDoubleOrNull(DestinationLatitudeKey),
            DestinationLongitude = GetDoubleOrNull(DestinationLongitudeKey)
        };
    }

    public void ClearUser()
    {
        Preferences.Default.Remove(UserIdKey);
        Preferences.Default.Remove(UsernameKey);
        Preferences.Default.Remove(HomeCityKey);
        Preferences.Default.Remove(HomeLatitudeKey);
        Preferences.Default.Remove(HomeLongitudeKey);
        Preferences.Default.Remove(TravelDestinationKey);
        Preferences.Default.Remove(DestinationLatitudeKey);
        Preferences.Default.Remove(DestinationLongitudeKey);
    }

    private static void SetOrRemove(string key, string? value)
    {
        if (value is null)
        {
            Preferences.Default.Remove(key);
        }
        else
        {
            Preferences.Default.Set(key, value);
        }
    }

    private static void SetOrRemove(string key, double? value)
    {
        if (value is null)
        {
            Preferences.Default.Remove(key);
        }
        else
        {
            Preferences.Default.Set(key, value.Value);
        }
    }

    private static string? GetStringOrNull(string key) =>
        Preferences.Default.ContainsKey(key) ? Preferences.Default.Get(key, string.Empty) : null;

    private static double? GetDoubleOrNull(string key) =>
        Preferences.Default.ContainsKey(key) ? Preferences.Default.Get(key, 0d) : null;
}
