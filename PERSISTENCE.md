# Database Persistence Guide

This document provides detailed information about the PostgreSQL persistence layer in MeAI.

## Overview

The MeAI project includes complete database persistence using:
- **PostgreSQL** - Relational database
- **Entity Framework Core** - ORM framework
- **pgAdmin** - Web-based database management UI

## Architecture

### Data Models

#### EmbeddingModel
Stores text embeddings for semantic search:
```
- Id (int, PK)
- Text (string) - The original text
- Embedding (float[]) - Vector stored as JSONB
- ModelName (string) - The model used to generate embedding
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
```

#### ConversationModel
Represents a conversation session:
```
- Id (int, PK)
- Title (string)
- ModelName (string) - The AI model used
- CreatedAt (DateTime)
- UpdatedAt (DateTime)
- Messages (ICollection<MessageModel>) - Navigation property
```

#### MessageModel
Individual message in a conversation:
```
- Id (int, PK)
- ConversationId (int, FK)
- Role (string) - "system", "user", or "assistant"
- Content (string) - The message text
- CreatedAt (DateTime)
- Conversation (ConversationModel) - Navigation property
```

## Setup Instructions

### Prerequisites

1. Docker and Docker Compose installed
2. .NET 8.0 SDK
3. Git

### Starting PostgreSQL

```bash
# Navigate to project root
cd /path/to/meai

# Start PostgreSQL and pgAdmin containers
docker-compose up -d

# Verify containers are running
docker-compose ps
```

Expected output:
```
NAME          STATUS
meai-postgres   Up (healthy)
meai-pgadmin    Up
```

### Connection Details

| Component | Host | Port | Username | Password |
|-----------|------|------|----------|----------|
| PostgreSQL | localhost | 5432 | meai_user | meai_password |
| pgAdmin | localhost | 5050 | admin@meai.local | admin_password |

### Database Configuration

Connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=meai_db;Username=meai_user;Password=meai_password;"
  }
}
```

## Using Repositories

### IConversationRepository

Manage conversation history:

```csharp
// Inject in constructor
private readonly IConversationRepository _conversationRepository;

// Create conversation
var conversation = await _conversationRepository.CreateAsync(
    title: "Technical Discussion",
    modelName: "gpt-4-turbo");

// Add messages
await _conversationRepository.AddMessageAsync(
    conversationId: conversation.Id,
    role: "user",
    content: "What is C#?");

// Retrieve conversation
var retrieved = await _conversationRepository.GetByIdAsync(conversation.Id);
foreach (var msg in retrieved.Messages)
{
    Console.WriteLine($"[{msg.Role}]: {msg.Content}");
}

// List all conversations
var all = await _conversationRepository.GetAllAsync();

// Update title
await _conversationRepository.UpdateAsync(
    id: conversation.Id,
    title: "New Title");

// Delete conversation (cascades to messages)
await _conversationRepository.DeleteAsync(conversation.Id);
```

### IEmbeddingRepository

Persist and search embeddings:

```csharp
// Inject in constructor
private readonly IEmbeddingRepository _embeddingRepository;

// Store embedding
var embedding = await embeddingService.GetEmbeddingAsync("Sample text");
var stored = await _embeddingRepository.StoreAsync(
    text: "Sample text",
    embedding: embedding,
    modelName: "text-embedding-3-small");

// Retrieve all embeddings
var all = await _embeddingRepository.GetAllAsync();

// Get specific embedding
var found = await _embeddingRepository.GetByIdAsync(storedId);

// Search similar embeddings
var queryEmbedding = await embeddingService.GetEmbeddingAsync("Related text");
var similar = await _embeddingRepository.SearchAsync(
    embedding: queryEmbedding,
    limit: 5);

// Delete embedding
await _embeddingRepository.DeleteAsync(embeddingId);
```

## Managing PostgreSQL

### Using pgAdmin Web UI

1. Open http://localhost:5050
2. Login: `admin@meai.local` / `admin_password`
3. Navigate to: Servers → MeAI PostgreSQL → Databases → meai_db → Schemas → public
4. View tables: Conversations, Messages, Embeddings

### Using psql Command Line

```bash
# Connect to PostgreSQL
psql -h localhost -U meai_user -d meai_db

# List all tables
\dt

# View conversations
SELECT id, title, model_name, created_at FROM conversations ORDER BY created_at DESC;

# View messages for conversation ID 1
SELECT id, role, content, created_at FROM messages 
WHERE conversation_id = 1 
ORDER BY created_at ASC;

# View embeddings
SELECT id, text, model_name, created_at FROM embeddings 
ORDER BY created_at DESC;

# Count records
SELECT COUNT(*) FROM conversations;
SELECT COUNT(*) FROM messages;
SELECT COUNT(*) FROM embeddings;

# Exit
\q
```

### Docker Commands

```bash
# View logs
docker-compose logs -f postgres

# Stop containers (data persists)
docker-compose stop

# Start existing containers
docker-compose start

# Restart containers
docker-compose restart

# Remove containers (data persists in volumes)
docker-compose down

# Remove containers AND data
docker-compose down -v

