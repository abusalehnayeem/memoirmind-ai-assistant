using Telegram.Bot;

namespace MemoirMind.Api.HostedServices;

//Set Telegram Webhook on startup
public class HookInitializer(
    IServiceProvider services,
    ILogger<HookInitializer> logger,
    IConfiguration config)
    : IHostedService
{
    private readonly string? _webhookUrl = config["Telegram:WebhookUrl"];

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var bot = scope.ServiceProvider.GetRequiredService<TelegramBotClient>();

        try
        {
            if (string.IsNullOrEmpty(_webhookUrl))
            {
                logger.LogWarning("Webhook URL is not configured. Skipping webhook setup.");
                return;
            }

            await bot.SetWebhook(_webhookUrl, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            logger.LogInformation("Webhook set to {WebhookUrl}", _webhookUrl);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set webhook");
        }
    }

    // no cleanup needed
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}