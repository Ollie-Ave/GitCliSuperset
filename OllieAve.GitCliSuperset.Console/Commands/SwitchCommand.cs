using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Services.Git;
using Spectre.Console;

namespace OllieAve.GitCliSuperset.Console.Commands;

public class SwitchCommand : ISwitchCommand
{
    private readonly IGitService gitService;

    public SwitchCommand(IGitService gitService)
    {
        this.gitService = gitService;
    }

    public async Task Switch()
    {
        gitService.ExecuteCommand("fetch --prune origin");
        var localBranches = GetLocalBranches();

        var branchToSwitchTo = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]branch to switch to.[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
                .AddChoices(localBranches));


        var hadLocalChanges = await HasLocalChangesIncludingUntracked();
        if (hadLocalChanges)
        {
            gitService.ExecuteCommand("stash push --include-untracked");
        }

        gitService.ExecuteCommand($"checkout {branchToSwitchTo}");

        if (hadLocalChanges)
        {
            gitService.ExecuteCommand("stash pop");
        }
    }

    private List<string> GetLocalBranches()
    {
        var output = gitService.ExecuteCommand("branch --list");

        var branches = output.Output
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !l.TrimStart().StartsWith('*'))
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (branches.Count == 0 && !string.IsNullOrWhiteSpace(output.Error))
        {
            AnsiConsole.MarkupLine(
                $"[red]Failed to list local branches:[/] {Markup.Escape(output.Error)}");
        }

        return branches;
    }

    private async Task<bool> HasLocalChangesIncludingUntracked()
    {
        var output = gitService.ExecuteCommand("status --porcelain");

        if (!output.Success)
        {
            AnsiConsole.MarkupLine($"[red]Failed to check git status:[/] {Markup.Escape(output.Error)}");
            return false;
        }

        return output.Output
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Length != 0;
    }
}
