using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NewsMonitor.Models;

namespace NewsMonitor.Publishers
{
    public class TelegramPublisher : INewsPublisher
    {
        private readonly string _token;
        private readonly string _chatId;
        private static readonly HttpClient _http = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public TelegramPublisher(string token, string chatId)
        {
            _token = token;
            _chatId = chatId;
        }

        public async Task PublishAsync(List<NewsItem> items)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("[Telegram] Нет новостей для публикации.");
                return;
            }

            await SendMessageAsync(
                $"<b>📰 Дайджест новостей за {DateTime.Now:dd.MM.yyyy}</b>\n" +
                $"Всего материалов: {items.Count}");

            int number = 1;
            foreach (var item in items)
            {
                string text = BuildMessage(item, number);
                bool ok = await SendMessageAsync(text);
                if (ok)
                    Console.WriteLine($"  [Telegram] Отправлено: {item.Title}");
                number++;
                await Task.Delay(700);
            }
        }

        private async Task<bool> SendMessageAsync(string text)
        {
            string url = $"https://api.telegram.org/bot{_token}/sendMessage";

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    var data = new Dictionary<string, string>
                    {
                        ["chat_id"] = _chatId,
                        ["text"] = text,
                        ["parse_mode"] = "HTML",
                        ["disable_web_page_preview"] = "false"
                    };

                    var response = await _http.PostAsync(url, new FormUrlEncodedContent(data));
                    if (response.IsSuccessStatusCode)
                        return true;

                    string body = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[Telegram] Ответ сервера: {body}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Telegram] Попытка {attempt} не удалась: {ex.Message}");
                    await Task.Delay(1500);
                }
            }
            return false;
        }

        private string BuildMessage(NewsItem item, int number)
        {
            var sb = new StringBuilder();

            string date = item.Published.HasValue && item.Published.Value.Year > 2000
                ? item.Published.Value.ToString("dd.MM.yyyy HH:mm")
                : "дата не указана";

            sb.AppendLine($"<b>{number}. {Escape(item.Title)}</b>");
            sb.AppendLine();

            if (!string.IsNullOrWhiteSpace(item.Summary))
            {
                sb.AppendLine(Escape(item.Summary));
                sb.AppendLine();
            }

            sb.AppendLine($"📰 <b>{Escape(item.Source)}</b>   🗓 {date}");

            if (item.Tags.Count > 0)
            {
                var tags = item.Tags
                    .Take(4)
                    .Select(t => "#" + ToHashtag(t))
                    .ToList();
                sb.AppendLine(string.Join(" ", tags));
            }

            sb.AppendLine($"🔗 <a href=\"{item.Url}\">Читать источник</a>");

            return sb.ToString();
        }

        private string ToHashtag(string tag)
        {
            return System.Text.RegularExpressions.Regex
                .Replace(tag, @"[^\p{L}\p{N}]", "_");
        }

        private string Escape(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Replace("&", "&amp;")
                       .Replace("<", "&lt;")
                       .Replace(">", "&gt;");
        }
    }
}
