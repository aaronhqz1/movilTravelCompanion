using System.Text.Json.Serialization;

namespace movilTravelCompanion.Core.Models;

public class UserPreferences
{
    [JsonPropertyName("defaultClothingStyle")]
    public string DefaultClothingStyle { get; set; } = "casual";

    [JsonPropertyName("coldSensitivity")]
    public string ColdSensitivity { get; set; } = "normal";
}
