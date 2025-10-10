namespace OllieAve.GitCliSuperset.Services.Jira;

public record JiraOptions
{
	public required string JiraUrl { get; init; }
	public required string JiraUser { get; init; }
	public required string JiraToken { get; init; }
	public required string JiraProjectKey { get; init; }
}