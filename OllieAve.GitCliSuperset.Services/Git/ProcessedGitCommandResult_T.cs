using System.Diagnostics.CodeAnalysis;

namespace OllieAve.GitCliSuperset.Services.Git;

public record GitCommandResult<T>
{
    public required T? Result { get; init; }

    [MemberNotNullWhen(true, nameof(Result))]
    [MemberNotNullWhen(false, nameof(Error))]
    public required bool Success { get; init; }

    public required string? Error { get; init; }
};
