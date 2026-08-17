using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public interface IPreferencesService
{
    Task<UserPreferences> GetPreferencesAsync(int userId);

    Task<UserPreferences> UpdatePreferencesAsync(int userId, UserPreferences preferences);
}
