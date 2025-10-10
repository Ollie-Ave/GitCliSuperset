namespace OllieAve.GitCliSuperset.Services.Git;

public interface IGitService
{
	Task<GitCommandResult> ExecuteCommand(string commandWithArgs);
}