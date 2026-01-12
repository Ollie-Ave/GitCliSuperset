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
}
