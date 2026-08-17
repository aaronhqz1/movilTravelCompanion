using movilTravelCompanion.Core.Models;

namespace movilTravelCompanion.Core.Services;

public interface IClothingService
{
    Task<ClothingRecommendation> GetRecommendationAsync(
        string city,
        double temperature,
        int weatherCode,
        double humidity,
        double windSpeed,
        string clothingStyle,
        string? coldSensitivity);
}
