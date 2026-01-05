# MeAI - OpenAI Learning Project

A comprehensive C# learning project demonstrating practical use-cases and patterns for **OpenAI APIs** and building AI-powered applications using .NET.

## 🎯 Project Overview

This project provides hands-on examples of:
- **Chat Interactions**: Multi-turn conversations with AI models
- **Text Generation**: Content creation with configurable parameters (temperature, token limits)
- **Embeddings**: Vector representations for semantic analysis and similarity
- **Streaming**: Real-time token-by-token response handling

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
│   ├── ChatExample.cs             # Chat example
│   ├── TextGenerationExample.cs   # Text generation example
│   ├── StreamingExample.cs        # Streaming example
│   └── EmbeddingExample.cs        # Embeddings example
│
└── UI/
    └── InteractiveApp.cs          # Interactive console menu
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
```

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
dotnet run
# Select option 1-4 to run examples
# Select option 5 for documentation
# Select option 6 to exit
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
