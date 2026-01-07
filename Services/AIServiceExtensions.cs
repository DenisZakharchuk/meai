using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenAI;

namespace MeAI.Services;

/// <summary>
/// Extension methods to register AI services in the dependency injection container
/// </summary>
public static class AIServiceExtensions
{
    public static IServiceCollection AddAIServices(this IServiceCollection services, IConfiguration? configuration = null)
    {
        // If configuration not provided, try to get it from services
        if (configuration == null)
        {
            var sp = services.BuildServiceProvider();
            configuration = sp.GetRequiredService<IConfiguration>();
            sp.Dispose();
        }

        var llmProvider = configuration["LLMProvider"]?.ToLower() ?? "openai";
        var ollmaConfig = configuration.GetSection("Ollama");
        var openAiConfig = configuration.GetSection("OpenAI");

        if (llmProvider == "ollama")
        {
            Console.WriteLine("✓ Using Ollama as LLM provider");
            RegisterOllamaServices(services, configuration, ollmaConfig);
        }
        else
        {
            Console.WriteLine("✓ Using OpenAI as LLM provider");
            RegisterOpenAIServices(services, configuration, openAiConfig);
        }

        // Register example services
        services.AddScoped<ChatExample>();
        services.AddScoped<TextGenerationExample>();
        services.AddScoped<EmbeddingExample>();
        services.AddScoped<StreamingExample>();
        services.AddScoped<PersistenceExample>();

        return services;
    }

    private static void RegisterOpenAIServices(IServiceCollection services, IConfiguration configuration, IConfigurationSection openAiConfig)
    {
        var apiKey = openAiConfig["ApiKey"];

        if (string.IsNullOrEmpty(apiKey) || apiKey == "your-api-key-here")
        {
            Console.WriteLine("⚠️  Warning: OpenAI API key not configured. Please set it in appsettings.json or user secrets.");
        }

        // Register OpenAI client
        services.AddSingleton(sp =>
        {
            return new OpenAIClient(apiKey ?? "");
        });

        // Register chat service
        services.AddScoped<IChatService>(sp =>
        {
            var client = sp.GetRequiredService<OpenAIClient>();
            var model = openAiConfig["ChatModel"] ?? "gpt-4-turbo";
            return new OpenAIChatService(client, model);
        });

        // Register embedding service
        services.AddScoped<IEmbeddingService>(sp =>
        {
            var client = sp.GetRequiredService<OpenAIClient>();
            var model = openAiConfig["EmbeddingModel"] ?? "text-embedding-3-small";
            return new OpenAIEmbeddingService(client, model);
        });
    }

    private static void RegisterOllamaServices(IServiceCollection services, IConfiguration configuration, IConfigurationSection ollmaConfig)
    {
        var baseUrl = ollmaConfig["BaseUrl"] ?? "http://localhost:11434";
        var model = ollmaConfig["ChatModel"] ?? "mistral";

        Console.WriteLine($"✓ Ollama configured at {baseUrl} with model {model}");

        // Register HttpClient for Ollama
        services.AddHttpClient();

        // Register chat service
        services.AddScoped<IChatService>(sp =>
        {
            var httpClientFactory = sp.GetRequiredService<IHttpClientFactory>();
            var httpClient = httpClientFactory.CreateClient();
            return new OllamaChatService(httpClient, model, baseUrl);
        });

        // Register embedding service (still using OpenAI if available, or mock)
        services.AddScoped<IEmbeddingService>(sp =>
        {
            var openAiConfig = configuration.GetSection("OpenAI");
            var apiKey = openAiConfig["ApiKey"];

            if (!string.IsNullOrEmpty(apiKey) && apiKey != "your-api-key-here")
            {
                var client = new OpenAIClient(apiKey);
                var model = openAiConfig["EmbeddingModel"] ?? "text-embedding-3-small";
                return new OpenAIEmbeddingService(client, model);
            }
            else
            {
                Console.WriteLine("⚠️  Warning: OpenAI API key not configured for embeddings. Embeddings example will not work.");
                // Return a mock implementation or null
                throw new InvalidOperationException("OpenAI API key required for embeddings service");
            }
        });
    }
}

