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

    public void Commit(bool debug)
    {
        if (debug)
        {
            var options = openAiService.GetOptions();

            AnsiConsole.WriteLine("Committing changes with configuration:");
            AnsiConsole.WriteLine($"{nameof(options.ApiKey)} - {options.ApiKey}");
            AnsiConsole.WriteLine($"{nameof(options.Model)} - {options.Model}");
        }

        AnsiConsole
            .Status()
            .Start("Thinking...", ctx =>
            {
                AnsiConsole.MarkupLine("Getting current branch name...");
                var result = gitService.ExecuteCommand("branch --show-current");

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
                string? jiraTitle = jiraService.GetJiraIssueTitle(jiraNumber.Value);

                AnsiConsole.MarkupLine("Generating commit message...");
                var gitDiffResult = gitService.ExecuteCommand("--no-pager diff --staged");

                if (!gitDiffResult.Success)
                {
                    AnsiConsole.MarkupLine("[red]Failed to get changes[/]");
                    return;
                }

                if (gitDiffResult.Output.Length == 0)
                {
                    AnsiConsole.MarkupLine("[red]No staged changes to commit[/]");
                    return;
                }

                string commitMessage = openAiService.GenerateCommitMessage(gitDiffResult.Output);
                string fullCommitMessage = $"{jiraService.GetProjectKey()}-{jiraNumber} - {jiraTitle}\n\n{commitMessage}";

                var commitResult = gitService.ExecuteCommand($"commit -m \"{fullCommitMessage}\"");

                if (!commitResult.Success)
                {
                    AnsiConsole.MarkupLine("[red]Failed to commit changes[/]");

                    AnsiConsole.MarkupLine("[red]Error:[/]");
                    AnsiConsole.WriteLine(commitResult.Error);

                    return;
                }
            });
    }
}
