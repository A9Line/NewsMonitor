using System.Collections.Generic;
using System.Linq;
using NewsMonitor.Models;

namespace NewsMonitor.Core
{
    public static class NewsFilter
    {
        // Оставляем только новости, где в заголовке/описании/тегах
        // встречается хотя бы одно ключевое слово темы.
        public static List<NewsItem> FilterByTopics(List<NewsItem> items, List<string> keywords)
        {
            if (keywords == null || keywords.Count == 0)
                return items;

            return items.Where(item =>
            {
                string text = (item.Title + " " + item.Summary + " " +
                               string.Join(" ", item.Tags)).ToLower();
                return keywords.Any(kw => text.Contains(kw.ToLower()));
            }).ToList();
        }
    }
}
