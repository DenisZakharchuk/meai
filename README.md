# MeAI - OpenAI Learning Project

A comprehensive C# learning project demonstrating practical use-cases and patterns for **OpenAI APIs** and building AI-powered applications using .NET.

## 🎯 Project Overview

This project provides hands-on examples of:
- **Chat Interactions**: Multi-turn conversations with AI models
- **Text Generation**: Content creation with configurable parameters (temperature, token limits)
- **Embeddings**: Vector representations for semantic analysis and similarity
- **Streaming**: Real-time token-by-token response handling
- **Data Persistence**: Store conversations and embeddings in PostgreSQL

## 📋 Prerequisites

- **.NET 8.0** or later
- **OpenAI API key** (get one at [platform.openai.com](https://platform.openai.com))
- Basic C# knowledge

## 🚀 Quick Start

### 1. Clone and Setup

```bash
cd /home/zakharchukd/develop/meai
dotnet restore
```

### 2. Configure API Key

**Option A: User Secrets (Recommended)**
```bash
dotnet user-secrets init
dotnet user-secrets set "OpenAI:ApiKey" "sk-..."
```

**Option B: appsettings.json**
```json
{
  "OpenAI": {
    "ApiKey": "sk-..."
  }
}
```

### 3. Run the Application

```bash
dotnet run
```

## 📁 Project Structure

```
MeAI/
├── Program.cs                      # Entry point with DI setup
├── appsettings.json               # Configuration
├── MeAI.csproj                    # Project file
│
├── Services/                      # Core AI functionality
│   ├── AIServiceExtensions.cs     # DI configuration
│   ├── ChatService.cs             # Chat operations
│   ├── EmbeddingService.cs        # Embedding operations
│   ├── IRepositories.cs           # Persistence interfaces
│   ├── Repositories.cs            # Repository implementations
│   ├── ChatExample.cs             # Chat example
│   ├── TextGenerationExample.cs   # Text generation example
│   ├── EmbeddingExample.cs        # Embeddings example
│   ├── StreamingExample.cs        # Streaming example
│   └── PersistenceExample.cs      # Persistence example
│
├── Data/                          # Database layer
│   ├── Models.cs                  # Database models
│   └── MeAIDbContext.cs           # Entity Framework DbContext
│
├── UI/
│   └── InteractiveApp.cs          # Interactive console menu
│
├── docker-compose.yml             # Docker configuration
├── scripts/
│   ├── init-db.sql                # Database initialization
│   └── pgadmin-servers.json       # pgAdmin configuration
│
└── README.md                       # Documentation
```

## 📚 Examples

### 1. Chat Example
Demonstrates multi-turn conversations with system prompts and message history management.

**Key Features**:
- System role for AI behavior control
- User message input
- Conversation continuity
- Message history tracking

### 2. Text Generation Example
Shows how to control output characteristics through parameters.

**Key Features**:
- Temperature control (creativity vs determinism)
- Token limits (response length)
- System prompts for style guidance
- Comparative responses

### 3. Embedding Example
Demonstrates semantic analysis using vector representations.

**Key Features**:
- Text-to-vector conversion
- Semantic similarity calculation
- Cosine similarity implementation
- Batch embedding generation

### 4. Streaming Example
Shows real-time token streaming for improved UX.

**Key Features**:
- Async streaming
- Progressive token output
- Real-time response rendering
- Better user experience for long responses

### 5. Persistence Example
Demonstrates storing conversations and embeddings in PostgreSQL.

**Key Features**:
- Create and manage conversations
- Store and retrieve message history
- Persist embeddings for semantic search
- Data survives application restarts
- Uses Entity Framework Core ORM
- PostgreSQL with JSONB storage for embeddings

**Prerequisites**:
- Docker and docker-compose installed
- Run `docker-compose up -d` to start PostgreSQL

## 🔌 Core Classes

### IChatService
Manages chat operations with OpenAI API.

```csharp
// Single completion
var response = await chatService.GetChatCompletionAsync(messages, temperature: 0.7f);

// Streaming
var stream = await chatService.GetStreamingChatCompletionAsync(messages);
await foreach (var chunk in stream)
{
    Console.Write(chunk);
}
```

### IEmbeddingService
Handles text embedding generation.

```csharp
var embedding = await embeddingService.GetEmbeddingAsync("text");
var embeddings = await embeddingService.GetEmbeddingsAsync(texts);
```

## 🛠️ Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning"
    }
  },
  "OpenAI": {
    "ApiKey": "your-api-key-here",
    "ChatModel": "gpt-4-turbo",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

## 📦 NuGet Dependencies

```xml
<PackageReference Include="OpenAI" Version="2.1.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

## 🗄️ Database Persistence

This project includes full data persistence support using **PostgreSQL** with **Entity Framework Core**.

### What Gets Stored?

- **Conversations**: Full chat conversation history with timestamps
- **Messages**: Individual messages (user, assistant, system) with roles
- **Embeddings**: Text embeddings for semantic search and analysis

### Getting Started with Persistence

#### 1. Start PostgreSQL and pgAdmin with Docker

```bash
# Make sure Docker is installed and running, then:
docker-compose up -d

# Verify containers are running:
docker-compose ps
```

This will start:
- **PostgreSQL**: Running on `localhost:5432`
  - Username: `meai_user`
  - Password: `meai_password`
  - Database: `meai_db`

- **pgAdmin**: Web UI running on `http://localhost:5050`
  - Email: `admin@meai.local`
  - Password: `admin_password`
  - Pre-configured PostgreSQL server connection

#### 2. Configure Connection String

The `appsettings.json` is already configured for local Docker:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=meai_db;Username=meai_user;Password=meai_password;"
  }
}
```

For other environments, update the connection string accordingly.

#### 3. Initialize the Database

The database schema is automatically created on first run via Entity Framework Core. Tables created:

- **conversations** - Stores conversation metadata
- **messages** - Stores individual messages with conversation references
- **embeddings** - Stores text and embeddings

#### 4. Use Persistence in Your Code

```csharp
// Inject repositories
private readonly IConversationRepository _conversationRepository;
private readonly IEmbeddingRepository _embeddingRepository;

