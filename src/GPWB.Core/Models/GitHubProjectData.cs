using System.Text.Json.Serialization;

namespace GPWB.Core.Models;

public sealed record GitHubProjectData(
    GitHubRepository Repository,
    IReadOnlyDictionary<string, long> Languages,
    IReadOnlyList<GitHubIssue> Issues,
    IReadOnlyList<GitHubCommit> Commits,
    IReadOnlyList<GitHubRepository> Forks,
    IReadOnlyList<GitHubBranchActivity> Branches,
    int ForksReturned,
    int IssuesReturned,
    int BranchesReturned);

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
    [property: JsonPropertyName("owner")]
    GitHubOwner? Owner,
    [property: JsonPropertyName("stargazers_count")]
    int StargazersCount,
    [property: JsonPropertyName("forks_count")]
    int ForksCount,
    [property: JsonPropertyName("open_issues_count")]
    int OpenIssuesCount,
    [property: JsonPropertyName("private")]
    bool IsPrivate,
    [property: JsonPropertyName("updated_at")]
    DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("pushed_at")]
    DateTimeOffset? PushedAt);

public sealed record GitHubOwner(
    [property: JsonPropertyName("login")]
    string Login,
    [property: JsonPropertyName("avatar_url")]
    string AvatarUrl);

public sealed record GitHubBranch(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("commit")]
    GitHubBranchHead Commit,
    [property: JsonPropertyName("protected")]
    bool Protected);

public sealed record GitHubBranchHead(
    [property: JsonPropertyName("sha")]
    string Sha);

public sealed record GitHubBranchActivity(
    string Name,
    bool Protected,
    IReadOnlyList<GitHubCommit> Commits);

public sealed record GitHubIssue(
    [property: JsonPropertyName("number")]
    int Number,
    [property: JsonPropertyName("title")]
    string Title,
    [property: JsonPropertyName("html_url")]
    string HtmlUrl,
    [property: JsonPropertyName("state")]
    string State,
    [property: JsonPropertyName("body")]
    string? Body,
    [property: JsonPropertyName("created_at")]
    DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updated_at")]
    DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("comments")]
    int Comments,
    [property: JsonPropertyName("user")]
    GitHubOwner? User,
    [property: JsonPropertyName("labels")]
    IReadOnlyList<GitHubLabel> Labels,
    [property: JsonPropertyName("pull_request")]
    GitHubPullRequest? PullRequest);

public sealed record GitHubLabel(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("color")]
    string Color);

public sealed record GitHubPullRequest([property: JsonPropertyName("html_url")] string HtmlUrl);

public sealed record GitHubCommit(
    [property: JsonPropertyName("sha")] string Sha,
    [property: JsonPropertyName("commit")] GitHubCommitDetails Commit,
    [property: JsonPropertyName("html_url")] string HtmlUrl,
    [property: JsonPropertyName("parents")] IReadOnlyList<GitHubCommitParent> Parents);

public sealed record GitHubCommitParent(
    [property: JsonPropertyName("sha")] string Sha,
    [property: JsonPropertyName("html_url")] string HtmlUrl);

public sealed record GitHubCommitDetails(
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("author")] GitHubCommitAuthor? Author);

public sealed record GitHubCommitAuthor(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("date")] DateTimeOffset? Date);
