using System.Collections.Generic;

namespace NewsMonitor.Core
{
    // Класс, в который читается config.json
    public class AppConfig
    {
        public int MaxNewsPerDay { get; set; } = 5;
        public List<string> Topics { get; set; } = new List<string>();
        public TelegramConfig Telegram { get; set; } = new TelegramConfig();
        public List<string> Sources { get; set; } = new List<string>();
    }

    public class TelegramConfig
    {
        public string Token { get; set; } = "";
        public string ChatId { get; set; } = "";
    }
}
