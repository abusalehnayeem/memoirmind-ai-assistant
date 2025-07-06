using System.Text;
using System.Text.RegularExpressions;
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
        if (string.IsNullOrWhiteSpace(text)) return;

        // Split text into sentences, preserving punctuation
        var sentences = Regex.Split(text, @"(?<=[\.\!\?])\s+")
            .Where(s => s.Length > 0)
            .ToArray();

        var chunks = new List<string>();
        var currentChunk = new StringBuilder();

        foreach (var sentence in sentences)
        {
            // If a single sentence exceeds max length, fall back to hard chunking
            if (sentence.Length > maxMessageLength)
            {
                // flush any accumulated chunk first
                if (currentChunk.Length > 0)
                {
                    chunks.Add(currentChunk.ToString());
                    currentChunk.Clear();
                }

                for (var i = 0; i < sentence.Length; i += maxMessageLength)
                {
                    var length = Math.Min(maxMessageLength, sentence.Length - i);
                    chunks.Add(sentence.Substring(i, length));
                }

                continue;
            }

            // If adding this sentence would exceed limit, flush current chunk
            if (currentChunk.Length + sentence.Length > maxMessageLength)
            {
                chunks.Add(currentChunk.ToString());
                currentChunk.Clear();
            }

            currentChunk.Append(sentence).Append(" ");
        }

        // Add any remaining text
        if (currentChunk.Length > 0)
            chunks.Add(currentChunk.ToString().Trim());

        // Send each chunk with a small delay
        foreach (var chunk in chunks)
        {
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
            await Task.Delay(500, cancellationToken);
        }
    }
}