# Access PostgreSQL shell directly
docker exec -it meai-postgres psql -U meai_user -d meai_db
```

## Database Schema

### Conversations Table
```sql
CREATE TABLE conversations (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255),
    model_name VARCHAR(100) DEFAULT 'gpt-4-turbo',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

### Messages Table
```sql
CREATE TABLE messages (
    id SERIAL PRIMARY KEY,
    conversation_id INTEGER NOT NULL REFERENCES conversations(id) ON DELETE CASCADE,
    role VARCHAR(50) NOT NULL,
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_messages_conversation ON messages(conversation_id);
```

### Embeddings Table
```sql
CREATE TABLE embeddings (
    id SERIAL PRIMARY KEY,
    text TEXT NOT NULL,
    embedding jsonb,  -- Vector stored as JSON array
    model_name VARCHAR(100) DEFAULT 'text-embedding-3-small',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
```

## Performance Considerations

### Optimization Tips

1. **Batch Operations**: Insert multiple records in a single transaction
   ```csharp
   _context.Messages.AddRange(messagesToInsert);
   await _context.SaveChangesAsync();
   ```

2. **Lazy Loading**: Load messages only when needed
   ```csharp
   // Explicitly load messages
   await _context.Entry(conversation)
       .Collection(c => c.Messages)
       .LoadAsync();
   ```

3. **Indexing**: Already optimized with:
   - Primary keys on all tables
   - Foreign key index on messages.conversation_id

4. **Connection Pooling**: Automatically managed by Npgsql

### Scaling Strategies

For production environments:

1. **Vector Search**: Upgrade to pgvector extension for native vector operations
   ```sql
   CREATE EXTENSION IF NOT EXISTS vector;
   ALTER TABLE embeddings 
   ADD COLUMN embedding_vec vector(1536);
   CREATE INDEX ON embeddings USING ivfflat (embedding_vec vector_cosine_ops);
   ```

2. **Partitioning**: Partition messages table by conversation_id for large datasets

3. **Replication**: Set up PostgreSQL streaming replication

4. **Connection Pooling**: Use PgBouncer for connection management

## Troubleshooting

### Connection Refused

**Problem**: `Could not connect to database`

**Solution**:
```bash
# Check if containers are running
docker-compose ps

# Start containers
docker-compose up -d

# Check logs
docker-compose logs postgres
```

### Migrations Not Applied

**Problem**: Tables don't exist

**Solution**:
The application automatically applies migrations on startup. If needed, manually create:
```bash
psql -h localhost -U meai_user -d meai_db < scripts/init-db.sql
```

### Out of Memory

**Problem**: Docker container running out of memory

**Solution**: Update `docker-compose.yml` memory limits:
```yaml
services:
  postgres:
    # ... other config
    deploy:
      resources:
        limits:
          memory: 2G
```

### Permission Denied

**Problem**: `Permission denied for schema public`

**Solution**:
```bash
docker exec -it meai-postgres psql -U postgres -d meai_db -c \
  "GRANT ALL PRIVILEGES ON SCHEMA public TO meai_user;"
```

## Backup and Restore

### Backup Database

```bash
# Full backup
docker exec meai-postgres pg_dump -U meai_user meai_db > meai_backup.sql

# Compressed backup
docker exec meai-postgres pg_dump -U meai_user meai_db | gzip > meai_backup.sql.gz
```

### Restore Database

```bash
# Restore from backup
docker exec -i meai-postgres psql -U meai_user meai_db < meai_backup.sql

# Restore from compressed backup
gunzip -c meai_backup.sql.gz | docker exec -i meai-postgres psql -U meai_user meai_db
```

## Migration Guide

### Creating New Tables

1. Define model class in `Data/Models.cs`
2. Add DbSet to `MeAIDbContext`
3. Configure in `OnModelCreating()`
4. Run: `dotnet ef migrations add [MigrationName]`
5. Run: `dotnet ef database update`

### Modifying Existing Tables

1. Update model in `Data/Models.cs`
2. Run: `dotnet ef migrations add [MigrationName]`
3. Review generated migration
4. Run: `dotnet ef database update`

## Best Practices

1. **Always use repositories** - Don't access DbContext directly from examples
2. **Dispose DbContext** - Properly dispose context in using statements
3. **Validate input** - Sanitize user input before database operations
4. **Transaction management** - Use transactions for related operations
5. **Error handling** - Wrap database operations in try-catch blocks
6. **Logging** - Enable EF Core logging for debugging:
   ```csharp
   services.AddLogging(builder => 
   {
       builder.AddConsole();
       builder.AddDebug();
   });
   ```

## Resources

- [Entity Framework Core Documentation](https://learn.microsoft.com/en-us/ef/core/)
- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [pgAdmin Documentation](https://www.pgadmin.org/docs/)
- [Docker Documentation](https://docs.docker.com/)
- [Npgsql Documentation](https://www.npgsql.org/doc/)

## Support

For issues or questions:
1. Check Docker logs: `docker-compose logs`
2. Review database with pgAdmin
3. Test connection: `psql -h localhost -U meai_user -d meai_db`
4. Check application logs for Entity Framework errors
