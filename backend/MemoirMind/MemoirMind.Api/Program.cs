using System.Text.RegularExpressions;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.AI;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.InMemory;
using Microsoft.SemanticKernel.Connectors.Ollama;
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
builder.Services
    .AddHttpClient("tgwebhook")
    .RemoveAllLoggers()
    .AddTypedClient(httpClient => new TelegramBotClient(token, httpClient));


// 3) Register Semantic Kernel

builder.Services.AddKernel();
builder.Services
    .AddOllamaChatCompletion(ollamaCfg["chatCompletionModel"]!, new Uri(ollamaCfg["endpoint"]!))
    .AddOllamaEmbeddingGenerator(ollamaCfg["embeddingGeneratorModel"]!, new Uri(ollamaCfg["endpoint"]!));

// Register In-Memory Vector Store
builder.Services.AddSingleton<InMemoryVectorStore>();


// added rate limiting to avoid hitting Telegram API limits
builder.Services.AddRateLimiter(rateLimiterOptions =>
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    rateLimiterOptions.AddTokenBucketLimiter("telegram-endpoint", options =>
    {
        options.TokenLimit = 100;
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 5;
        options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
        options.TokensPerPeriod = 20;
        options.AutoReplenishment = true;
    });
});

var app = builder.Build();

app.MapOpenApi();

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

app.UseHttpsRedirection();
app.UseRateLimiter();


app.MapGet("/setWebhook", async (TelegramBotClient bot) =>
{
    await bot.SetWebhook(webhookUrl);
    return $"Webhook set to {webhookUrl}";
}).RequireRateLimiting("telegram-endpoint");

app.MapPost("/telegram", async (Update update,
    TelegramBotClient botClient,
    Kernel kernel,
    IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator,
    InMemoryVectorStore vectorStore,
    HttpContext httpContext) =>
{
    if (update.Message is not { Text: { } messageText, Chat.Id: var chatId }) return Results.NotFound();

    var userId = update.Message.From!.Id.ToString();
    // Get or create user-specific collection
    var collection = vectorStore.GetCollection<string, VectorStoreRecord<string>>(userId);
    await collection.EnsureCollectionExistsAsync(httpContext.RequestAborted);

    // Embed & store the incoming user message
    var embedResults = await embeddingGenerator.GenerateAsync([messageText], cancellationToken: httpContext.RequestAborted);
    var userEmbedding = embedResults.FirstOrDefault();
    if (userEmbedding is null)
    {
        await botClient.SendMessage(chatId, "Failed to generate embedding for your message.");
        return Results.NotFound();
    }

    var record = new VectorStoreRecord<string>
    {
        Key = Guid.NewGuid().ToString(),
        Data = $"User: {messageText}",
        Vector = userEmbedding.Vector
    };


    await collection.UpsertAsync([record]);

    //// 3. Retrieve relevant context from vector store
    var matches = await collection.SearchAsync(userEmbedding.Vector, 5).ToListAsync(httpContext.RequestAborted);

    //// 4. Build chat history context
    var chatHistory = string.Join("\n", matches.OrderByDescending(x => x.Record.Data).Select(x => x.Record.Data));

    var settings = new OllamaPromptExecutionSettings
    {
        TopP = 0.9f,
        TopK = 100,
        Temperature = 0.7f,
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    };

    // Define your prompt template as a string
    const string prompt = """
                          [Role]
                          You are a helpful AI assistant for a Telegram bot. Your responses should be:
                          - Clear and concise
                          - Factually accurate
                          - Contextually appropriate
                          - Human-friendly and conversational
                          [Chat History]
                          {{$chat_history}}

                          [User Query]
                          {{$user_input}}

                          [Instructions]
                          1. Understand the user's intent and context
                          2. Provide practical, actionable information
                          3. Use bullet points for complex answers
                          4. Add examples when helpful
                          5. Suggest follow-up questions when relevant
                          6. If unsure, ask for clarification rather than guessing
                          7. Do NOT output any internal thought tags like <think>…</think>; only return the final user-facing text.

                          [Response Format]
                          - Keep paragraphs under 3 sentences
                          - Use bold for key terms
                          - Avoid technical jargon unless requested

                          [Response]
                          """;

    var kernelArguments = new KernelArguments
    {
        ["user_input"] = messageText,
        ["chat_history"] = chatHistory,
        ExecutionSettings = new Dictionary<string, PromptExecutionSettings>
        {
            { "default", settings }
        }
    };
    // Execute with Semantic Kernel
    var response = await kernel
        .InvokePromptAsync(
            prompt,
            kernelArguments, cancellationToken: httpContext.RequestAborted)
        .ConfigureAwait(false);

    // Remove anything between <think> and </think>, including the tags themselves
    var cleanedResponse = Regex.Replace(response.ToString(),
        @"<think>[\s\S]*?<\/think>",
        string.Empty,
        RegexOptions.IgnoreCase).Trim();
    // Send response back to Telegram
    await botClient.SendMessage(chatId, cleanedResponse, cancellationToken: httpContext.RequestAborted);

    return Results.Ok();
}).RequireRateLimiting("telegram-endpoint");

app.Run();