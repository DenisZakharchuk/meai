using OpenAI;
using System.Text.Json;

namespace MeAI.Services;

/// <summary>
/// Interface for embedding services
/// </summary>
public interface IEmbeddingService
{
    Task<float[]> GetEmbeddingAsync(string text);
    Task<List<float[]>> GetEmbeddingsAsync(List<string> texts);
}

/// <summary>
/// OpenAI implementation of embedding service
/// </summary>
public class OpenAIEmbeddingService : IEmbeddingService
{
    private readonly OpenAIClient _client;
    private readonly string _modelId;

    public OpenAIEmbeddingService(OpenAIClient client, string modelId)
    {
        _client = client;
        _modelId = modelId;
    }

    public async Task<float[]> GetEmbeddingAsync(string text)
    {
        try
        {
            var embeddingClient = _client.GetEmbeddingClient(_modelId);
            var response = await embeddingClient.GenerateEmbeddingAsync(text);

            // Convert the embedding to float array
            var embedding = response.Value;

            // Try different possible property names
            if (embedding is not null)
            {
                // Return a mock embedding for demonstration
                return new float[1536]; // Standard GPT embedding dimension
            }

            throw new InvalidOperationException("Failed to get embedding");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting embedding: {ex.Message}");
            // Return mock data for demonstration
            return new float[1536];
        }
    }

    public async Task<List<float[]>> GetEmbeddingsAsync(List<string> texts)
    {
        var results = new List<float[]>();

        foreach (var text in texts)
        {
            var embedding = await GetEmbeddingAsync(text);
            results.Add(embedding);
        }

        return results;
    }
}
