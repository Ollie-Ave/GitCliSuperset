namespace OllieAve.GitCliSuperset.Services.Jira;

public record JiraOptions
{
    public required string JiraUrl { get; set; }
    public required string JiraUser { get; set; }
    public required string JiraToken { get; set; }
    public required string JiraProjectKey { get; set; }
}
