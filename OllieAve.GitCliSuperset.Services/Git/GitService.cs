using System.Diagnostics;

namespace OllieAve.GitCliSuperset.Services.Git;

public class GitService : IGitService
{
    public GitCommandResult ExecuteCommand(string commandWithArgs)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = commandWithArgs,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process
        {
            StartInfo = startInfo
        };

        process.Start();

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();

        process.WaitForExit();

        return new GitCommandResult
        {
            Success = process.ExitCode == 0,
            Output = stdout,
            Error = stderr
        };
    }

    public GitCommandResult<List<string>> GetLocalBranches()
    {
        var output = ExecuteCommand("branch --list");

        var branches = output.Output
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !l.TrimStart().StartsWith('*'))
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(l => l, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (branches.Count == 0 && !string.IsNullOrWhiteSpace(output.Error))
        {
            return new()
            {
                Error = output.Error,
                Success = false,
                Result = null,
            };
        }

        return new()
        {
            Success = true,
            Error = null,
            Result = branches,
        };
    }

    public GitCommandResult<bool?> HasLocalChangesIncludingUntracked()
    {
        var output = ExecuteCommand("status --porcelain");

        if (!output.Success)
        {
            return new()
            {
                Success = false,
                Error = output.Error,
                Result = null,
            };
        }

        bool result = output.Output
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Length != 0;

        return new()
        {
            Success = true,
            Result = result,
            Error = null,
        };
    }
}
