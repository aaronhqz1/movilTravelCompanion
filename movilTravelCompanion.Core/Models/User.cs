using System.Text.Json.Serialization;

namespace movilTravelCompanion.Core.Models;

public class User
{
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("homeCity")]
    public string? HomeCity { get; set; }

    [JsonPropertyName("homeLatitude")]
    public double? HomeLatitude { get; set; }

    [JsonPropertyName("homeLongitude")]
    public double? HomeLongitude { get; set; }

    [JsonPropertyName("travelDestination")]
    public string? TravelDestination { get; set; }

    [JsonPropertyName("destinationLatitude")]
    public double? DestinationLatitude { get; set; }

    [JsonPropertyName("destinationLongitude")]
    public double? DestinationLongitude { get; set; }
}
