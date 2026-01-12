using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using OllieAve.GitCliSuperset.Console.Commands;
using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Console.Models;
using OllieAve.GitCliSuperset.Services.Git;
using OllieAve.GitCliSuperset.Services.Jira;
using OllieAve.GitCliSuperset.Services.OpenAi;
using Spectre.Console;

namespace OllieAve.GitCliSuperset.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var debug = args.Contains("--debug");

        var appSettings = await LoadAppSettingsAsync(debug);

        if (appSettings is null)
        {
            AnsiConsole.MarkupLine("[red]Failed to load app settings[/]");
            return;
        }

        var serviceProvider = ConfigureServices(appSettings);

        var command = args.FirstOrDefault();

        switch (command)
        {
            case "commit":
                await serviceProvider
                    .GetRequiredService<ICommitCommand>()
                    .Commit(debug);
                break;
            case "checkout":
                await serviceProvider
                    .GetRequiredService<ICheckoutCommand>()
                    .Checkout();
                break;
            default:
                await serviceProvider
                    .GetRequiredService<IPassThroughToGitCommand>()
                    .PassThroughToGit(args);
                break;
        }
    }

    private static async Task<AppSettings?> LoadAppSettingsAsync(bool debug)
    {
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string settingsPath = Path.Join(homePath, ".gitCliSuperset", "settings.json");

        if (debug)
        {
            AnsiConsole.WriteLine($"Loading application settings from path - {settingsPath}");
        }

        var appSettingsRaw = await File.ReadAllTextAsync(settingsPath);

        var appSettings = JsonSerializer.Deserialize<AppSettings>(appSettingsRaw);

        return appSettings;
    }

    private static ServiceProvider ConfigureServices(AppSettings appSettings)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ICommitCommand, CommitCommand>();
        services.AddSingleton<ICheckoutCommand, CheckoutCommand>();
        services.AddSingleton<IPassThroughToGitCommand, PassThroughToGitCommand>();

        services.AddSingleton<IGitService, GitService>();
        services.AddSingleton<IJiraService, JiraService>();
        services.AddSingleton<OpenAiService, OpenAiService>();

        services.Configure<JiraOptions>(x =>
        {
            x.JiraUrl = appSettings.JiraUrl;
            x.JiraUser = appSettings.JiraUser;
            x.JiraToken = appSettings.JiraToken;
        });

        services.Configure<OpenAiOptions>(x =>
        {
            x.ApiKey = appSettings.OpenAiApiKey;
            x.Model = appSettings.OpenAiModel;
        });

        return services.BuildServiceProvider();
    }
}
