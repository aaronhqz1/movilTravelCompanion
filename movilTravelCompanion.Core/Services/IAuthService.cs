using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public interface IAuthService
{
    Task<int> RegisterAsync(string username, string password, string? homeCity);

    Task<User> LoginAsync(string username, string password);
}
