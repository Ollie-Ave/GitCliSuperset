using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Services.Git;
using Spectre.Console;

namespace OllieAve.GitCliSuperset.Console.Commands;

public class BranchDeleteCommand : IBranchDeleteCommand
{
    private const string AllOption = "[yellow]all[/] (ignores other selections)";
    private readonly IGitService gitService;

    public BranchDeleteCommand(IGitService gitService)
    {
        this.gitService = gitService;
    }

    public void BranchDelete()
    {
        gitService.ExecuteCommand("fetch --prune origin");

        var localBranches = gitService.GetLocalBranches();

        if (localBranches.Success == false)
        {
            AnsiConsole.MarkupLine(
                $"[red]Failed to list local branches:[/] {Markup.Escape(localBranches.Error)}");

            return;
        }

        if (localBranches.Result.Count == 0)
        {
            AnsiConsole.MarkupLine("There are no local branches except your current working branch");
            return;
        }

        var branchesToDelete = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select the [red]branches to delete.[/]")
                .PageSize(10)
                .MoreChoicesText("[grey](Move up and down to reveal more branches)[/]")
                .AddChoices([AllOption, .. localBranches.Result]));

        if (branchesToDelete.Contains(AllOption))
        {
            branchesToDelete = localBranches.Result;
        }

        gitService.ExecuteCommand($"branch -D {string.Join(" ", branchesToDelete)}");


        AnsiConsole.MarkupLine("[red]Deleted selected branches.[/]");
        AnsiConsole.MarkupLine(string.Join(",\n", branchesToDelete));
    }
}
