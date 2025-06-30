using Microsoft.SemanticKernel;
using Telegram.Bot;
using Telegram.Bot.Types;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// 1) load configuration
var ollamaCfg = builder.Configuration.GetSection("ollama");

// 2) Register Telegram Bot Client
var token = builder.Configuration["telegram:bot_token"]!;
var webhookUrl = builder.Configuration["telegram:webhookUrl"]!;
builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers()
    .AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));


// 3) Register Semantic Kernel  
builder.Services.AddKernel();
builder.Services.AddOllamaChatCompletion(modelId: ollamaCfg["model"]!, endpoint: new Uri(ollamaCfg["endpoint"]!));


var app = builder.Build();

// 4) Set Telegram Webhook on startup
app.Lifetime.ApplicationStarted.Register(async () =>
{
    var bot = app.Services.GetRequiredService<TelegramBotClient>();
    var logger = app.Services.GetRequiredService<ILogger<Program>>();

    try
    {
        await bot.SetWebhook(webhookUrl).ConfigureAwait(false);
        logger.LogInformation("Webhook set to {WebhookUrl}", webhookUrl);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to set webhook");
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseHttpsRedirection();

app.MapGet("/setWebhook", async (TelegramBotClient bot) =>
{
    await bot.SetWebhook(webhookUrl);
    return $"Webhook set to {webhookUrl}";
});

app.MapPost("/telegram", async (Update update, TelegramBotClient botClient, Kernel kernel) =>
{
    if (update.Message is { Text: { } messageText, Chat.Id: var chatId })
    {
        // Use Semantic Kernel to process the message
        var response = await kernel.InvokePromptAsync($"Answer in <200 characters: {messageText}").ConfigureAwait(false);

        // Send response back to Telegram
        await botClient.SendMessage(chatId, response.ToString());
    }

    return Results.Ok();
});

app.Run();