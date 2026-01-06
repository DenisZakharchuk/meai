using MeAI.Data;
using MeAI.Services;
using OpenAI.Chat;

namespace MeAI.Services;

/// <summary>
/// Example demonstrating conversation persistence with PostgreSQL
/// </summary>
public class PersistenceExample
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IEmbeddingRepository _embeddingRepository;
    private readonly IChatService _chatService;
    private readonly IEmbeddingService _embeddingService;

    public PersistenceExample(
        IConversationRepository conversationRepository,
        IEmbeddingRepository embeddingRepository,
        IChatService chatService,
        IEmbeddingService embeddingService)
    {
        _conversationRepository = conversationRepository;
        _embeddingRepository = embeddingRepository;
        _chatService = chatService;
        _embeddingService = embeddingService;
    }

    public async Task RunAsync()
    {
        Console.WriteLine("\n=== Persistence Example ===");
        Console.WriteLine("This example demonstrates storing conversations and embeddings in PostgreSQL.\n");

        try
        {
            // Step 1: Create a new conversation
            Console.WriteLine("Step 1: Creating a new conversation...");
            var conversation = await _conversationRepository.CreateAsync(
                title: "Technical Discussion",
                modelName: "gpt-4-turbo");
            Console.WriteLine($"✓ Created conversation #{conversation.Id}: {conversation.Title}\n");

            // Step 2: Add messages to the conversation
            Console.WriteLine("Step 2: Adding messages to the conversation...");
            
            var systemMessage = ChatMessage.CreateSystemMessage(
                "You are a helpful AI assistant specializing in C# and .NET development.");
            
            var userMessage = ChatMessage.CreateUserMessage(
                "What are the benefits of using Entity Framework Core?");
            
            // Store messages in database
            await _conversationRepository.AddMessageAsync(
                conversation.Id, "system", systemMessage.Content?.ToString() ?? "System message");
            await _conversationRepository.AddMessageAsync(
                conversation.Id, "user", userMessage.Content?.ToString() ?? "User question");
            
            Console.WriteLine("✓ Added system and user messages to conversation\n");

            // Step 3: Get AI response and store it
            Console.WriteLine("Step 3: Getting AI response...");
            var messages = new List<ChatMessage> { systemMessage, userMessage };
            
            try
            {
                var response = await _chatService.GetChatCompletionAsync(messages);
                Console.WriteLine($"AI Response: {response}\n");
                
                // Store AI response in database
                await _conversationRepository.AddMessageAsync(
                    conversation.Id, "assistant", response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Note: API call failed (check your API key): {ex.Message}");
                Console.WriteLine("Storing a demo response instead...\n");
                
                var demoResponse = "Entity Framework Core provides a powerful ORM with LINQ support, " +
                    "automatic change tracking, and seamless database migrations. It simplifies data access " +
                    "and maintains type safety throughout your application.";
                
                await _conversationRepository.AddMessageAsync(
                    conversation.Id, "assistant", demoResponse);
            }

            // Step 4: Store embeddings for semantic search
            Console.WriteLine("Step 4: Storing text embeddings for semantic search...");
            
            var textsToEmbed = new[]
            {
                "Entity Framework Core is an ORM framework",
                "PostgreSQL is a powerful relational database",
                "Vector databases enable semantic search"
            };
            
            foreach (var text in textsToEmbed)
            {
                try
                {
                    var embedding = await _embeddingService.GetEmbeddingAsync(text);
                    await _embeddingRepository.StoreAsync(text, embedding);
                    Console.WriteLine($"✓ Stored embedding for: \"{text}\"");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Note: Embedding failed (check your API key): {ex.Message}");
                    // Store with dummy embedding for demo
                    var dummyEmbedding = new float[1536];
                    await _embeddingRepository.StoreAsync(text, dummyEmbedding);
                    Console.WriteLine($"✓ Stored demo embedding for: \"{text}\"");
                }
            }
            Console.WriteLine();

            // Step 5: Retrieve and display stored conversation
            Console.WriteLine("Step 5: Retrieving conversation from database...");
            var retrievedConversation = await _conversationRepository.GetByIdAsync(conversation.Id);
            
            if (retrievedConversation != null)
            {
                Console.WriteLine($"Conversation: {retrievedConversation.Title}");
                Console.WriteLine($"Created: {retrievedConversation.CreatedAt:yyyy-MM-dd HH:mm:ss}");
                Console.WriteLine($"Messages: {retrievedConversation.Messages.Count}");
                Console.WriteLine("\nMessage History:");
                
                foreach (var msg in retrievedConversation.Messages)
                {
                    var preview = msg.Content.Length > 80 
                        ? msg.Content.Substring(0, 80) + "..." 
                        : msg.Content;
                    Console.WriteLine($"  [{msg.Role.ToUpper()}]: {preview}");
                }
            }
            Console.WriteLine();

            // Step 6: List all conversations
            Console.WriteLine("Step 6: All stored conversations:");
            var allConversations = await _conversationRepository.GetAllAsync();
            foreach (var conv in allConversations)
            {
                Console.WriteLine($"  - {conv.Title} (ID: {conv.Id}, Messages: {conv.Messages.Count})");
            }
            Console.WriteLine();

            // Step 7: List all embeddings
            Console.WriteLine("Step 7: All stored embeddings:");
            var allEmbeddings = await _embeddingRepository.GetAllAsync();
            foreach (var emb in allEmbeddings)
            {
                var preview = emb.Text.Length > 50 
                    ? emb.Text.Substring(0, 50) + "..." 
                    : emb.Text;
                Console.WriteLine($"  - {preview} (ID: {emb.Id})");
            }

            Console.WriteLine("\n✓ Persistence example completed successfully!");
            Console.WriteLine("\nNote: All data is stored in PostgreSQL and persists between application runs.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error in persistence example: {ex.Message}");
            Console.WriteLine("\nMake sure PostgreSQL is running with: docker-compose up");
        }
    }
}
