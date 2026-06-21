namespace NewsMonitor.Collectors
{
    public class IxbtCollector : RssCollectorBase
    {
        public override string SourceName => "iXBT";
        protected override string FeedUrl => "https://www.ixbt.com/export/news.rss";
    }
}
