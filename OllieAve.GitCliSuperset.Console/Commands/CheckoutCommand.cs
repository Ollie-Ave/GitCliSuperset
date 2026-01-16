using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Services.Git;
using OllieAve.GitCliSuperset.Services.Jira;
using Spectre.Console;

namespace OllieAve.GitCliSuperset.Console.Commands;

public class CheckoutCommand : ICheckoutCommand
{
    private readonly IGitService gitService;
    private readonly IJiraService jiraService;

    public CheckoutCommand(
        IGitService gitService,
         IJiraService jiraService)
    {
        this.gitService = gitService;
        this.jiraService = jiraService;
    }

    public async void Checkout()
    {
        var jira = AnsiConsole.Prompt(
            new TextPrompt<string>($"Enter the Jira Ref (Without {jiraService.GetProjectKey()}):"));

        gitService.ExecuteCommand("fetch --prune origin");
        var originBranches = GetRemoteOriginBranches();

        var originBranch = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]origin branch[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
                .AddChoices(originBranches));


        var hadLocalChanges = gitService.HasLocalChangesIncludingUntracked();

        if (hadLocalChanges.Success == false)
        {
            AnsiConsole.MarkupLine($"[red]Failed to check git status:[/] {Markup.Escape(hadLocalChanges.Error)}");

            return;
        }

        if (hadLocalChanges.Result.Value)
        {
            gitService.ExecuteCommand("stash push --include-untracked");
        }

        string newBranchName = $"{originBranch.Replace("origin/", "")}-{jiraService.GetProjectKey()}-{jira}";
        gitService.ExecuteCommand($"checkout -b {newBranchName} {originBranch}");

        gitService.ExecuteCommand("branch --unset-upstream");

        if (hadLocalChanges.Result.Value)
        {
            gitService.ExecuteCommand("stash pop");
        }
    }

    private List<string> GetRemoteOriginBranches()
    {
        var output = gitService.ExecuteCommand("branch -r");

        var branches = output.Output
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.StartsWith("remotes/") ? l["remotes/".Length..] : l)
            .Where(l => l.StartsWith("origin/"))
            .Where(l => !l.Contains("->"))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (branches.Count == 0 && !string.IsNullOrWhiteSpace(output.Error))
        {
            AnsiConsole.MarkupLine($"[red]Failed to list remote branches:[/] {Markup.Escape(output.Error)}");
        }

        return branches;
    }
}

