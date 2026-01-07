using System.Text.Json;
using MeAI.Services;
using OpenAI.Chat;

namespace MeAI.Services;

/// <summary>
/// Example: JSON output generation
/// Demonstrates: system prompts optimized for JSON output, structured data extraction
/// </summary>
public class JsonOutputExample
{
    private readonly IChatService _chatService;

    public JsonOutputExample(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n=== JSON Output Example ===");
        Console.WriteLine("This example demonstrates generating structured JSON responses.\n");

        try
        {
            // Example 1: Extract person information
            Console.WriteLine("Example 1: Extract Person Information");
            Console.WriteLine("─────────────────────────────────────");
            var personExtraction = await ExtractPersonInfo();
            Console.WriteLine(personExtraction);
            Console.WriteLine();

            // Example 2: Generate product data
            Console.WriteLine("Example 2: Generate Product Data");
            Console.WriteLine("─────────────────────────────────");
            var productData = await GenerateProductData();
            Console.WriteLine(productData);
            Console.WriteLine();

            // Example 3: Parse sentiment analysis
            Console.WriteLine("Example 3: Sentiment Analysis");
            Console.WriteLine("─────────────────────────────");
            var sentimentAnalysis = await AnalyzeSentiment();
            Console.WriteLine(sentimentAnalysis);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task<string> ExtractPersonInfo()
    {
        var systemPrompt = @"You are a JSON data extraction assistant. You MUST respond with ONLY valid JSON, no other text.
Extract person information from the provided text and return it as JSON with these exact fields:
{
  ""name"": ""full name"",
  ""age"": number,
  ""email"": ""email address"",
  ""location"": ""city, country""
}";

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage("John Doe is 28 years old, from New York, USA. His email is john.doe@example.com")
        };

        var response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.1f);
        return FormatJsonResponse(response);
    }

    private async Task<string> GenerateProductData()
    {
        var systemPrompt = @"You are a product data generator. You MUST respond with ONLY valid JSON, no other text.
Generate product information as JSON with this exact structure:
{
  ""products"": [
    {
      ""id"": 1,
      ""name"": ""product name"",
      ""price"": 99.99,
      ""category"": ""category name"",
      ""in_stock"": true
    }
  ]
}
Generate 3 sample products.";

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage("Generate sample e-commerce products")
        };

        var response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.3f);
        return FormatJsonResponse(response);
    }

    private async Task<string> AnalyzeSentiment()
    {
        var systemPrompt = @"You are a sentiment analysis tool. You MUST respond with ONLY valid JSON, no other text.
Analyze the sentiment of the provided text and return JSON with this exact structure:
{
  ""text"": ""the analyzed text"",
  ""sentiment"": ""positive|negative|neutral"",
  ""confidence"": 0.95,
  ""key_phrases"": [""phrase1"", ""phrase2""],
  ""emotions"": [""emotion1"", ""emotion2""]
}";

        var messages = new List<ChatMessage>
        {
            ChatMessage.CreateSystemMessage(systemPrompt),
            ChatMessage.CreateUserMessage("I absolutely love this product! It exceeded my expectations and the customer service was fantastic!")
        };

        var response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.1f);
        return FormatJsonResponse(response);
    }

    private string FormatJsonResponse(string response)
    {
        try
        {
            // Try to parse and pretty-print the JSON
            var jsonDoc = JsonDocument.Parse(response);
            var options = new JsonSerializerOptions { WriteIndented = true };
            return JsonSerializer.Serialize(jsonDoc, options);
        }
        catch
        {
            // If parsing fails, return as-is
            return response;
        }
    }
}
