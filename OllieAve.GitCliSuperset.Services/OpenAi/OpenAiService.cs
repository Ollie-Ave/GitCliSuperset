using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace OllieAve.GitCliSuperset.Services.OpenAi;

public class OpenAiService
{
    private readonly IOptions<OpenAiOptions> options;
    public OpenAiService(IOptions<OpenAiOptions> options)
    {
        this.options = options;
    }

    public OpenAiOptions GetOptions()
    {
        return options.Value;
    }

    public string GenerateCommitMessage(string changes)
    {
        ChatClient client = new(
            model: options.Value.Model,
            apiKey: options.Value.ApiKey
            );

        ChatCompletion completion = client.CompleteChat(string.Format(Prompts.GenerateCommitMessage, changes));

        return completion.Content[0].Text;
    }
}
