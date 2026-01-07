# Ollama Integration Guide

## Overview
The MeAI project now supports using **Ollama** as a local LLM (Large Language Model) provider, eliminating the need for OpenAI API keys. This allows you to run AI models completely locally.

## What is Ollama?
Ollama is an open-source tool that makes it easy to run large language models locally. It handles model downloading, memory management, and provides a simple API interface.

## Setup & Configuration

### 1. Start Ollama Container
Ollama is now included in `docker-compose.yml` and starts automatically:

```bash
docker-compose up -d
```

This starts Ollama at `http://localhost:11434`

### 2. Pull a Model
Pull a model into Ollama before running examples. Some recommended lightweight models:

```bash
# Lightweight & Fast (recommended for limited resources)
docker exec meai-ollama ollama pull orca-mini

# Medium size (~2.7GB)
docker exec meai-ollama ollama pull neural-chat

# Larger model (requires ~4.5GB+ RAM)
docker exec meai-ollama ollama pull mistral
```

### 3. Configure in appsettings.json

```json
{
  "LLMProvider": "ollama",
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "ChatModel": "orca-mini",
    "Enabled": true
  },
  "OpenAI": {
    "ApiKey": "your-api-key-here",
    "ChatModel": "gpt-4-turbo",
    "EmbeddingModel": "text-embedding-3-small"
  }
}
```

**LLMProvider Options:**
- `"ollama"` - Uses local Ollama models
- `"openai"` - Uses OpenAI API (requires API key)

### 4. Switch Between Providers

To use OpenAI:
```json
"LLMProvider": "openai"
```

To use Ollama:
```json
"LLMProvider": "ollama"
```

## Running Examples

```bash
dotnet run
```

When the app starts, it will show which provider is being used:
```
✓ Using Ollama as LLM provider
✓ Ollama configured at http://localhost:11434 with model orca-mini
```

### Supported Examples with Ollama

- ✅ **Chat** (Option 1) - Multi-turn conversations
- ✅ **Text Generation** (Option 2) - Content creation with parameters
- ✅ **Streaming** (Option 4) - Real-time token streaming
- ✅ **Persistence** (Option 5) - Store conversations in PostgreSQL
- ❌ **Embeddings** (Option 3) - Still requires OpenAI API key

## Available Models

### Lightweight Models (Recommended)
- `orca-mini` (366MB) - Fast, good for testing
- `tinyllama` (637MB) - Very fast, lower quality
- `phi` (2.7GB) - Balanced performance

### Medium Models
- `neural-chat` (4.1GB) - Good quality and speed
- `mistral` (4.1GB) - More capable

### Large Models
- `llama2` (3.8GB) - Very capable
- `mixtral` (26GB) - Requires significant resources

## Architecture

### Service Registration
The `AIServiceExtensions` class now supports both providers:

```csharp
// Automatically selects provider based on config
services.AddAIServices(configuration);
```

### Chat Service Interface
Both providers implement the same `IChatService` interface:

```csharp
public interface IChatService
{
    Task<string> GetChatCompletionAsync(List<ChatMessage> messages, 
        float? temperature = null, int? maxTokens = null);
    
    Task<IAsyncEnumerable<string>> GetStreamingChatCompletionAsync(
        List<ChatMessage> messages);
}
```

### OllamaChatService
New service that communicates with Ollama API:
- File: `Services/OllamaChatService.cs`
- Implements both regular and streaming chat completions
- No authentication required

## Docker Integration

### Container Details
```yaml
ollama:
  image: ollama/ollama:latest
  container_name: meai-ollama
  ports:
    - "11434:11434"
  volumes:
    - ollama_data:/root/.ollama  # Persistent model storage
  environment:
    OLLAMA_HOST: "0.0.0.0:11434"
```

### GPU Support
To enable GPU acceleration (requires nvidia-docker):

```yaml
ollama:
  deploy:
    resources:
      reservations:
        devices:
          - driver: nvidia
            count: 1
            capabilities: [gpu]
```

## Performance Notes

### Response Times
- **First request**: Slow (5-30s) - model is loaded into memory
- **Subsequent requests**: Faster (2-5s) - model stays warm
- **Streaming**: Tokens appear as they're generated

### Memory Requirements
- `orca-mini`: ~2GB RAM
- `neural-chat`: ~4GB RAM
- `mistral`: ~5GB+ RAM

### Local Advantages
- ✅ No internet required
- ✅ No API costs
- ✅ Faster after warm-up
- ✅ Private - data stays local
- ❌ Slower on first request
- ❌ Requires local compute resources

## Troubleshooting

### "Connection refused"
```
Error: Name or service not known (ollama:11434)
Solution: Use http://localhost:11434 instead
```

### "Model too large for system memory"
```
Error: model requires more system memory (4.5 GiB) than is available
Solution: Use a smaller model like orca-mini
```

### Model not found
```
Error: model not found
Solution: Pull the model first: docker exec meai-ollama ollama pull orca-mini
```

### Ollama container not starting
```bash
# Check logs
docker logs meai-ollama

# Restart
docker-compose restart ollama
```

## Comparison: Ollama vs OpenAI

| Feature | Ollama | OpenAI |
|---------|--------|--------|
| Cost | Free | API charges |
| Internet | Not required | Required |
| Speed (cold) | 5-30s | 1-2s |
| Speed (warm) | 2-5s | 1-2s |
| Privacy | Fully local | Cloud-based |
| Customization | Full control | Limited |
| Model options | Growing | Limited |
| Documentation | Good | Excellent |

## Next Steps

1. **Experiment with models**: Try different models to find the best balance for your use case
2. **Optimize parameters**: Adjust temperature and max_tokens for different tasks
3. **Add custom models**: Fine-tune models or add community models via Ollama
4. **Hybrid approach**: Use Ollama for development/testing, OpenAI for production

## Resources

- Ollama Documentation: https://github.com/ollama/ollama
- Available Models: https://ollama.ai/library
- API Reference: https://github.com/ollama/ollama/blob/main/docs/api.md
