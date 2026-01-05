using OpenAI.Chat;
using MeAI.Services;

namespace MeAI.Services;

/// <summary>
/// Example: Streaming responses
/// Demonstrates: async streaming, real-time response handling
/// </summary>
public class StreamingExample
{
    private readonly IChatService _chatService;

    public StreamingExample(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n=== Streaming Example ===");
        Console.WriteLine("This example demonstrates streaming responses in real-time.\n");

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage("You are a helpful AI assistant."),
            ChatMessage.CreateUserMessage("Explain quantum computing in 3 paragraphs.")
        };

        try
        {
            Console.WriteLine("User: Explain quantum computing in 3 paragraphs.");
            Console.WriteLine("Assistant: ");

            var stream = await _chatService.GetStreamingChatCompletionAsync(messages);
            await foreach (var chunk in stream)
            {
                if (!string.IsNullOrEmpty(chunk))
                {
                    Console.Write(chunk);
                }
            }

            Console.WriteLine("\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
