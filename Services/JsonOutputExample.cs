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
        var systemPrompt = """
You are a JSON data extraction assistant. You MUST respond with ONLY valid JSON that conforms to the provided schema, no other text.

Extract person information from the provided text according to this JSON Schema:
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "name": {
      "type": "string",
      "description": "Full name of the person"
    },
    "age": {
      "type": "integer",
      "minimum": 0,
      "maximum": 150,
      "description": "Age in years"
    },
    "email": {
      "type": "string",
      "format": "email",
      "description": "Email address"
    },
    "location": {
      "type": "string",
      "description": "City and country"
    }
  },
  "required": ["name", "email"]
}
""";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage("John Smith, 32 years old, from New York, USA. Email: john.smith@example.com")
        };

        var response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.1f);
        return FormatJsonResponse(response);
    }

    private async Task<string> GenerateProductData()
    {
        var systemPrompt = """
You are a product data generator. You MUST respond with ONLY valid JSON that conforms to the provided schema, no other text.

Generate exactly 3 products according to this JSON Schema:
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "products": {
      "type": "array",
      "minItems": 3,
      "maxItems": 3,
      "items": {
        "type": "object",
        "properties": {
          "id": {
            "type": "integer",
            "minimum": 1,
            "description": "Product ID"
          },
          "name": {
            "type": "string",
            "minLength": 1,
            "description": "Product name"
          },
          "price": {
            "type": "number",
            "minimum": 0,
            "multipleOf": 0.01,
            "description": "Price in USD"
          },
          "in_stock": {
            "type": "boolean",
            "description": "Stock availability"
          }
        },
        "required": ["id", "name", "price", "in_stock"]
      }
    }
  },
  "required": ["products"]
}
""";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage("Generate 3 sample tech products")
        };

        var response = await _chatService.GetChatCompletionAsync(messages, temperature: 0.3f);
        return FormatJsonResponse(response);
    }

    private async Task<string> AnalyzeSentiment()
    {
        var systemPrompt = """
You are a sentiment analysis tool. You MUST respond with ONLY valid JSON that conforms to the provided schema, no other text.

Analyze the sentiment of the provided text according to this JSON Schema:
{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "type": "object",
  "properties": {
    "sentiment": {
      "type": "string",
      "enum": ["positive", "negative", "neutral"],
      "description": "Overall sentiment classification"
    },
    "confidence": {
      "type": "number",
      "minimum": 0,
      "maximum": 1,
      "description": "Confidence score between 0 and 1"
    },
    "key_phrases": {
      "type": "array",
      "items": {
        "type": "string"
      },
      "minItems": 2,
      "description": "Key phrases that indicate sentiment"
    },
    "emotions": {
      "type": "array",
      "items": {
        "type": "string"
      },
      "minItems": 1,
      "description": "Detected emotions"
    }
  },
  "required": ["sentiment", "confidence", "key_phrases", "emotions"]
}
""";

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(systemPrompt),
            new UserChatMessage("I absolutely love this product! It exceeded my expectations and the customer service was fantastic!")
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
            var formatted = JsonSerializer.Serialize(jsonDoc.RootElement, options);
            return formatted;
        }
        catch
        {
            // If parsing fails, return as-is
            return response;
        }
    }
}
