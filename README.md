# OllieAve.GitCliSuperset

I build this tool to save me a little time after a policy came in wherby I had to structure my commit messages in a certain way.

I'm lazy, I don't want to do this, so I automated it.

## Prerequisites

- You must have Git installed.
- You must also be using Jira
- You must also have an OpenAi Token.

## Setting up.

Create the file `~/.gitCliSuperset/settings.json` and ensure it follows the following schema:

```json
{
    "JiraProjectKey": "YOUR_PROJECT_KEY",
	"JiraUrl": "YOUR_JIRAURL",
	"JiraUser": "YOUR_JIRAUSER",
	"JiraToken": "YOUR_JIRATOKEN",
	"OpenAiApiKey": "YOUR_OPENAIAPIKEY",
	"OpenAiModel": "YOUR_OPENAIMODEL"
}
```

Install the tool and away you go.

## Features

### Git commit.

Git commit has been revamped to it will produce messages in the following format based on your changes:

```
[Jira-Key]-[Jira-Number] - [Jira-Title]

- Bullet pointed
- Summary of the
- Changes
```

### Git checkout.

Git checkout has also been revamped so that you can input a Jira number and it'll then prompt you with a list of remote branches (fetched and pruned) to select from.

It'll then checkout a fresh branch named `[origin-branch-name]-[jira-key]-[jira-number]`. This won't have upstream info and pending changes will be carried over if possible, if not stashed.