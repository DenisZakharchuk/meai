namespace MeAI.Data;

/// <summary>
/// Represents a stored embedding with metadata
/// </summary>
public class EmbeddingModel
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
    /// <summary>
    /// Embedding stored as JSON array of floats
    /// </summary>
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public string ModelName { get; set; } = "text-embedding-3-small";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Represents a conversation with an AI model
/// </summary>
public class ConversationModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string ModelName { get; set; } = "gpt-4-turbo";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation property
    public ICollection<MessageModel> Messages { get; set; } = new List<MessageModel>();
}

/// <summary>
/// Represents a single message in a conversation
/// </summary>
public class MessageModel
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public string Role { get; set; } = string.Empty; // "system", "user", "assistant"
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign key
    public ConversationModel? Conversation { get; set; }
}
