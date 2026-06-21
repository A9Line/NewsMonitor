using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using NewsMonitor.Models;

namespace NewsMonitor.Collectors
{
    // Базовый класс для всех RSS-источников.
    // Конкретный источник задаёт только имя и адрес ленты.
    public abstract class RssCollectorBase : INewsCollector
    {
        public abstract string SourceName { get; }
        protected abstract string FeedUrl { get; }

        public async Task<List<NewsItem>> FetchAsync()
        {
            var result = new List<NewsItem>();
            try
            {
                using var reader = XmlReader.Create(FeedUrl);
                var feed = await Task.Run(() => SyndicationFeed.Load(reader));

                foreach (var item in feed.Items)
                {
                    var news = new NewsItem
                    {
                        Title = CleanText(item.Title?.Text ?? ""),
                        Url = GetArticleLink(item),
                        Published = item.PublishDate.DateTime,
                        Summary = MakeSummary(item.Summary?.Text ?? ""),
                        Source = SourceName
                    };

                    foreach (var cat in item.Categories)
                        news.Tags.Add(cat.Name);

                    result.Add(news);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{SourceName}] Ошибка загрузки: {ex.Message}");
            }
            return result;
        }

        // Берём ссылку именно на статью, а не на картинку.
        // Картинки обычно имеют тип image/* или ведут на .jpg/.png.
        private string GetArticleLink(SyndicationItem item)
        {
            // 1. Ищем ссылку с типом "alternate" — это и есть статья
            var alternate = item.Links.FirstOrDefault(l =>
                l.RelationshipType == "alternate" && l.Uri != null);
            if (alternate != null)
                return alternate.Uri.ToString();

            // 2. Иначе берём первую ссылку, которая НЕ картинка
            var notImage = item.Links.FirstOrDefault(l =>
                l.Uri != null &&
                (l.MediaType == null || !l.MediaType.StartsWith("image")) &&
                !IsImageUrl(l.Uri.ToString()));
            if (notImage != null)
                return notImage.Uri.ToString();

            // 3. На крайний случай — ссылка из поля Id
            if (!string.IsNullOrEmpty(item.Id) && item.Id.StartsWith("http"))
                return item.Id;

            return item.Links.Count > 0 ? item.Links[0].Uri.ToString() : "";
        }

        private bool IsImageUrl(string url)
        {
            url = url.ToLower();
            return url.EndsWith(".jpg") || url.EndsWith(".jpeg")
                || url.EndsWith(".png") || url.EndsWith(".gif")
                || url.EndsWith(".webp") || url.Contains("/img/")
                || url.Contains("media.ixbt");
        }

        // Убираем HTML-теги и лишние пробелы из текста
        private string CleanText(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            text = Regex.Replace(text, "<.*?>", " ");           // убрать html-теги
            text = System.Net.WebUtility.HtmlDecode(text);      // &amp; -> &
            text = Regex.Replace(text, @"\s+", " ").Trim();     // лишние пробелы
            return text;
        }

        // Делаем краткое описание: чистим и обрезаем до 250 символов
        private string MakeSummary(string text)
        {
            text = CleanText(text);
            if (text.Length > 250)
                text = text.Substring(0, 250).TrimEnd() + "...";
            return text;
        }
    }
}
