using Microsoft.EntityFrameworkCore;

namespace MeAI.Data;

/// <summary>
/// Entity Framework Core DbContext for MeAI database
/// </summary>
public class MeAIDbContext : DbContext
{
    public MeAIDbContext(DbContextOptions<MeAIDbContext> options) : base(options)
    {
    }

    public DbSet<EmbeddingModel> Embeddings { get; set; } = null!;
    public DbSet<ConversationModel> Conversations { get; set; } = null!;
    public DbSet<MessageModel> Messages { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure EmbeddingModel
        modelBuilder.Entity<EmbeddingModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Text).IsRequired();
            entity.Property(e => e.ModelName).HasDefaultValue("text-embedding-3-small");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Store embedding as JSON array
            entity.Property(e => e.Embedding)
                .HasColumnType("jsonb");
        });

        // Configure ConversationModel
        modelBuilder.Entity<ConversationModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.ModelName).HasDefaultValue("gpt-4-turbo");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Configure relationship with Messages
            entity.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure MessageModel
        modelBuilder.Entity<MessageModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            
            // Index for conversation lookup
            entity.HasIndex(e => e.ConversationId);
        });
    }
}
