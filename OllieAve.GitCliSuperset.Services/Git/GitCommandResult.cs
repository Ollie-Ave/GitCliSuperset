using System.Diagnostics.CodeAnalysis;

namespace OllieAve.GitCliSuperset.Services.Git;

public record GitCommandResult
{
	public required bool Success { get; init; }

	public required string Output { get; init; }

	public required string Error { get; init; }
};