using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Services.Git;
using Spectre.Console;

namespace OllieAve.GitCliSuperset.Console.Commands;

public class PassThroughToGitCommand : IPassThroughToGitCommand
{
    private readonly IGitService gitService;

    public PassThroughToGitCommand(IGitService gitService)
    {
        this.gitService = gitService;
    }

    public void PassThroughToGit(string[] args)
    {
        var result = gitService.ExecuteCommand(string.Join(" ", args));

        AnsiConsole.WriteLine(result.Output);
    }
}
