namespace OllieAve.GitCliSuperset.Console.Interfaces;

public interface IPassThroughToGitCommand
{
	Task PassThroughToGit(string[] args);
}