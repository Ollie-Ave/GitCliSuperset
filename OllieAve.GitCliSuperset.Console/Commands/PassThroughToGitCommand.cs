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

	public async Task PassThroughToGit(string[] args)
	{
		var result = await gitService.ExecuteCommand(string.Join(" ", args));

		AnsiConsole.WriteLine(result.Output);
	}
}
