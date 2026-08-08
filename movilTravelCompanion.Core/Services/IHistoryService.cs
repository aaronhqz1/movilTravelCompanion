using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public interface IHistoryService
{
    Task AddAsync(int userId, WeatherData weather);

    Task<List<HistoryEntry>> GetRecentAsync(int userId);
}
