using GPWB.Models;

namespace GPWB.Services;

public interface IGitHubProjectService
{
    Task<GitHubProjectData> GetProjectAsync(string owner, string repository, CancellationToken cancellationToken = default);
}
