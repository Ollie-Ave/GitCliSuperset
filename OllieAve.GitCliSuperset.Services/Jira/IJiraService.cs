namespace OllieAve.GitCliSuperset.Services.Jira;

public interface IJiraService
{
	Task<string?> GetJiraIssueTitle(int jiraNumber);

	int? ParseJiraNumber(string branchName);

	string GetProjectKey();
}