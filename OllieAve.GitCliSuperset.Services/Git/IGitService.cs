namespace OllieAve.GitCliSuperset.Services.Git;

public interface IGitService
{
    GitCommandResult ExecuteCommand(string commandWithArgs);
}