// Create a conversation
var conversation = await _conversationRepository.CreateAsync(
    title: "My Discussion",
    modelName: "gpt-4-turbo");

// Add messages
await _conversationRepository.AddMessageAsync(
    conversation.Id, "user", "Hello, how are you?");

// Store embeddings
var embedding = await embeddingService.GetEmbeddingAsync("Sample text");
await _embeddingRepository.StoreAsync("Sample text", embedding);

// Retrieve data
var storedConversation = await _conversationRepository.GetByIdAsync(conversation.Id);
var allEmbeddings = await _embeddingRepository.GetAllAsync();
```

### PersistenceExample

The application includes a complete example demonstrating:

```bash
dotnet run
# Select option 5: Persistence Example
```

This example shows:
- Creating and managing conversations
- Storing/retrieving messages
- Embedding storage and search
- Data persistence across application restarts

### Repository Interfaces

#### IConversationRepository
- `CreateAsync()` - Create new conversation
- `GetByIdAsync()` - Retrieve conversation with all messages
- `GetAllAsync()` - List all conversations
- `UpdateAsync()` - Update conversation title
- `DeleteAsync()` - Delete conversation (cascades to messages)
- `AddMessageAsync()` - Add message to conversation
- `GetMessagesAsync()` - Retrieve conversation messages

#### IEmbeddingRepository
- `StoreAsync()` - Save text and embedding
- `GetAllAsync()` - Retrieve all embeddings
- `GetByIdAsync()` - Get specific embedding
- `DeleteAsync()` - Remove embedding
- `SearchAsync()` - Find similar embeddings (cosine similarity)

### Managing PostgreSQL

#### View Data with pgAdmin

1. Open `http://localhost:5050` in your browser
2. Login with `admin@meai.local` / `admin_password`
3. Expand "Servers" → "MeAI PostgreSQL"
4. Browse tables under "Databases" → "meai_db" → "public"

#### Command Line Access

```bash
# Connect to PostgreSQL
psql -h localhost -U meai_user -d meai_db

# List tables
\dt

# View conversations
SELECT * FROM conversations;

# View messages for a conversation
SELECT * FROM messages WHERE conversation_id = 1;
```

### Stopping Containers

```bash
# Stop without removing data:
docker-compose stop

# Stop and remove containers (data persists in volumes):
docker-compose down

# Stop and remove everything (data also removed):
docker-compose down -v
```

### Production Considerations

For production use, consider:

1. **Vector Search**: Upgrade to pgvector extension for native vector similarity
2. **Connection Pooling**: Use Npgsql connection pooling
3. **Migrations**: Use EF Core migrations for schema updates
4. **Backups**: Regular PostgreSQL backups
5. **Security**: Use environment variables for connection strings and credentials

## 🎓 Learning Paths

### Beginner
1. Run the application
2. Try the Chat Example
3. Read the chat service implementation
4. Modify prompts and observe responses

### Intermediate
1. Study TextGenerationExample for parameter tuning
2. Explore EmbeddingExample for semantic understanding
3. Implement custom example combining concepts
4. Modify temperature and max_tokens values

### Advanced
1. Implement proper error handling and retries
2. Add caching for embeddings
3. Implement conversation persistence
4. Build a custom AI feature

## 🧪 Testing the Examples

Run the application and select examples from the interactive menu:

```bash
# Start PostgreSQL containers first (for persistence example)
docker-compose up -d

# Run the application
dotnet run

# Select examples:
# Option 1: Chat Example
# Option 2: Text Generation Example
# Option 3: Embeddings Example
# Option 4: Streaming Example
# Option 5: Persistence Example (requires PostgreSQL)
# Option 6: Documentation
# Option 7: Exit
```

## 🔐 Security Best Practices

Never commit API keys to version control!

```bash
# Initialize secrets (one-time)
dotnet user-secrets init

# Set secrets securely
dotnet user-secrets set "OpenAI:ApiKey" "your-key"

# View all secrets
dotnet user-secrets list
```

## 📖 Resources

- [OpenAI Python SDK](https://github.com/openai/openai-python)
- [OpenAI API Documentation](https://platform.openai.com/docs)
- [.NET Documentation](https://docs.microsoft.com/dotnet)
- [Dependency Injection in .NET](https://docs.microsoft.com/en-us/dotnet/core/extensions/dependency-injection)

## 🤝 Common Issues & Solutions

### Issue: "API key not configured"
**Solution**: Set your API key using user secrets or appsettings.json

### Issue: "401 Unauthorized"
**Solution**: Verify your API key is valid and has remaining quota

### Issue: "Model not found"
**Solution**: Check the ChatModel and EmbeddingModel settings match your available models

### Issue: Embedding errors
**Solution**: Ensure you have the correct embedding model configured (text-embedding-3-small or text-embedding-3-large)

## 📝 Next Steps

1. **Extend Examples**: Add new features to existing examples
2. **Custom Models**: Implement support for different AI models
3. **Persist Data**: Store conversations and embeddings in a database
4. **Build UI**: Create a GUI using WPF or MAUI
5. **Deploy**: Host as a service or cloud application

## 📄 License

This project is provided as-is for educational purposes.
