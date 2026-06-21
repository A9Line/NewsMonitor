namespace NewsMonitor.Collectors
{
    public class HabrCollector : RssCollectorBase
    {
        public override string SourceName => "Habr";
        protected override string FeedUrl => "https://habr.com/ru/rss/all/all/?fl=ru";
    }
}
