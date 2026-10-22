using System.Net;
using System.Net.Http.Json;
using GPWB.Models;

namespace GPWB.Services;

public sealed class GitHubProjectService(IGitHubApiSession apiSession) : IGitHubProjectService
{
    public async Task<GitHubProjectData> GetProjectAsync(
        string owner,
        string repository,
        CancellationToken cancellationToken = default)
    {
        var path = $"repos/{Uri.EscapeDataString(owner)}/{Uri.EscapeDataString(repository)}";
        var repositoryData = await GetRequiredAsync<GitHubRepository>(path, cancellationToken);
        if (repositoryData.IsPrivate)
        {
            throw new InvalidOperationException(
                "RepoCanvas solo muestra información de repositorios públicos. El repositorio indicado es privado.");
        }

        var languagesTask = GetRequiredAsync<Dictionary<string, long>>($"{path}/languages", cancellationToken);
        var issuesTask = GetRequiredAsync<List<GitHubIssue>>(
            $"{path}/issues?state=open&sort=updated&direction=desc&per_page=100",
            cancellationToken);
        var commitsTask = GetListOrEmptyOnConflictAsync<GitHubCommit>(
            $"{path}/commits?per_page=30",
            cancellationToken);
        var forksTask = GetRequiredAsync<List<GitHubRepository>>(
            $"{path}/forks?sort=newest&per_page=100",
            cancellationToken);
        var branchesTask = GetListOrEmptyOnConflictAsync<GitHubBranch>(
            $"{path}/branches?per_page=100",
            cancellationToken);

        await Task.WhenAll(languagesTask, issuesTask, commitsTask, forksTask, branchesTask);

        var fetchedIssues = await issuesTask;
        var issues = fetchedIssues
            .Where(issue => issue.PullRequest is null)
            .ToArray();
        var branches = await branchesTask;
        if (!string.IsNullOrWhiteSpace(repositoryData.DefaultBranch) &&
            branches.All(branch => !branch.Name.Equals(repositoryData.DefaultBranch, StringComparison.OrdinalIgnoreCase)))
        {
            var defaultBranch = await GetRequiredAsync<GitHubBranch>(
                $"{path}/branches/{Uri.EscapeDataString(repositoryData.DefaultBranch)}",
                cancellationToken);
            branches.Add(defaultBranch);
        }

        var selectedBranches = branches
            .OrderByDescending(branch => branch.Name.Equals(repositoryData.DefaultBranch, StringComparison.OrdinalIgnoreCase))
            .ThenBy(branch => branch.Name, StringComparer.OrdinalIgnoreCase)
            .Take(6)
            .ToArray();
        var branchActivities = selectedBranches
            .Select(async branch =>
            {
                var commits = await GetListOrEmptyOnConflictAsync<GitHubCommit>(
                    $"{path}/commits?sha={Uri.EscapeDataString(branch.Name)}&per_page=10",
                    cancellationToken);
                return new GitHubBranchActivity(branch.Name, branch.Protected, commits);
            });
        var branchResults = (await Task.WhenAll(branchActivities))
            .OrderByDescending(branch => branch.Name.Equals(repositoryData.DefaultBranch, StringComparison.OrdinalIgnoreCase))
            .ThenByDescending(branch => branch.Commits.FirstOrDefault()?.Commit.Author?.Date)
            .ToArray();

        return new GitHubProjectData(
            repositoryData,
            await languagesTask,
            issues,
            await commitsTask,
            (await forksTask).Take(30).ToArray(),
            branchResults,
            Math.Min((await forksTask).Count, 100),
            issues.Length,
            branches.Count);
    }

    private async Task<T> GetRequiredAsync<T>(string path, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        using var response = await apiSession.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException($"GitHub devolvió una respuesta vacía para {path}.");
    }

    private async Task<List<T>> GetListOrEmptyOnConflictAsync<T>(string path, CancellationToken cancellationToken)
    {
        try
        {
            return await GetRequiredAsync<List<T>>(path, cancellationToken);
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            return [];
        }
    }
}
