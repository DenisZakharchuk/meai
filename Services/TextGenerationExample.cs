using OpenAI.Chat;
using MeAI.Services;

namespace MeAI.Services;

/// <summary>
/// Example: Text generation with various options
/// Demonstrates: temperature, max tokens, system prompts
/// </summary>
public class TextGenerationExample
{
    private readonly IChatService _chatService;

    public TextGenerationExample(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n=== Text Generation Example ===");
        Console.WriteLine("This example demonstrates text generation with different parameters.\n");

        var prompt = "Write a short poem about artificial intelligence (max 100 words).";

        try
        {
            // Basic generation
            var messages = new List<ChatMessage>
            {
                ChatMessage.CreateSystemMessage("You are a creative writer."),
                ChatMessage.CreateUserMessage(prompt)
            };

            var response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.7f, maxTokens: 150);

            Console.WriteLine("Creative Response (Temperature: 0.7):");
            Console.WriteLine(response);
            Console.WriteLine();

            // More deterministic response
            response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.2f, maxTokens: 150);

            Console.WriteLine("Deterministic Response (Temperature: 0.2):");
            Console.WriteLine(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
