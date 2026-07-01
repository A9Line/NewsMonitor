using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NewsMonitor.Collectors;
using NewsMonitor.Core;
using NewsMonitor.Models;
using NewsMonitor.Publishers;

namespace NewsMonitor
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string configText = File.ReadAllText("config.json");
            AppConfig config = JsonConvert.DeserializeObject<AppConfig>(configText)!;

            // Источники берём прямо из конфига — код про них ничего не знает
            var perSource = new List<List<NewsItem>>();
            foreach (var src in config.Sources)
            {
                INewsCollector collector = new RssCollector(src.Name, src.Url);
                Console.WriteLine($"Собираю новости: {collector.SourceName}...");
                var news = await collector.FetchAsync();
                Console.WriteLine($"  получено: {news.Count}");

                var filtered = NewsFilter.FilterByTopics(news, config.Topics)
                    .OrderByDescending(n => n.Published ?? DateTime.MinValue)
                    .ToList();
                Console.WriteLine($"  после фильтра: {filtered.Count}");
                perSource.Add(filtered);
            }

            // Равномерный отбор по источникам до лимита
            var limited = new List<NewsItem>();
            int index = 0;
            while (limited.Count < config.MaxNewsPerDay &&
                   perSource.Any(list => index < list.Count))
            {
                foreach (var list in perSource)
                {
                    if (index < list.Count && limited.Count < config.MaxNewsPerDay)
                        limited.Add(list[index]);
                }
                index++;
            }

            Console.WriteLine($"К публикации: {limited.Count} (из разных источников)");

            INewsPublisher publisher = new TelegramPublisher(
                config.Telegram.Token, config.Telegram.ChatId);
            await publisher.PublishAsync(limited);

            Console.WriteLine("Готово. Проверь Telegram.");
        }
    }
}
