namespace NewsMonitor.Collectors
{
    public class CNewsCollector : RssCollectorBase
    {
        public override string SourceName => "CNews";
        protected override string FeedUrl => "https://www.cnews.ru/inc/rss/news.xml";
    }
}
