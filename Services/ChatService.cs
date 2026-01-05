using OpenAI.Chat;
using OpenAI;

namespace MeAI.Services;

/// <summary>
/// Interface for chat operations
/// </summary>
public interface IChatService
{
    Task<string> GetChatCompletionAsync(List<ChatMessage> messages, float? temperature = null, int? maxTokens = null);
    Task<IAsyncEnumerable<string>> GetStreamingChatCompletionAsync(List<ChatMessage> messages);
}

/// <summary>
/// OpenAI chat service implementation
/// </summary>
public class OpenAIChatService : IChatService
{
    private readonly OpenAIClient _client;
    private readonly string _modelId;

    public OpenAIChatService(OpenAIClient client, string modelId)
    {
        _client = client;
        _modelId = modelId;
    }

    public async Task<string> GetChatCompletionAsync(List<ChatMessage> messages, float? temperature = null, int? maxTokens = null)
    {
        var chatClient = _client.GetChatClient(_modelId);

        var options = new ChatCompletionOptions();
        if (temperature.HasValue)
            options.Temperature = (int)(temperature.Value * 100);
        if (maxTokens.HasValue)
            options.MaxOutputTokenCount = maxTokens.Value;

        var response = await chatClient.CompleteChatAsync(messages, options);
        return response.Value.Content[0].Text;
    }

    public Task<IAsyncEnumerable<string>> GetStreamingChatCompletionAsync(List<ChatMessage> messages)
    {
        var chatClient = _client.GetChatClient(_modelId);

        async IAsyncEnumerable<string> StreamResults()
        {
            await foreach (var update in chatClient.CompleteChatStreamingAsync(messages))
            {
                if (update.ContentUpdate.Count > 0)
                {
                    yield return update.ContentUpdate[0].Text ?? "";
                }
            }
        }

        return Task.FromResult<IAsyncEnumerable<string>>(StreamResults());
    }
}
