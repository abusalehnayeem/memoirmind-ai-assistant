using Telegram.Bot;
using Telegram.Bot.Types;

namespace MemoirMind.Api.Extensions;

public static class TelegramExtension
{
    public static async Task SplitAndSendMessageAsync(
        this TelegramBotClient botClient,
        ChatId chatId,
        string text,
        CancellationToken cancellationToken = default)
    {
        const int maxMessageLength = 4096;

        if (string.IsNullOrWhiteSpace(text))
            return;

        var chunks = new List<string>();
        var position = 0;

        while (position < text.Length)
        {
            var chunkSize = Math.Min(maxMessageLength, text.Length - position);
            var chunk = text.Substring(position, chunkSize);
            chunks.Add(chunk);
            position += chunkSize;
        }

        foreach (var chunk in chunks)
        {
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
            await Task.Delay(500, cancellationToken); // Optional: avoid hitting rate limits
        }
    }
}