using MeAI.Data;

namespace MeAI.Services;

/// <summary>
/// Interface for conversation persistence operations
/// </summary>
public interface IConversationRepository
{
    Task<ConversationModel> CreateAsync(string? title = null, string modelName = "gpt-4-turbo");
    Task<ConversationModel?> GetByIdAsync(int id);
    Task<List<ConversationModel>> GetAllAsync();
    Task<ConversationModel> UpdateAsync(int id, string? title = null);
    Task DeleteAsync(int id);
    Task AddMessageAsync(int conversationId, string role, string content);
    Task<List<MessageModel>> GetMessagesAsync(int conversationId);
}

/// <summary>
/// Interface for embedding persistence operations
/// </summary>
public interface IEmbeddingRepository
{
    Task<EmbeddingModel> StoreAsync(string text, float[] embedding, string modelName = "text-embedding-3-small");
    Task<List<EmbeddingModel>> GetAllAsync();
    Task<EmbeddingModel?> GetByIdAsync(int id);
    Task DeleteAsync(int id);
    Task<List<EmbeddingModel>> SearchAsync(float[] embedding, int limit = 5);
}
