namespace GPWB.Core.Models;

public sealed record GitHubApiUsage(
    int AnonymousRequests,
    bool IsAuthenticated,
    string? Login,
    int? RemainingRequests,
    int? RequestLimit);

public sealed record GitHubAccount(
    [property: System.Text.Json.Serialization.JsonPropertyName("login")]
    string Login);
