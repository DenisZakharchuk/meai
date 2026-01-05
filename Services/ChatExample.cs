using OpenAI.Chat;
using MeAI.Services;

namespace MeAI.Services;

/// <summary>
/// Example: Basic chat interaction
/// Demonstrates: chat messages, responses, and conversation history
/// </summary>
public class ChatExample
{
    private readonly IChatService _chatService;

    public ChatExample(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n=== Chat Example ===");
        Console.WriteLine("This example demonstrates basic chat interaction with AI model.\n");

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage("You are a helpful AI assistant. Keep responses concise and friendly."),
            ChatMessage.CreateUserMessage("What is OpenAI's GPT technology?")
        };

        try
        {
            var response = await _chatService.GetChatCompletionAsync(messages);

            Console.WriteLine("User: What is OpenAI's GPT technology?");
            Console.WriteLine($"Assistant: {response}\n");

            // Continue conversation
            messages.Add(ChatMessage.CreateAssistantMessage(response));
            messages.Add(ChatMessage.CreateUserMessage("What are its main features?"));

            response = await _chatService.GetChatCompletionAsync(messages);

            Console.WriteLine("User: What are its main features?");
            Console.WriteLine($"Assistant: {response}\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
