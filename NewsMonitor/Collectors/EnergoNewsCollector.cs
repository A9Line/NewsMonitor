namespace NewsMonitor.Collectors
{
    public class EnergoNewsCollector : RssCollectorBase
    {
        public override string SourceName => "ЭнергоНьюс";
        protected override string FeedUrl => "https://energo-news.ru/feed/";
    }
}
