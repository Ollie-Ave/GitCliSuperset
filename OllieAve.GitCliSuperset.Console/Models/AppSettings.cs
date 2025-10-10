namespace OllieAve.GitCliSuperset.Console.Models;

public record AppSettings
{
	public required string JiraUrl { get; init; }
	public required string JiraUser { get; init; }
	public required string JiraToken { get; init; }
	public required string OpenAiApiKey { get; init; }
	public required string OpenAiModel { get; init; }
}