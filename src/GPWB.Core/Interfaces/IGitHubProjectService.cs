using GPWB.Core.Models;

namespace GPWB.Core.Interfaces;

public interface IGitHubProjectService
{
    Task<GitHubProjectData> GetProjectAsync(
        string owner,
        string repository,
        CancellationToken cancellationToken = default);
}
