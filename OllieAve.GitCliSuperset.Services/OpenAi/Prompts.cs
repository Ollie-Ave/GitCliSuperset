namespace OllieAve.GitCliSuperset.Services.OpenAi;

public static class Prompts
{
	public const string GenerateCommitMessage = """
	You are a helpful assistant that generates a commit message for a given Jira issue.
	You will be given the changes present in the current branch.
	Please summarise the changes in a way that is easy to understand and will be used as the commit message.
	The commit messages should be just simple bullet poitns of the change, for example:

	"
	- Updated the login page so that user's password is obfuscated when entering it.
	- Ensured that the values are encrypted before being stored in the database.
	- Added a new field to the user model to store the hashed password.
	"

	here are my git changes:
	{0}
	""";
}