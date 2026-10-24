using GPWB.Core.Models;

namespace GPWB.Core.Interfaces;

public interface IGitHubApiSession
{
    GitHubApiUsage Usage { get; }

    Task<string> ConnectAsync(string token, CancellationToken cancellationToken = default);

    void Disconnect();

    Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default);
}
