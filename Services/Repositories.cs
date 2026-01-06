using Microsoft.EntityFrameworkCore;
using MeAI.Data;

namespace MeAI.Services;

/// <summary>
/// Repository for conversation persistence operations
/// </summary>
public class ConversationRepository : IConversationRepository
{
    private readonly MeAIDbContext _context;

    public ConversationRepository(MeAIDbContext context)
    {
        _context = context;
    }

    public async Task<ConversationModel> CreateAsync(string? title = null, string modelName = "gpt-4-turbo")
    {
        var conversation = new ConversationModel
        {
            Title = title ?? $"Conversation at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}",
            ModelName = modelName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync();

        return conversation;
    }

    public async Task<ConversationModel?> GetByIdAsync(int id)
    {
        return await _context.Conversations
            .Include(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<ConversationModel>> GetAllAsync()
    {
        return await _context.Conversations
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<ConversationModel> UpdateAsync(int id, string? title = null)
    {
        var conversation = await GetByIdAsync(id);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {id} not found");

        if (!string.IsNullOrEmpty(title))
            conversation.Title = title;

        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return conversation;
    }

    public async Task DeleteAsync(int id)
    {
        var conversation = await GetByIdAsync(id);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {id} not found");

        _context.Conversations.Remove(conversation);
        await _context.SaveChangesAsync();
    }

    public async Task AddMessageAsync(int conversationId, string role, string content)
    {
        var conversation = await GetByIdAsync(conversationId);
        if (conversation == null)
            throw new InvalidOperationException($"Conversation with ID {conversationId} not found");

        var message = new MessageModel
        {
            ConversationId = conversationId,
            Role = role,
            Content = content,
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        conversation.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task<List<MessageModel>> GetMessagesAsync(int conversationId)
    {
        return await _context.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderBy(m => m.CreatedAt)
            .ToListAsync();
    }
}

/// <summary>
/// Repository for embedding persistence operations
/// </summary>
public class EmbeddingRepository : IEmbeddingRepository
{
    private readonly MeAIDbContext _context;

    public EmbeddingRepository(MeAIDbContext context)
    {
        _context = context;
    }

    public async Task<EmbeddingModel> StoreAsync(string text, float[] embedding, string modelName = "text-embedding-3-small")
    {
        var embeddingModel = new EmbeddingModel
        {
            Text = text,
            Embedding = embedding,
            ModelName = modelName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Embeddings.Add(embeddingModel);
        await _context.SaveChangesAsync();

        return embeddingModel;
    }

    public async Task<List<EmbeddingModel>> GetAllAsync()
    {
        return await _context.Embeddings
            .OrderByDescending(e => e.CreatedAt)
            .ToListAsync();
    }

    public async Task<EmbeddingModel?> GetByIdAsync(int id)
    {
        return await _context.Embeddings
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task DeleteAsync(int id)
    {
        var embedding = await GetByIdAsync(id);
        if (embedding == null)
            throw new InvalidOperationException($"Embedding with ID {id} not found");

        _context.Embeddings.Remove(embedding);
        await _context.SaveChangesAsync();
    }

    public async Task<List<EmbeddingModel>> SearchAsync(float[] embedding, int limit = 5)
    {
        // Get all embeddings and calculate similarity in-memory
        // For production, consider using pgvector or a dedicated vector DB
        var allEmbeddings = await GetAllAsync();
        
        var results = allEmbeddings
            .Select(e => new { Embedding = e, Similarity = CalculateCosineSimilarity(embedding, e.Embedding) })
            .OrderByDescending(x => x.Similarity)
            .Take(limit)
            .Select(x => x.Embedding)
            .ToList();

        return results;
    }

    /// <summary>
    /// Calculate cosine similarity between two embeddings
    /// </summary>
    private static float CalculateCosineSimilarity(float[] a, float[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Embeddings must have the same length");

        float dotProduct = 0;
        float magnitudeA = 0;
        float magnitudeB = 0;

        for (int i = 0; i < a.Length; i++)
        {
            dotProduct += a[i] * b[i];
            magnitudeA += a[i] * a[i];
            magnitudeB += b[i] * b[i];
        }

        magnitudeA = (float)Math.Sqrt(magnitudeA);
        magnitudeB = (float)Math.Sqrt(magnitudeB);

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }
}
