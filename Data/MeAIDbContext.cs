using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

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
            entity.ToTable("embeddings");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Text).HasColumnName("text").IsRequired();
            entity.Property(e => e.Embedding).HasColumnName("embedding").HasColumnType("jsonb");
            entity.Property(e => e.ModelName).HasColumnName("model_name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        // Configure ConversationModel
        modelBuilder.Entity<ConversationModel>(entity =>
        {
            entity.ToTable("conversations");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasColumnName("title").HasMaxLength(255);
            entity.Property(e => e.ModelName).HasColumnName("model_name");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configure relationship with Messages
            entity.HasMany(c => c.Messages)
                .WithOne(m => m.Conversation)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure MessageModel
        modelBuilder.Entity<MessageModel>(entity =>
        {
            entity.ToTable("messages");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConversationId).HasColumnName("conversation_id");
            entity.Property(e => e.Role).HasColumnName("role").IsRequired();
            entity.Property(e => e.Content).HasColumnName("content").IsRequired();
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Index for conversation lookup
            entity.HasIndex(e => e.ConversationId);
        });
    }
}
