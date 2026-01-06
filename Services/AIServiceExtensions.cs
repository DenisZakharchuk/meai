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

        var openAiConfig = configuration.GetSection("OpenAI");
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

        // Register example services
        services.AddScoped<ChatExample>();
        services.AddScoped<TextGenerationExample>();
        services.AddScoped<EmbeddingExample>();
        services.AddScoped<StreamingExample>();
        services.AddScoped<PersistenceExample>();

        return services;
    }
}
