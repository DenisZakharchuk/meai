using Microsoft.Extensions.DependencyInjection;
using MeAI.Services;

namespace MeAI.UI;

/// <summary>
/// Interactive console UI for exploring Microsoft.Extensions.AI examples
/// </summary>
public class InteractiveApp
{
    private readonly IServiceProvider _serviceProvider;

    public InteractiveApp(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task RunAsync()
    {
        Console.Clear();
        PrintWelcome();

        while (true)
        {
            PrintMenu();
            var choice = Console.ReadLine()?.Trim().ToLower() ?? "";

            switch (choice)
            {
                case "1":
                    await RunExample("Chat Example", async () =>
                    {
                        var example = _serviceProvider.GetRequiredService<ChatExample>();
                        await example.RunAsync();
                    });
                    break;

                case "2":
                    await RunExample("Text Generation Example", async () =>
                    {
                        var example = _serviceProvider.GetRequiredService<TextGenerationExample>();
                        await example.RunAsync();
                    });
                    break;

                case "3":
                    await RunExample("Embedding Example", async () =>
                    {
                        var example = _serviceProvider.GetRequiredService<EmbeddingExample>();
                        await example.RunAsync();
                    });
                    break;

                case "4":
                    await RunExample("Streaming Example", async () =>
                    {
                        var example = _serviceProvider.GetRequiredService<StreamingExample>();
                        await example.RunAsync();
                    });
                    break;

                case "5":
                    PrintDocumentation();
                    break;

                case "6":
                case "q":
                case "exit":
                    Console.WriteLine("\n👋 Goodbye!");
                    return;

                default:
                    Console.WriteLine("❌ Invalid option. Please try again.\n");
                    break;
            }
        }
    }

    private static void PrintWelcome()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║    Microsoft.Extensions.AI Learning Application            ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║  Explore practical scenarios and use-cases with the        ║");
        Console.WriteLine("║  Microsoft.Extensions.AI library (v9.0.0)                  ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\n📋 SELECT AN EXAMPLE:");
        Console.WriteLine("  1. Chat - Basic conversation with AI model");
        Console.WriteLine("  2. Text Generation - Create content with different parameters");
        Console.WriteLine("  3. Embeddings - Generate and analyze text embeddings");
        Console.WriteLine("  4. Streaming - Real-time streaming responses");
        Console.WriteLine("  5. Documentation - Learn more about this project");
        Console.WriteLine("  6. Exit");
        Console.Write("\n👉 Enter your choice (1-6): ");
    }

    private async Task RunExample(string title, Func<Task> example)
    {
        Console.Clear();
        try
        {
            await example();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine("💡 Make sure your OpenAI API key is configured in appsettings.json");
        }

        Console.WriteLine("\n" + new string('─', 60));
        Console.Write("Press any key to return to menu...");
        Console.ReadKey(true);
        Console.Clear();
    }

    private static void PrintDocumentation()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════════════════════════════════════════════════");
        Console.WriteLine("📚 MICROSOFT.EXTENSIONS.AI OVERVIEW");
        Console.WriteLine("═══════════════════════════════════════════════════════════════\n");

        Console.WriteLine("🎯 WHAT IS MICROSOFT.EXTENSIONS.AI?");
        Console.WriteLine("────────────────────────────────────────────────────────────────");
        Console.WriteLine(@"
Microsoft.Extensions.AI (v9.0.0) is a modern .NET library that provides:

  • Unified API for AI/ML operations across different models and providers
  • Support for chat, embeddings, and text generation
  • Built-in support for streaming responses
  • Dependency injection integration for easy service configuration
  • Type-safe chat message handling with role-based semantics
  • Extensible architecture for custom implementations
");

        Console.WriteLine("\n📦 KEY COMPONENTS IN THIS PROJECT:");
        Console.WriteLine("────────────────────────────────────────────────────────────────");
        Console.WriteLine(@"
  ChatExample
    ├─ Demonstrates multi-turn conversations
    ├─ Shows how to maintain message history
    └─ Uses system prompts for AI behavior control

  TextGenerationExample
    ├─ Different generation parameters (temperature, max tokens)
    ├─ Creative vs. deterministic outputs
    └─ Content generation use-cases

  EmbeddingExample
    ├─ Convert text to vector representations
    ├─ Calculate semantic similarity
    └─ Used for search, clustering, recommendations

  StreamingExample
    ├─ Real-time response streaming
    ├─ Progressive token-by-token output
    └─ Improved UX for long responses
");

        Console.WriteLine("\n🔧 CONFIGURATION:");
        Console.WriteLine("────────────────────────────────────────────────────────────────");
        Console.WriteLine(@"
Set your OpenAI API key in one of these ways:

  1. appsettings.json (not recommended for secrets)
  2. User Secrets (recommended for local development)
  3. Environment variables (for production)

Command to set user secret:
  dotnet user-secrets set ""OpenAI:ApiKey"" ""your-api-key""
");

        Console.WriteLine("\n💡 LEARNING RESOURCES:");
        Console.WriteLine("────────────────────────────────────────────────────────────────");
        Console.WriteLine(@"
  • Official: https://github.com/microsoft/extensions
  • Examples: Check the Examples/ folder in this project
  • Tests: Look for [ExampleName]Example.cs files for implementation details
");

        Console.WriteLine("\n" + new string('─', 60));
        Console.Write("Press any key to return to menu...");
        Console.ReadKey(true);
        Console.Clear();
    }
}
