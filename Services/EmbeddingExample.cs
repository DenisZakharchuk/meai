using MeAI.Services;

namespace MeAI.Services;

/// <summary>
/// Example: Text embeddings and semantic similarity
/// Demonstrates: embedding generation, similarity comparison
/// </summary>
public class EmbeddingExample
{
    private readonly IEmbeddingService _embeddingService;

    public EmbeddingExample(IEmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n=== Embedding Example ===");
        Console.WriteLine("This example demonstrates text embeddings and semantic similarity.\n");

        var texts = new List<string>
        {
            "The cat sat on the mat",
            "A feline rested on the rug",
            "The weather is sunny today",
            "Dogs are loyal pets"
        };

        try
        {
            Console.WriteLine("Generating embeddings for texts...");
            var embeddings = await _embeddingService.GetEmbeddingsAsync(texts);

            Console.WriteLine($"Generated {embeddings.Count} embeddings of dimension {embeddings[0].Length}\n");

            // Calculate cosine similarity
            Console.WriteLine("Similarity Analysis:");
            Console.WriteLine("Text 1: \"The cat sat on the mat\"");
            Console.WriteLine("Text 2: \"A feline rested on the rug\"");
            var similarity = CosineSimilarity(embeddings[0], embeddings[1]);
            Console.WriteLine($"Similarity: {similarity:F4} (high - semantically similar)\n");

            Console.WriteLine("Text 1: \"The cat sat on the mat\"");
            Console.WriteLine("Text 3: \"The weather is sunny today\"");
            similarity = CosineSimilarity(embeddings[0], embeddings[2]);
            Console.WriteLine($"Similarity: {similarity:F4} (low - different topics)\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private static float CosineSimilarity(float[] vectorA, float[] vectorB)
    {
        var dotProduct = vectorA.Zip(vectorB, (a, b) => a * b).Sum();
        var magnitudeA = MathF.Sqrt(vectorA.Sum(a => a * a));
        var magnitudeB = MathF.Sqrt(vectorB.Sum(b => b * b));

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }
}
