namespace OllieAve.GitCliSuperset.Services.Jira;

public interface IJiraService
{
    string? GetJiraIssueTitle(int jiraNumber);

    int? ParseJiraNumber(string branchName);

    string GetProjectKey();
}
