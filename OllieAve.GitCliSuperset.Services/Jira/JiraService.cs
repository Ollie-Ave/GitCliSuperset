using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;

namespace OllieAve.GitCliSuperset.Services.Jira;

public partial class JiraService : IJiraService
{
	private readonly IOptions<JiraOptions> options;

	public JiraService(IOptions<JiraOptions> options)
	{
		this.options = options;
	}

	public async Task<string?> GetJiraIssueTitle(int jiraNumber)
	{
		try
		{
			var jira = Atlassian.Jira.Jira.CreateRestClient(
				options.Value.JiraUrl,
				options.Value.JiraUser,
				options.Value.JiraToken);

			string issueKey = $"{options.Value.JiraProjectKey}-{jiraNumber}";

			var issue = await jira.Issues.GetIssueAsync(issueKey);

			return issue?.Summary;
		}
		catch (Exception)
		{
			return null;
		}
	}
	public int? ParseJiraNumber(string branchName)
	{
		if (string.IsNullOrWhiteSpace(branchName))
		{
			return null;
		}

		var regex = @"(?i)(?:^|-)" + options.Value.JiraProjectKey + @"(\d+)(?:-|$)";
		var match = Regex.Match(branchName, regex);

		if (!match.Success)
		{
			return null;
		}

		_ = int.TryParse(match.Groups[1].Value, out var jiraNumber);
		return jiraNumber;
	}

	public string GetProjectKey()
	{
		return options.Value.JiraProjectKey;
	}
}
