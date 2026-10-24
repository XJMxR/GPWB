using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using GPWB.Core.Models;
using GPWB.Core.Interfaces;

namespace GPWB.Application.Services;

public sealed class GitHubApiSession(HttpClient httpClient) : IGitHubApiSession
{
    private readonly object _sync = new();
    private string? _accessToken;
    private string? _login;
    private int _anonymousRequests;
    private int? _remainingRequests;
    private int? _requestLimit;

    public GitHubApiUsage Usage
    {
        get
        {
            lock (_sync)
            {
                return new GitHubApiUsage(
                    _anonymousRequests,
                    _accessToken is not null,
                    _login,
                    _remainingRequests,
                    _requestLimit);
            }
        }
    }

    public async Task<string> ConnectAsync(string token, CancellationToken cancellationToken = default)
    {
        var candidateToken = token.Trim();
        if (candidateToken.Length == 0)
        {
            throw new InvalidOperationException("Introduce un token personal de acceso de GitHub.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, "user");
        using var response = await SendAsync(request, candidateToken, cancellationToken);
        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
        {
            throw new InvalidOperationException(
                "GitHub rechazó el token. Comprueba que sea válido y tenga acceso de solo lectura a repositorios públicos.");
        }

        response.EnsureSuccessStatusCode();
        var viewer = await response.Content.ReadFromJsonAsync<GitHubAccount>(cancellationToken: cancellationToken)
            ?? throw new InvalidOperationException("GitHub devolvió una respuesta de usuario vacía.");

        lock (_sync)
        {
            _accessToken = candidateToken;
            _login = viewer.Login;
        }

        return viewer.Login;
    }

    public void Disconnect()
    {
        lock (_sync)
        {
            _accessToken = null;
            _login = null;
            _remainingRequests = null;
            _requestLimit = null;
        }
    }

    public Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default)
    {
        string? token;
        lock (_sync)
        {
            token = _accessToken;
        }

        return SendAsync(request, token, cancellationToken);
    }

    private async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        string? token,
        CancellationToken cancellationToken)
    {
        if (token is null)
        {
            lock (_sync)
            {
                _anonymousRequests++;
            }
        }
        else
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await httpClient.SendAsync(request, cancellationToken);
        UpdateRateLimit(response);
        return response;
    }

    private void UpdateRateLimit(HttpResponseMessage response)
    {
        int? remaining = ParseRateLimitHeader(response, "X-RateLimit-Remaining");
        int? limit = ParseRateLimitHeader(response, "X-RateLimit-Limit");
        if (remaining is null && limit is null)
        {
            return;
        }

        lock (_sync)
        {
            _remainingRequests = remaining ?? _remainingRequests;
            _requestLimit = limit ?? _requestLimit;
        }
    }

    private static int? ParseRateLimitHeader(HttpResponseMessage response, string name) =>
        response.Headers.TryGetValues(name, out var values) &&
        int.TryParse(values.FirstOrDefault(), out var value)
            ? value
            : null;

}
