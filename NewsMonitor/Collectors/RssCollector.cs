namespace NewsMonitor.Collectors
{
    // Универсальный RSS-коллектор.
    // Имя источника и адрес ленты задаются извне (из конфига),
    // поэтому для нового источника не нужен новый класс.
    public class RssCollector : RssCollectorBase
    {
        private readonly string _sourceName;
        private readonly string _feedUrl;

        public RssCollector(string sourceName, string feedUrl)
        {
            _sourceName = sourceName;
            _feedUrl = feedUrl;
        }

        public override string SourceName => _sourceName;
        protected override string FeedUrl => _feedUrl;
    }
}
