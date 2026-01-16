using System.Collections;
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
    public static void Main(string[] args)
    {
        var debug = args.Contains("--debug");

        var appSettings = LoadAppSettingsAsync(debug);

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
                serviceProvider
                    .GetRequiredService<ICommitCommand>()
                    .Commit(debug);
                break;
            case "checkout":
                serviceProvider
                    .GetRequiredService<ICheckoutCommand>()
                    .Checkout();
                break;
            case "switch":
                serviceProvider
                    .GetRequiredService<ISwitchCommand>()
                    .Switch();
                break;
            case "branch":
                if (args.Contains("delete")
                    || args.Contains("-D")
                    || args.Contains("-d"))
                {
                    serviceProvider.GetRequiredService<IBranchDeleteCommand>()
                        .BranchDelete();
                }
                else
                {
                    serviceProvider
                        .GetRequiredService<IPassThroughToGitCommand>()
                        .PassThroughToGit(args);
                }
                break;
            default:
                serviceProvider
                    .GetRequiredService<IPassThroughToGitCommand>()
                    .PassThroughToGit(args);
                break;
        }
    }

    private static AppSettings LoadAppSettingsAsync(bool debug)
    {
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string settingsPath = Path.Join(homePath, ".gitCliSuperset", "settings.json");

        if (debug)
        {
            AnsiConsole.WriteLine($"Loading application settings from path - {settingsPath}");
        }

        var appSettingsRaw = File.ReadAllText(settingsPath);

        var appSettings = JsonSerializer.Deserialize<AppSettings>(appSettingsRaw);

        return appSettings;
    }

    private static ServiceProvider ConfigureServices(AppSettings appSettings)
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<ICommitCommand, CommitCommand>();
        services.AddSingleton<ICheckoutCommand, CheckoutCommand>();
        services.AddSingleton<ISwitchCommand, SwitchCommand>();
        services.AddSingleton<IBranchDeleteCommand, BranchDeleteCommand>();
        services.AddSingleton<IPassThroughToGitCommand, PassThroughToGitCommand>();

        services.AddSingleton<IGitService, GitService>();
        services.AddSingleton<IJiraService, JiraService>();
        services.AddSingleton<OpenAiService, OpenAiService>();

        services.Configure<JiraOptions>(x =>
        {
            x.JiraUrl = appSettings.JiraUrl;
            x.JiraUser = appSettings.JiraUser;
            x.JiraToken = appSettings.JiraToken;
            x.JiraProjectKey = appSettings.JiraProjectKey;
        });

        services.Configure<OpenAiOptions>(x =>
        {
            x.ApiKey = appSettings.OpenAiApiKey;
            x.Model = appSettings.OpenAiModel;
        });

        return services.BuildServiceProvider();
    }
}
