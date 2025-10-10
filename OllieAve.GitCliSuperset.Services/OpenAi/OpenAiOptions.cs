namespace OllieAve.GitCliSuperset.Services.OpenAi;

public record OpenAiOptions
{
	public required string ApiKey { get; init; }
	public required string Model { get; init; }
}