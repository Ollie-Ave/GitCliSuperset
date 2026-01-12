namespace OllieAve.GitCliSuperset.Services.OpenAi;

public record OpenAiOptions
{
    public required string ApiKey { get; set; }
    public required string Model { get; set; }
}
