using System.Text.Json.Serialization;

namespace movilTravelCompanion.Core.Models;

public class ClothingRecommendation
{
    [JsonPropertyName("city")]
    public string? City { get; set; }

    [JsonPropertyName("clothingStyle")]
    public string ClothingStyle { get; set; } = string.Empty;

    [JsonPropertyName("coldSensitivity")]
    public string ColdSensitivity { get; set; } = "normal";

    [JsonPropertyName("recommendation")]
    public string Recommendation { get; set; } = string.Empty;
}
