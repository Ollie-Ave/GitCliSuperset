using OllieAve.GitCliSuperset.Console.Interfaces;
using OllieAve.GitCliSuperset.Services.Git;
using OllieAve.GitCliSuperset.Services.Jira;
using OllieAve.GitCliSuperset.Services.OpenAi;
using Spectre.Console;

namespace OllieAve.GitCliSuperset.Console.Commands;

public class CommitCommand : ICommitCommand
{
	private readonly IGitService gitService;
	private readonly IJiraService jiraService;
	private readonly OpenAiService openAiService;

	public CommitCommand(
		IGitService gitService,
		IJiraService jiraService,
		OpenAiService openAiService)
	{
		this.gitService = gitService;
		this.jiraService = jiraService;
		this.openAiService = openAiService;
	}

	public async Task Commit()
	{
		await AnsiConsole
			.Status()
			.Start("Thinking...", async ctx =>
			{
				AnsiConsole.MarkupLine("Getting current branch name...");
				var result = await gitService.ExecuteCommand("branch --show-current");

				if (!result.Success)
				{
					AnsiConsole.MarkupLine("[red]Failed to get current branch name[/]");
					return;
				}

				int? jiraNumber = jiraService.ParseJiraNumber(result.Output);

				if (jiraNumber == null)
				{
					AnsiConsole.MarkupLine("[red]Failed to get Jira issue number[/]");
					return;
				}

				AnsiConsole.MarkupLine("Getting Jira issue title...");
				string? jiraTitle = await jiraService.GetJiraIssueTitle(jiraNumber.Value);

				AnsiConsole.MarkupLine("Generating commit message...");
				var gitDiffResult = await gitService.ExecuteCommand("--no-pager diff --staged");

				if (!gitDiffResult.Success)
				{
					AnsiConsole.MarkupLine("[red]Failed to get changes[/]");
					return;
				}

				string commitMessage = await openAiService.GenerateCommitMessage(gitDiffResult.Output);
				string fullCommitMessage = $"{jiraService.GetProjectKey()}-{jiraNumber} - {jiraTitle}\n\n{commitMessage}";

				AnsiConsole.MarkupLine("Committing changes...");
				var commitResult = await gitService.ExecuteCommand($"commit -m \"{commitMessage}\"");

				if (!commitResult.Success)
				{
					AnsiConsole.MarkupLine("[red]Failed to commit changes[/]");
					return;
				}
			});
	}
}