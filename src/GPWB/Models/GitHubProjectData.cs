using System.Text.Json.Serialization;

namespace GPWB.Models;

public sealed record GitHubProjectData(
    GitHubRepository Repository,
    IReadOnlyDictionary<string, long> Languages,
    IReadOnlyList<GitHubIssue> Issues,
    IReadOnlyList<GitHubCommit> Commits);

public sealed record GitHubRepository(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("full_name")]
    string FullName,
    [property: JsonPropertyName("html_url")]
    string HtmlUrl,
    [property: JsonPropertyName("description")]
    string? Description,
    [property: JsonPropertyName("default_branch")]
    string? DefaultBranch,
    [property: JsonPropertyName("language")]
    string? Language,
    [property: JsonPropertyName("stargazers_count")]
    int StargazersCount,
    [property: JsonPropertyName("forks_count")]
    int ForksCount,
    [property: JsonPropertyName("open_issues_count")]
    int OpenIssuesCount,
    [property: JsonPropertyName("updated_at")]
    DateTimeOffset UpdatedAt);

public sealed record GitHubIssue(
    [property: JsonPropertyName("number")]
    int Number,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("html_url")]
    string HtmlUrl,
    [property: JsonPropertyName("state")]
    string State,
    [property: JsonPropertyName("updated_at")]
    DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("pull_request")]
    GitHubPullRequest? PullRequest);

public sealed record GitHubPullRequest([property: JsonPropertyName("html_url")] string HtmlUrl);

public sealed record GitHubCommit(
    [property: JsonPropertyName("sha")] string Sha,
    [property: JsonPropertyName("commit")] GitHubCommitDetails Commit);

public sealed record GitHubCommitDetails(
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("author")] GitHubCommitAuthor? Author);

public sealed record GitHubCommitAuthor(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("date")] DateTimeOffset Date);
