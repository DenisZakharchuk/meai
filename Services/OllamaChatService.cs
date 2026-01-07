using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenAI.Chat;

namespace MeAI.Services;

/// <summary>
/// Ollama chat service implementation - uses locally running Ollama instance
/// </summary>
public class OllamaChatService : IChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _modelId;
    private readonly string _baseUrl;

    public OllamaChatService(HttpClient httpClient, string modelId, string baseUrl = "http://localhost:11434")
    {
        _httpClient = httpClient;
        _modelId = modelId;
        _baseUrl = baseUrl;
    }

    public async Task<string> GetChatCompletionAsync(List<ChatMessage> messages, float? temperature = null, int? maxTokens = null)
    {
        var ollaamaMessages = ConvertMessages(messages);

        var request = new OllamaCompletionRequest
        {
            Model = _modelId,
            Messages = ollaamaMessages,
            Stream = false,
            Temperature = temperature ?? 0.7f,
            NumPredict = maxTokens ?? -1
        };

        var json = JsonSerializer.Serialize(request);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<OllamaCompletionResponse>(responseJson);

        return result?.Message?.Content ?? "No response from Ollama";
    }

    public Task<IAsyncEnumerable<string>> GetStreamingChatCompletionAsync(List<ChatMessage> messages)
    {
        var ollaamaMessages = ConvertMessages(messages);

        async IAsyncEnumerable<string> StreamResults()
        {
            var request = new OllamaCompletionRequest
            {
                Model = _modelId,
                Messages = ollaamaMessages,
                Stream = true,
                Temperature = 0.7f,
                NumPredict = -1
            };

            var json = JsonSerializer.Serialize(request);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var response = await _httpClient.PostAsync($"{_baseUrl}/api/chat", httpContent))
            {
                response.EnsureSuccessStatusCode();

                using (var stream = await response.Content.ReadAsStreamAsync())
                using (var reader = new StreamReader(stream))
                {
                    string? line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(line))
                        {
                            if (TryParseOllamaResponse(line, out var responseText))
                            {
                                yield return responseText;
                            }
                        }
                    }
                }
            }
        }

        return Task.FromResult<IAsyncEnumerable<string>>(StreamResults());
    }

    private bool TryParseOllamaResponse(string line, out string responseContent)
    {
        responseContent = "";
        try
        {
            var delta = JsonSerializer.Deserialize<OllamaCompletionResponse>(line);
            if (delta?.Message?.Content != null)
            {
                responseContent = delta.Message.Content;
                return true;
            }
        }
        catch (JsonException)
        {
            // Skip invalid JSON lines
        }
        return false;
    }

    private List<OllamaMessage> ConvertMessages(List<ChatMessage> messages)
    {
        return messages.Select(msg => new OllamaMessage
        {
            Role = GetRoleName(msg),
            Content = msg.Content[0].Text ?? ""
        }).ToList();
    }

    private string GetRoleName(ChatMessage msg)
    {
        // ChatMessage from OpenAI SDK has a Role property
        var roleStr = msg.GetType().GetProperty("Role")?.GetValue(msg)?.ToString() ?? "user";
        return roleStr.ToLower();
    }
}

/// <summary>
/// Ollama API request models
/// </summary>
public class OllamaCompletionRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = "";

    [JsonPropertyName("messages")]
    public List<OllamaMessage> Messages { get; set; } = new();

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    [JsonPropertyName("temperature")]
    public float Temperature { get; set; } = 0.7f;

    [JsonPropertyName("num_predict")]
    public int NumPredict { get; set; } = -1;
}

public class OllamaMessage
{
    [JsonPropertyName("role")]
    public string Role { get; set; } = "";

    [JsonPropertyName("content")]
    public string Content { get; set; } = "";
}

public class OllamaCompletionResponse
{
    [JsonPropertyName("message")]
    public OllamaMessage? Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }
}
