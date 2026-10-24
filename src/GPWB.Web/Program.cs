using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using GPWB;
using GPWB.Application.Services;
using GPWB.Core.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add basic logging
builder.Logging.AddProvider(new ConsoleLoggerProvider());
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri("https://api.github.com/") };
    client.DefaultRequestHeaders.UserAgent.ParseAdd("GPWB/1.0");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
    client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
    return client;
});
builder.Services.AddScoped<IGitHubApiSession, GitHubApiSession>();
builder.Services.AddScoped<IGitHubProjectService, GitHubProjectService>();

await builder.Build().RunAsync();

// Simple console logger provider for WASM
internal sealed class ConsoleLoggerProvider : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new ConsoleLogger(categoryName);

    public void Dispose() { }

    private sealed class ConsoleLogger(string categoryName) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Information;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;
            var message = formatter(state, exception);
            var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            var level = logLevel switch
            {
                LogLevel.Trace => "trce",
                LogLevel.Debug => "dbug",
                LogLevel.Information => "info",
                LogLevel.Warning => "warn",
                LogLevel.Error => "fail",
                LogLevel.Critical => "crit",
                _ => "info"
            };
            Console.WriteLine($"[{timestamp} {level}] {categoryName}: {message}");
            if (exception != null)
                Console.WriteLine(exception);
        }
    }
}
