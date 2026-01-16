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

    public void Switch()
    {
        gitService.ExecuteCommand("fetch --prune origin");
        var localBranches = gitService.GetLocalBranches();

        if (localBranches.Success == false)
        {
            AnsiConsole.MarkupLine(
                $"[red]Failed to list local branches:[/] {Markup.Escape(localBranches.Error)}");

            return;
        }

        var branchToSwitchTo = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select the [green]branch to switch to.[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
                .AddChoices(localBranches.Result));


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

        gitService.ExecuteCommand($"checkout {branchToSwitchTo}");

        if (hadLocalChanges.Result.Value)
        {
            gitService.ExecuteCommand("stash pop");
        }
    }
}
