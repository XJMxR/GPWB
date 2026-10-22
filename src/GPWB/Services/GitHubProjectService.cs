using System.Net.Http.Json;
using GPWB.Models;

namespace GPWB.Services;

public sealed class GitHubProjectService(HttpClient httpClient) : IGitHubProjectService
{
    public async Task<GitHubProjectData> GetProjectAsync(
        string owner,
        string repository,
        CancellationToken cancellationToken = default)
    {
        var path = $"repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repository)}";
        var repositoryTask = GetRequiredAsync<GitHubRepository>(path, cancellationToken);
        var languagesTask = GetRequiredAsync<Dictionary<string, long>>($"{path}/languages", cancellationToken);
        var issuesTask = GetRequiredAsync<List<GitHubIssue>>(
            $"{path}/issues?state=open&sort=updated&direction=desc&per_page=20",
            cancellationToken);
        var commitsTask = GetRequiredAsync<List<GitHubCommit>>(
            $"{path}/commits?per_page=5",
            cancellationToken);

        await Task.WhenAll(repositoryTask, languagesTask, issuesTask, commitsTask);

        var issues = (await issuesTask)
            .Where(issue => issue.PullRequest is null)
            .Take(5)
            .ToArray();

        return new GitHubProjectData(
            await repositoryTask,
            await languagesTask,
            issues,
            await commitsTask);
    }

    private async Task<T> GetRequiredAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var response = await httpClient.GetAsync(path, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"GitHub devolvió una respuesta vacía para {path}.");
    }
}
