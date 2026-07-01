using System.Collections.Generic;

namespace NewsMonitor.Core
{
    public class AppConfig
    {
        public int MaxNewsPerDay { get; set; } = 5;
        public List<string> Topics { get; set; } = new List<string>();
        public TelegramConfig Telegram { get; set; } = new TelegramConfig();
        public List<SourceConfig> Sources { get; set; } = new List<SourceConfig>();
    }

    public class TelegramConfig
    {
        public string Token { get; set; } = "";
        public string ChatId { get; set; } = "";
    }

    // Описание одного источника: как называется и откуда брать
    public class SourceConfig
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
    }
}
