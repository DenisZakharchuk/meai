using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MeAI.Services;
using MeAI.UI;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false)
    .AddUserSecrets<Program>(optional: true)
    .Build();

var services = new ServiceCollection();

// Configure logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Information);
});

// Add configuration
services.AddSingleton(configuration);

// Register AI services
services.AddAIServices();

var serviceProvider = services.BuildServiceProvider();

// Run the application
var app = new InteractiveApp(serviceProvider);
await app.RunAsync();
