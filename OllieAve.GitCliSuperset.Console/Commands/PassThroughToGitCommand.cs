using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Services.Git;

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
        var result = gitService.ExecuteCommand(string.Join(" ", args), true);

        if (result.Success)
        {
            System.Console.WriteLine(result.Output);

            return;
        }

        System.Console.WriteLine(result.Error);
    }
}
