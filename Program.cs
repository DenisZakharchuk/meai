using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MeAI.Data;
using MeAI.Services;
using MeAI.UI;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>(optional: true)
    .Build();

var services = new ServiceCollection();

// Add configuration FIRST (before other services)
services.AddSingleton(configuration);

// Configure logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Register database context
var connectionString = configuration.GetConnectionString("DefaultConnection");
services.AddDbContext<MeAIDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register repositories
services.AddScoped<IConversationRepository, ConversationRepository>();
services.AddScoped<IEmbeddingRepository, EmbeddingRepository>();

// Register AI services (pass configuration directly)
services.AddAIServices(configuration);

var serviceProvider = services.BuildServiceProvider();

// Run the application
var app = new InteractiveApp(serviceProvider);
await app.RunAsync();
