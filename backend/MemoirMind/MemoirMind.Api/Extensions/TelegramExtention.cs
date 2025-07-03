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

        // First split on newlines so we respect paragraphs
        var paragraphs = text.Split(["\r\n", "\n"], StringSplitOptions.None).Where(x => x.Length > 0).ToArray();
        var chunks = new List<string>();

        foreach (var paragraph in paragraphs)
        {
            if (paragraph.Length <= maxMessageLength)
            {
                chunks.Add(paragraph);
                continue;
            }

            var start = 0;
            while (start < paragraph.Length)
            {
                var remaining = paragraph.Length - start;
                var take = Math.Min(maxMessageLength, remaining);

                // Try to break on sentence boundary if we're not at the end
                if (take < remaining)
                {
                    var segment = paragraph.Substring(start, take);
                    // find last ., ?, or ! in that segment
                    var lastIndexOfAny = segment.LastIndexOfAny(['.', '?', '!']);
                    // but only accept it if it's reasonably far in (e.g. beyond half the chunk)
                    if (lastIndexOfAny > maxMessageLength / 4)
                        // +1 to include the punctuation
                        take = lastIndexOfAny + 1;
                }

                // avoid splitting a UTF-16 surrogate
                if (start + take < paragraph.Length
                    && char.IsHighSurrogate(paragraph[start + take - 1]))
                    take--;

                chunks.Add(paragraph.Substring(start, take));
                start += take;
            }
        }

        foreach (var chunk in chunks)
        {
            await botClient.SendMessage(chatId, chunk, cancellationToken: cancellationToken);
            // small delay to play nice with rate limits
            await Task.Delay(500, cancellationToken);
        }
    }
}